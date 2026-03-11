using System;
using System.Collections.Generic;

namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Снимок оценки поля после хода.
    /// </summary>
    public sealed class TicTacToeBoardEvaluation
    {
        public static readonly TicTacToeBoardEvaluation InProgress =
            new(TicTacToeRoundOutcome.None, Array.Empty<BoardPosition>());

        public TicTacToeBoardEvaluation(
            TicTacToeRoundOutcome outcome,
            IReadOnlyList<BoardPosition> winningPositions)
        {
            Outcome = outcome;
            WinningPositions = winningPositions ?? Array.Empty<BoardPosition>();
        }

        public TicTacToeRoundOutcome Outcome { get; }

        public IReadOnlyList<BoardPosition> WinningPositions { get; }

        public bool IsFinished => Outcome != TicTacToeRoundOutcome.None;
    }
}
