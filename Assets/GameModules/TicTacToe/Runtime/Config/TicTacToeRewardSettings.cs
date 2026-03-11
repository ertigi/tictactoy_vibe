using System;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Domain;
using UnityEngine;

namespace GameModules.TicTacToe.Config
{
    /// <summary>
    /// Базовая конфигурация награды модуля.
    /// </summary>
    [Serializable]
    public sealed class TicTacToeRewardSettings
    {
        [SerializeField]
        private string currencyId = "coins";

        [SerializeField]
        [Min(0)]
        private int winReward = 100;

        [SerializeField]
        [Min(0)]
        private int drawReward = 30;

        [SerializeField]
        [Min(0)]
        private int loseReward = 10;

        [SerializeField]
        [Min(0)]
        private int quickWinBonusPerEmptyCell = 5;

        [SerializeField]
        private TicTacToeRewardDropSettings dropSettings = new();

        public string CurrencyId => currencyId ?? string.Empty;

        public TicTacToeRewardDropSettings DropSettings => dropSettings;

        public int GetBaseReward(
            TicTacToeRoundOutcome outcome,
            TicTacToeRewardOverrides overrides = null)
        {
            switch (outcome)
            {
                case TicTacToeRoundOutcome.PlayerWin:
                    return overrides?.WinReward ?? winReward;
                case TicTacToeRoundOutcome.Draw:
                    return overrides?.DrawReward ?? drawReward;
                case TicTacToeRoundOutcome.BotWin:
                    return overrides?.LoseReward ?? loseReward;
                default:
                    return 0;
            }
        }

        public int GetBonusReward(
            TicTacToeRoundOutcome outcome,
            int emptyCellsCount,
            TicTacToeRewardOverrides overrides = null)
        {
            if (outcome != TicTacToeRoundOutcome.PlayerWin)
            {
                return 0;
            }

            var perCellBonus = overrides?.QuickWinBonusPerEmptyCell ?? quickWinBonusPerEmptyCell;
            return Math.Max(0, emptyCellsCount) * Math.Max(0, perCellBonus);
        }
    }
}
