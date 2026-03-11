namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Описание дополнительного дропа, который можно выдать игроку.
    /// </summary>
    public sealed class TicTacToeRewardDropData
    {
        public TicTacToeRewardDropData(
            string dropId,
            string displayName,
            TicTacToeRewardRarity rarity)
        {
            DropId = dropId ?? string.Empty;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? DropId : displayName;
            Rarity = rarity;
        }

        public string DropId { get; }

        public string DisplayName { get; }

        public TicTacToeRewardRarity Rarity { get; }
    }
}
