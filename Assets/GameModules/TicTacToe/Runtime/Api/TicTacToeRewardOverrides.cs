namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Локальные override-настройки награды для конкретного запуска.
    /// </summary>
    public sealed class TicTacToeRewardOverrides
    {
        public int? WinReward { get; set; }

        public int? DrawReward { get; set; }

        public int? LoseReward { get; set; }

        public int? QuickWinBonusPerEmptyCell { get; set; }
    }
}
