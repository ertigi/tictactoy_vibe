using System;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Config;

namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Базовая реализация расчета награды на основе конфига модуля.
    /// </summary>
    public sealed class DefaultTicTacToeRewardCalculator : ITicTacToeRewardCalculator
    {
        private readonly TicTacToeModuleSettings _moduleSettings;
        private readonly Random _random = new();

        public DefaultTicTacToeRewardCalculator(TicTacToeModuleSettings moduleSettings)
        {
            _moduleSettings = moduleSettings;
        }

        public TicTacToeRewardData Calculate(
            TicTacToeRoundOutcome outcome,
            int emptyCellsCount,
            TicTacToeRewardOverrides overrides = null)
        {
            var rewardSettings = _moduleSettings.RewardSettings;
            var baseAmount = rewardSettings.GetBaseReward(outcome, overrides);
            var bonusAmount = rewardSettings.GetBonusReward(outcome, emptyCellsCount, overrides);
            var drop = TryRollDrop(outcome, rewardSettings.DropSettings);

            return new TicTacToeRewardData(
                baseAmount,
                bonusAmount,
                rewardSettings.CurrencyId,
                drop);
        }

        private TicTacToeRewardDropData TryRollDrop(
            TicTacToeRoundOutcome outcome,
            TicTacToeRewardDropSettings dropSettings)
        {
            if (dropSettings == null || !dropSettings.Enabled)
            {
                return null;
            }

            var dropChance = dropSettings.GetDropChance(outcome);

            if (dropChance <= 0f || _random.NextDouble() > dropChance)
            {
                return null;
            }

            var entries = dropSettings.Entries;
            var totalWeight = 0;

            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null)
                {
                    totalWeight += entries[i].Weight;
                }
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            var roll = _random.Next(0, totalWeight);

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry == null || entry.Weight <= 0)
                {
                    continue;
                }

                if (roll < entry.Weight)
                {
                    return new TicTacToeRewardDropData(
                        entry.DropId,
                        entry.DisplayName,
                        entry.Rarity);
                }

                roll -= entry.Weight;
            }

            return null;
        }
    }
}
