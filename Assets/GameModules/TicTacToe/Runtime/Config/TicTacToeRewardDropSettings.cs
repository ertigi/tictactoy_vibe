using System;
using System.Collections.Generic;
using GameModules.TicTacToe.Domain;
using UnityEngine;

namespace GameModules.TicTacToe.Config
{
    /// <summary>
    /// Настройки демонстрационного случайного дропа.
    /// </summary>
    [Serializable]
    public sealed class TicTacToeRewardDropSettings
    {
        [SerializeField]
        private bool enabled = true;

        [SerializeField]
        [Range(0f, 1f)]
        private float winDropChance = 0.75f;

        [SerializeField]
        [Range(0f, 1f)]
        private float drawDropChance = 0.25f;

        [SerializeField]
        [Range(0f, 1f)]
        private float loseDropChance = 0.05f;

        [SerializeField]
        private TicTacToeRewardDropEntry[] entries = Array.Empty<TicTacToeRewardDropEntry>();

        public bool Enabled => enabled;

        public IReadOnlyList<TicTacToeRewardDropEntry> Entries => entries ?? Array.Empty<TicTacToeRewardDropEntry>();

        public float GetDropChance(TicTacToeRoundOutcome outcome)
        {
            switch (outcome)
            {
                case TicTacToeRoundOutcome.PlayerWin:
                    return winDropChance;
                case TicTacToeRoundOutcome.Draw:
                    return drawDropChance;
                case TicTacToeRoundOutcome.BotWin:
                    return loseDropChance;
                default:
                    return 0f;
            }
        }
    }
}
