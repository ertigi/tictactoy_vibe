using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace GameModules.TicTacToe.Demo
{
    /// <summary>
    /// Точка входа demo-сценария. Показывает внешний вызов модуля и получение результата назад.
    /// </summary>
    public sealed class TicTacToeDemoEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private TicTacToeDemoView view;

        private readonly CancellationTokenSource _lifetimeCancellationTokenSource = new();

        private TicTacToeDemoCaller _demoCaller;
        private bool _isBusy;

        [Inject]
        public void Construct(TicTacToeDemoCaller demoCaller)
        {
            _demoCaller = demoCaller;
        }

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<TicTacToeDemoView>();
            }

            if (view == null)
            {
                view = gameObject.AddComponent<TicTacToeDemoView>();
            }
        }

        private void OnEnable()
        {
            if (view != null)
            {
                view.LaunchRequested += HandleLaunchRequested;
                view.ReplayRequested += HandleReplayRequested;
            }
        }

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private void OnDisable()
        {
            if (view != null)
            {
                view.LaunchRequested -= HandleLaunchRequested;
                view.ReplayRequested -= HandleReplayRequested;
            }
        }

        private void OnDestroy()
        {
            _lifetimeCancellationTokenSource.Cancel();
            _lifetimeCancellationTokenSource.Dispose();
            _demoCaller?.Release();
        }

        private void HandleLaunchRequested()
        {
            LaunchMiniGameAsync().Forget();
        }

        private void HandleReplayRequested()
        {
            LaunchMiniGameAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            view.ShowWarmupState();

            if (_demoCaller == null)
            {
                view.ShowError("Demo caller is not injected. Ensure SceneContext installs TicTacToeDemoInstaller.");
                return;
            }

            try
            {
                await _demoCaller.PreloadAsync(_lifetimeCancellationTokenSource.Token);
                view.ShowReadyState();
            }
            catch (OperationCanceledException)
            {
                // Игнорируем отмену при закрытии demo-сцены.
            }
            catch (Exception exception)
            {
                view.ShowWarmupFailed(exception.Message);
            }
        }

        private async UniTaskVoid LaunchMiniGameAsync()
        {
            if (_isBusy || _demoCaller == null || view == null || view.ModuleHost == null)
            {
                return;
            }

            _isBusy = true;
            view.ShowLaunchState();

            try
            {
                var result = await _demoCaller.LaunchAsync(
                    view.ModuleHost,
                    _lifetimeCancellationTokenSource.Token);
                view.ShowResult(result);
            }
            catch (OperationCanceledException)
            {
                // Игнорируем отмену при уничтожении demo entry point.
            }
            catch (Exception exception)
            {
                view.ShowError(exception.Message);
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
