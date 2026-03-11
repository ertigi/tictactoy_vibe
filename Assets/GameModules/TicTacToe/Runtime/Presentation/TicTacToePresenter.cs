using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Application;
using GameModules.TicTacToe.Domain;
using Zenject;

namespace GameModules.TicTacToe.Presentation
{
    /// <summary>
    /// Связывает view и controller без прямой зависимости UI от бизнес-логики.
    /// </summary>
    public sealed class TicTacToePresenter : IDisposable
    {
        public sealed class Factory : PlaceholderFactory<TicTacToeScreenView, ITicTacToeSessionController, TicTacToePresenter>
        {
        }

        private readonly TicTacToeScreenView _view;
        private readonly ITicTacToeSessionController _sessionController;
        private readonly CancellationTokenSource _disposeCancellationTokenSource = new();

        private UniTaskCompletionSource<TicTacToePostGameAction> _postGameActionSource = new();
        private bool _autoCloseOnFinish;
        private bool _allowRestart;
        private bool _sessionFinished;
        private float _autoCloseDelaySeconds;
        private int _sessionVersion;

        public TicTacToePresenter(
            TicTacToeScreenView view,
            ITicTacToeSessionController sessionController)
        {
            _view = view;
            _sessionController = sessionController;

            _view.CellClicked += HandleCellClicked;
            _view.CloseRequested += HandleCloseRequested;
            _view.RestartRequested += HandleRestartRequested;
            _sessionController.StateChanged += HandleStateChanged;
            _sessionController.Completed += HandleCompleted;

            _view.Render(_sessionController.CurrentSnapshot);
        }

        public void BeginSession(
            bool autoCloseOnFinish,
            bool allowRestart,
            float autoCloseDelaySeconds)
        {
            _autoCloseOnFinish = autoCloseOnFinish;
            _allowRestart = allowRestart;
            _autoCloseDelaySeconds = Math.Max(0f, autoCloseDelaySeconds);
            _sessionFinished = false;
            _postGameActionSource = new UniTaskCompletionSource<TicTacToePostGameAction>();
            _sessionVersion++;

            _view.ConfigureForSession(allowRestart);
            _view.Render(_sessionController.CurrentSnapshot);
        }

        public UniTask<TicTacToePostGameAction> WaitForPostGameActionAsync(
            CancellationToken cancellationToken = default)
        {
            return cancellationToken.CanBeCanceled
                ? _postGameActionSource.Task.AttachExternalCancellation(cancellationToken)
                : _postGameActionSource.Task;
        }

        public void Dispose()
        {
            _disposeCancellationTokenSource.Cancel();
            _disposeCancellationTokenSource.Dispose();

            _view.CellClicked -= HandleCellClicked;
            _view.CloseRequested -= HandleCloseRequested;
            _view.RestartRequested -= HandleRestartRequested;
            _sessionController.StateChanged -= HandleStateChanged;
            _sessionController.Completed -= HandleCompleted;
        }

        private void HandleCellClicked(int index)
        {
            HandleCellClickedAsync(index).Forget();
        }

        private void HandleCloseRequested()
        {
            if (_sessionController.IsRunning && !_sessionFinished)
            {
                _sessionController.Cancel();
                return;
            }

            _postGameActionSource.TrySetResult(TicTacToePostGameAction.Close);
        }

        private void HandleRestartRequested()
        {
            if (_sessionFinished && _allowRestart)
            {
                _postGameActionSource.TrySetResult(TicTacToePostGameAction.Restart);
            }
        }

        private void HandleStateChanged(TicTacToeSessionSnapshot snapshot)
        {
            _view.Render(snapshot);
        }

        private void HandleCompleted(TicTacToeResult result)
        {
            _sessionFinished = true;
            _view.RenderResult(result, _allowRestart && !result.WasCancelled);

            if (result.WasCancelled)
            {
                _postGameActionSource.TrySetResult(TicTacToePostGameAction.Close);
                return;
            }

            if (_autoCloseOnFinish)
            {
                AutoCloseAfterDelayAsync(_sessionVersion).Forget();
            }
        }

        private async UniTaskVoid HandleCellClickedAsync(int index)
        {
            try
            {
                var position = BoardPosition.FromIndex(index);
                await _sessionController.TryMakePlayerMoveAsync(
                    position,
                    _disposeCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Игнорируем отмену при уничтожении presenter.
            }
        }

        private async UniTaskVoid AutoCloseAfterDelayAsync(int sessionVersion)
        {
            try
            {
                if (_autoCloseDelaySeconds > 0f)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(_autoCloseDelaySeconds),
                        cancellationToken: _disposeCancellationTokenSource.Token);
                }

                if (sessionVersion != _sessionVersion || !_sessionFinished)
                {
                    return;
                }

                _postGameActionSource.TrySetResult(TicTacToePostGameAction.Close);
            }
            catch (OperationCanceledException)
            {
                // Игнорируем отмену при уничтожении presenter.
            }
        }
    }
}
