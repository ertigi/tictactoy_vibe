using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Api;
using UnityEngine;

namespace GameModules.TicTacToe.Demo
{
    /// <summary>
    /// Имитирует внешний код основной игры, который запускает миниигру как встроенный модуль.
    /// </summary>
    public sealed class TicTacToeDemoCaller
    {
        private readonly ITicTacToeLauncher _launcher;
        private readonly ITicTacToeModule _module;

        private int _launchCounter;

        public TicTacToeDemoCaller(
            ITicTacToeLauncher launcher,
            ITicTacToeModule module)
        {
            _launcher = launcher;
            _module = module;
        }

        public bool IsRunning => _module.IsRunning;

        public UniTask PreloadAsync(CancellationToken cancellationToken = default)
        {
            return _module.PreloadAsync(cancellationToken);
        }

        public UniTask<TicTacToeResult> LaunchAsync(
            RectTransform host,
            CancellationToken cancellationToken = default)
        {
            _launchCounter++;

            return _launcher.LaunchAsync(
                new TicTacToeLaunchOptions
                {
                    Parent = host,
                    SessionId = $"demo-session-{_launchCounter:000}",
                    AutoCloseOnFinish = true,
                    AllowRestart = false,
                    ResultAutoCloseDelaySecondsOverride = 1.75f,
                    RewardOverrides = new TicTacToeRewardOverrides
                    {
                        WinReward = 150,
                        DrawReward = 70,
                        LoseReward = 25,
                        QuickWinBonusPerEmptyCell = 10
                    }
                },
                cancellationToken);
        }

        public void Release()
        {
            _module.Release();
        }
    }
}
