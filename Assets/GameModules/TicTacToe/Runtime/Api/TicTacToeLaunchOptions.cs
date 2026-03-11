using UnityEngine;

namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Параметры запуска одной игровой сессии.
    /// </summary>
    public sealed class TicTacToeLaunchOptions
    {
        public Transform Parent { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public bool AutoCloseOnFinish { get; set; } = true;

        public bool AllowRestart { get; set; } = true;

        public float? BotTurnDelaySecondsOverride { get; set; }

        public float? ResultAutoCloseDelaySecondsOverride { get; set; }

        public TicTacToeRewardOverrides RewardOverrides { get; set; }
    }
}
