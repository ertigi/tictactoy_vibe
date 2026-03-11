using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Config;
using GameModules.TicTacToe.Domain;
using Zenject;

namespace GameModules.TicTacToe.Application
{
    /// <summary>
    /// Контроллер хранит состояние матча и координирует ходы.
    /// </summary>
    public sealed class TicTacToeSessionController : ITicTacToeSessionController
    {
        public sealed class Factory : PlaceholderFactory<TicTacToeSessionController>
        {
        }

        private readonly TicTacToeRules _rules;
        private readonly ITicTacToeBotStrategy _botStrategy;
        private readonly ITicTacToeRewardCalculator _rewardCalculator;
        private readonly TicTacToeModuleSettings _moduleSettings;

        private readonly TicTacToeBoard _board = new();

        private UniTaskCompletionSource<TicTacToeResult> _completionSource = new();
        private TicTacToeLaunchOptions _launchOptions;
        private string _sessionId = string.Empty;
        private BoardMark _currentTurn;
        private bool _isInputLocked;
        private int _turnsCount;

        [Inject]
        public TicTacToeSessionController(
            TicTacToeRules rules,
            ITicTacToeBotStrategy botStrategy,
            ITicTacToeRewardCalculator rewardCalculator,
            TicTacToeModuleSettings moduleSettings)
        {
            _rules = rules;
            _botStrategy = botStrategy;
            _rewardCalculator = rewardCalculator;
            _moduleSettings = moduleSettings;
            CurrentSnapshot = TicTacToeSessionSnapshot.Empty;
        }

        public bool IsRunning { get; private set; }

        public TicTacToeSessionSnapshot CurrentSnapshot { get; private set; }

        public event Action<TicTacToeSessionSnapshot> StateChanged;

        public event Action<TicTacToeResult> Completed;

        public void StartSession(TicTacToeLaunchOptions options)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("TicTacToe session is already running.");
            }

            _launchOptions = options ?? new TicTacToeLaunchOptions();
            _sessionId = string.IsNullOrWhiteSpace(_launchOptions.SessionId)
                ? Guid.NewGuid().ToString("N")
                : _launchOptions.SessionId;
            _completionSource = new UniTaskCompletionSource<TicTacToeResult>();
            _board.Reset();
            _turnsCount = 0;
            _currentTurn = BoardMark.Player;
            _isInputLocked = false;
            IsRunning = true;

