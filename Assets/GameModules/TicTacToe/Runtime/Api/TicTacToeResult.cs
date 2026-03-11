using System;
using System.Collections.Generic;
using GameModules.TicTacToe.Domain;

namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Результат завершенной игровой сессии.
    /// </summary>
    public sealed class TicTacToeResult
    {
        public TicTacToeResult(
            string sessionId,
            TicTacToeOutcome outcome,
            TicTacToeRewardData reward,
            int turnsCount,
            IReadOnlyList<BoardPosition> winningCells,
            bool wasCancelled)
        {
            SessionId = sessionId ?? string.Empty;
            Outcome = outcome;
            Reward = reward ?? TicTacToeRewardData.Empty;
            TurnsCount = Math.Max(0, turnsCount);
            WinningCells = winningCells ?? Array.Empty<BoardPosition>();
            WasCancelled = wasCancelled;
        }

        public string SessionId { get; }

        public TicTacToeOutcome Outcome { get; }

        public TicTacToeRewardData Reward { get; }

        public int TurnsCount { get; }

        public IReadOnlyList<BoardPosition> WinningCells { get; }

        public bool WasCancelled { get; }
    }
}
