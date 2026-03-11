namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Проверяет победные комбинации и состояние ничьей.
    /// </summary>
    public sealed class TicTacToeRules
    {
        private static readonly BoardPosition[][] WinningLines =
        {
            new[] { new BoardPosition(0, 0), new BoardPosition(0, 1), new BoardPosition(0, 2) },
            new[] { new BoardPosition(1, 0), new BoardPosition(1, 1), new BoardPosition(1, 2) },
            new[] { new BoardPosition(2, 0), new BoardPosition(2, 1), new BoardPosition(2, 2) },
            new[] { new BoardPosition(0, 0), new BoardPosition(1, 0), new BoardPosition(2, 0) },
            new[] { new BoardPosition(0, 1), new BoardPosition(1, 1), new BoardPosition(2, 1) },
            new[] { new BoardPosition(0, 2), new BoardPosition(1, 2), new BoardPosition(2, 2) },
            new[] { new BoardPosition(0, 0), new BoardPosition(1, 1), new BoardPosition(2, 2) },
            new[] { new BoardPosition(0, 2), new BoardPosition(1, 1), new BoardPosition(2, 0) }
        };

        public TicTacToeBoardEvaluation Evaluate(TicTacToeBoard board)
        {
            for (var i = 0; i < WinningLines.Length; i++)
            {
                var line = WinningLines[i];
                var firstMark = board.GetMark(line[0]);

                if (firstMark == BoardMark.None)
                {
                    continue;
                }

                if (board.GetMark(line[1]) == firstMark && board.GetMark(line[2]) == firstMark)
                {
                    return new TicTacToeBoardEvaluation(ToOutcome(firstMark), line);
                }
            }

            return board.IsFull
                ? new TicTacToeBoardEvaluation(TicTacToeRoundOutcome.Draw, System.Array.Empty<BoardPosition>())
                : TicTacToeBoardEvaluation.InProgress;
        }

        public bool TryFindFinishingMove(
            TicTacToeBoard board,
            BoardMark mark,
            out BoardPosition position)
        {
            var availablePositions = board.GetAvailablePositions();
            var targetOutcome = ToOutcome(mark);

            for (var i = 0; i < availablePositions.Count; i++)
            {
                var candidate = availablePositions[i];
                var boardCopy = board.Clone();

                if (!boardCopy.TrySetMark(candidate, mark))
                {
                    continue;
                }

                var evaluation = Evaluate(boardCopy);

                if (evaluation.Outcome == targetOutcome)
                {
                    position = candidate;
                    return true;
                }
            }

            position = default;
            return false;
        }

        private static TicTacToeRoundOutcome ToOutcome(BoardMark mark)
        {
            switch (mark)
            {
                case BoardMark.Player:
                    return TicTacToeRoundOutcome.PlayerWin;
                case BoardMark.Bot:
                    return TicTacToeRoundOutcome.BotWin;
                default:
                    return TicTacToeRoundOutcome.None;
            }
        }
    }
}
