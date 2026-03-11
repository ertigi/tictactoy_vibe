using System;
using GameModules.TicTacToe.Api;
using UnityEngine;

namespace GameModules.TicTacToe.Config
{
    /// <summary>
    /// Один возможный элемент случайного дропа.
    /// </summary>
    [Serializable]
    public sealed class TicTacToeRewardDropEntry
    {
        [SerializeField]
        private string dropId = "gem_common";

        [SerializeField]
        private string displayName = "Common Gem";

        [SerializeField]
        private TicTacToeRewardRarity rarity = TicTacToeRewardRarity.Common;

        [SerializeField]
        [Min(0)]
        private int weight = 100;

        public string DropId => dropId ?? string.Empty;

        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? DropId : displayName;

        public TicTacToeRewardRarity Rarity => rarity;

        public int Weight => Math.Max(0, weight);
    }
}
