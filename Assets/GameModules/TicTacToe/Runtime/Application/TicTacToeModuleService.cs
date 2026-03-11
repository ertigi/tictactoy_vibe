using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Config;
using GameModules.TicTacToe.Domain;
using GameModules.TicTacToe.Infrastructure;
using GameModules.TicTacToe.Presentation;

namespace GameModules.TicTacToe.Application
{
    /// <summary>
    /// Фасад модуля, который будет дергать внешний код.
    /// </summary>
    public sealed class TicTacToeModuleService : ITicTacToeModule, ITicTacToeLauncher
    {
        private readonly TicTacToeModuleSettings _moduleSettings;
        private readonly ITicTacToeScreenFactory _screenFactory;
        private readonly TicTacToeSessionController.Factory _sessionControllerFactory;
        private readonly TicTacToePresenter.Factory _presenterFactory;

        public TicTacToeModuleService(
            TicTacToeModuleSettings moduleSettings,
            ITicTacToeScreenFactory screenFactory,
            TicTacToeSessionController.Factory sessionControllerFactory,
            TicTacToePresenter.Factory presenterFactory)
        {
            _moduleSettings = moduleSettings;
            _screenFactory = screenFactory;
            _sessionControllerFactory = sessionControllerFactory;
            _presenterFactory = presenterFactory;
        }

        public bool IsRunning { get; private set; }

        public UniTask PreloadAsync(CancellationToken cancellationToken = default)
        {
            return _screenFactory.PreloadAsync(cancellationToken);
        }

        public UniTask<TicTacToeResult> LaunchAsync(
            TicTacToeLaunchOptions options,
            CancellationToken cancellationToken = default)
        {
            return RunAsync(options, cancellationToken);
        }

        public async UniTask<TicTacToeResult> RunAsync(
            TicTacToeLaunchOptions options,
            CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("TicTacToe module is already running.");
            }

            options ??= new TicTacToeLaunchOptions();

            TicTacToeScreenInstance screenInstance = null;
            ITicTacToeSessionController sessionController = null;
            TicTacToePresenter presenter = null;

            try
            {
                IsRunning = true;
                screenInstance = await _screenFactory.CreateAsync(options.Parent, cancellationToken);
                sessionController = _sessionControllerFactory.Create();
                presenter = _presenterFactory.Create(screenInstance.View, sessionController);

                while (true)
                {
                    sessionController.StartSession(options);
                    presenter.BeginSession(
                        options.AutoCloseOnFinish,
                        options.AllowRestart,
                        ResolveResultAutoCloseDelaySeconds(options));

                    var result = await sessionController.WaitForCompletionAsync(cancellationToken);
                    var action = await presenter.WaitForPostGameActionAsync(cancellationToken);

                    if (action == TicTacToePostGameAction.Restart)
                    {
                        continue;
                    }

                    return result;
                }
            }
            catch (OperationCanceledException)
            {
                sessionController?.Cancel();
                return new TicTacToeResult(
                    options.SessionId,
                    TicTacToeOutcome.Cancelled,
                    TicTacToeRewardData.Empty,
                    sessionController?.CurrentSnapshot?.TurnsCount ?? 0,
                    Array.Empty<BoardPosition>(),
                    true);
            }
            finally
            {
                presenter?.Dispose();
                screenInstance?.Release();
                IsRunning = false;
            }
        }

        public void Release()
        {
            _screenFactory.ReleasePreloaded();
        }

        private float ResolveResultAutoCloseDelaySeconds(TicTacToeLaunchOptions options)
        {
            return options?.ResultAutoCloseDelaySecondsOverride ?? _moduleSettings.ResultAutoCloseDelaySeconds;
        }
    }
}
