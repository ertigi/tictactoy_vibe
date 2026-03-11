using System;

namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Данные о награде, которые внешний код может выдать игроку.
    /// </summary>
    public sealed class TicTacToeRewardData
    {
        public static readonly TicTacToeRewardData Empty = new(0, 0, string.Empty, null);

        public TicTacToeRewardData(
            int baseAmount,
            int bonusAmount,
            string currencyId,
            TicTacToeRewardDropData drop)
        {
            BaseAmount = Math.Max(0, baseAmount);
            BonusAmount = Math.Max(0, bonusAmount);
            CurrencyId = currencyId ?? string.Empty;
            Drop = drop;
        }

        public int BaseAmount { get; }

        public int BonusAmount { get; }

        public int TotalAmount => BaseAmount + BonusAmount;

        public string CurrencyId { get; }

        public string RewardId => CurrencyId;

        public TicTacToeRewardDropData Drop { get; }

        public bool HasCurrency => TotalAmount > 0 && !string.IsNullOrWhiteSpace(CurrencyId);

        public bool HasDrop => Drop != null;

        public bool HasAnyReward => HasCurrency || HasDrop;
    }
}