            PublishSnapshot(TicTacToeBoardEvaluation.InProgress);
        }

        public async UniTask<bool> TryMakePlayerMoveAsync(
            BoardPosition position,
            CancellationToken cancellationToken = default)
        {
            if (!IsRunning || _isInputLocked || _currentTurn != BoardMark.Player)
            {
                return false;
            }

            if (!_board.TrySetMark(position, BoardMark.Player))
            {
                return false;
            }

            _turnsCount++;

            if (TryComplete())
            {
                return true;
            }

            _currentTurn = BoardMark.Bot;
            _isInputLocked = true;
            PublishSnapshot(TicTacToeBoardEvaluation.InProgress);

            var delaySeconds = ResolveBotDelaySeconds();

            try
            {
                if (delaySeconds > 0f)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(delaySeconds),
                        cancellationToken: cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                RestorePlayerTurnAfterInterruptedBotDelay();
                throw;
            }

            ExecuteBotTurn();
            return true;
        }

        public UniTask<TicTacToeResult> WaitForCompletionAsync(CancellationToken cancellationToken = default)
        {
            return cancellationToken.CanBeCanceled
                ? _completionSource.Task.AttachExternalCancellation(cancellationToken)
                : _completionSource.Task;
        }

        public void Cancel()
        {
            if (!IsRunning)
            {
                return;
            }

            CompleteSession(CreateCancelledResult());
        }

        private void ExecuteBotTurn()
        {
            if (!IsRunning)
            {
                return;
            }

            if (!TryResolveBotMove(out var position))
            {
                if (TryComplete())
                {
                    return;
                }

                _currentTurn = BoardMark.Player;
                _isInputLocked = false;
                PublishSnapshot(TicTacToeBoardEvaluation.InProgress);
                return;
            }

            _board.TrySetMark(position, BoardMark.Bot);
            _turnsCount++;

            if (TryComplete())
            {
                return;
            }

            _currentTurn = BoardMark.Player;
            _isInputLocked = false;
            PublishSnapshot(TicTacToeBoardEvaluation.InProgress);
        }

        private bool TryComplete()
        {
            var evaluation = _rules.Evaluate(_board);

            if (!evaluation.IsFinished)
            {
                return false;
            }

            CompleteSession(CreateResult(evaluation), evaluation);
            return true;
        }

        private void CompleteSession(
            TicTacToeResult result,
            TicTacToeBoardEvaluation evaluation = null)
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            _currentTurn = BoardMark.None;
            _isInputLocked = true;

            PublishSnapshot(evaluation ?? TicTacToeBoardEvaluation.InProgress);
            _completionSource.TrySetResult(result);
            Completed?.Invoke(result);
        }

        private void PublishSnapshot(TicTacToeBoardEvaluation evaluation)
        {
            var snapshot = new TicTacToeSessionSnapshot(
                _board.GetCellsCopy(),
                _currentTurn,
                _isInputLocked,
                (evaluation != null && evaluation.IsFinished) || !IsRunning,
                evaluation?.Outcome ?? TicTacToeRoundOutcome.None,
                evaluation?.WinningPositions,
                _turnsCount);

            CurrentSnapshot = snapshot;
            StateChanged?.Invoke(snapshot);
        }

        private TicTacToeResult CreateResult(TicTacToeBoardEvaluation evaluation)
        {
            var reward = _rewardCalculator.Calculate(
                evaluation.Outcome,
                _board.EmptyCellsCount,
                _launchOptions.RewardOverrides);

            return new TicTacToeResult(
                _sessionId,
                MapOutcome(evaluation.Outcome),
                reward,
                _turnsCount,
                evaluation.WinningPositions,
                false);
        }

        private TicTacToeResult CreateCancelledResult()
        {
            return new TicTacToeResult(
                _sessionId,
                TicTacToeOutcome.Cancelled,
                TicTacToeRewardData.Empty,
                _turnsCount,
                Array.Empty<BoardPosition>(),
                true);
        }

        private bool TryResolveBotMove(out BoardPosition position)
        {
            if (_botStrategy.TryChooseMove(_board, BoardMark.Bot, BoardMark.Player, out position)
                && _board.CanSetMark(position))
            {
                return true;
            }

            var availablePositions = _board.GetAvailablePositions();

            if (availablePositions.Count > 0)
            {
                position = availablePositions[0];
                return true;
            }

            position = default;
            return false;
        }

        private float ResolveBotDelaySeconds()
        {
            return _launchOptions?.BotTurnDelaySecondsOverride ?? _moduleSettings.DefaultBotTurnDelaySeconds;
        }

        private void RestorePlayerTurnAfterInterruptedBotDelay()
        {
            if (!IsRunning || _currentTurn != BoardMark.Bot)
            {
                return;
            }

            _currentTurn = BoardMark.Player;
            _isInputLocked = false;
            PublishSnapshot(TicTacToeBoardEvaluation.InProgress);
        }

        private static TicTacToeOutcome MapOutcome(TicTacToeRoundOutcome outcome)
        {
            switch (outcome)
            {
                case TicTacToeRoundOutcome.PlayerWin:
                    return TicTacToeOutcome.PlayerWin;
                case TicTacToeRoundOutcome.BotWin:
                    return TicTacToeOutcome.BotWin;
                case TicTacToeRoundOutcome.Draw:
                    return TicTacToeOutcome.Draw;
                default:
                    return TicTacToeOutcome.None;
            }
        }
    }
}
