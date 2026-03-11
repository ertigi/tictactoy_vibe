using System;
using System.Collections.Generic;
using GameModules.TicTacToe.Domain;

namespace GameModules.TicTacToe.Application
{
    /// <summary>
    /// Иммутабельный снимок состояния сессии для UI.
    /// </summary>
    public sealed class TicTacToeSessionSnapshot
    {
        public static readonly TicTacToeSessionSnapshot Empty =
            new(Array.Empty<BoardMark>(), BoardMark.None, false, false, TicTacToeRoundOutcome.None, Array.Empty<BoardPosition>(), 0);

        public TicTacToeSessionSnapshot(
            IReadOnlyList<BoardMark> cells,
            BoardMark currentTurn,
            bool isInputLocked,
            bool isFinished,
            TicTacToeRoundOutcome outcome,
            IReadOnlyList<BoardPosition> winningCells,
            int turnsCount)
        {
            Cells = cells ?? Array.Empty<BoardMark>();
            CurrentTurn = currentTurn;
            IsInputLocked = isInputLocked;
            IsFinished = isFinished;
            Outcome = outcome;
            WinningCells = winningCells ?? Array.Empty<BoardPosition>();
            TurnsCount = Math.Max(0, turnsCount);
        }

        public IReadOnlyList<BoardMark> Cells { get; }

        public BoardMark CurrentTurn { get; }

        public bool IsInputLocked { get; }

        public bool IsFinished { get; }

        public TicTacToeRoundOutcome Outcome { get; }

        public IReadOnlyList<BoardPosition> WinningCells { get; }

        public int TurnsCount { get; }
    }
}
