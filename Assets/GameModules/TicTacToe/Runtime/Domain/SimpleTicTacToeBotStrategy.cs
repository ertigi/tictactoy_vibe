namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Простая приоритетная стратегия:
    /// сначала добивает победу, затем блокирует игрока,
    /// потом берет центр, углы и только после этого стороны.
    /// </summary>
    public sealed class SimpleTicTacToeBotStrategy : ITicTacToeBotStrategy
    {
        private static readonly BoardPosition Center = new(1, 1);

        private static readonly BoardPosition[] CornerPositions =
        {
            new(0, 0),
            new(0, 2),
            new(2, 0),
            new(2, 2)
        };

        private static readonly BoardPosition[] SidePositions =
        {
            new(0, 1),
            new(1, 0),
            new(1, 2),
            new(2, 1)
        };

        private readonly TicTacToeRules _rules;

        public SimpleTicTacToeBotStrategy(TicTacToeRules rules)
        {
            _rules = rules;
        }

        public bool TryChooseMove(
            TicTacToeBoard board,
            BoardMark botMark,
            BoardMark playerMark,
            out BoardPosition position)
        {
            if (_rules.TryFindFinishingMove(board, botMark, out position))
            {
                return true;
            }

            if (_rules.TryFindFinishingMove(board, playerMark, out position))
            {
                return true;
            }

            if (TryChoosePreferredPosition(board, Center, out position))
            {
                return true;
            }

            if (TryChoosePreferredPosition(board, CornerPositions, out position))
            {
                return true;
            }

            if (TryChoosePreferredPosition(board, SidePositions, out position))
            {
                return true;
            }

            var availablePositions = board.GetAvailablePositions();

            if (availablePositions.Count > 0)
            {
                position = availablePositions[0];
                return true;
            }

            position = default;
            return false;
        }

        private static bool TryChoosePreferredPosition(
            TicTacToeBoard board,
            BoardPosition preferredPosition,
            out BoardPosition position)
        {
            if (board.CanSetMark(preferredPosition))
            {
                position = preferredPosition;
                return true;
            }

            position = default;
            return false;
        }

        private static bool TryChoosePreferredPosition(
            TicTacToeBoard board,
            BoardPosition[] preferredPositions,
            out BoardPosition position)
        {
            for (var i = 0; i < preferredPositions.Length; i++)
            {
                if (board.CanSetMark(preferredPositions[i]))
                {
                    position = preferredPositions[i];
                    return true;
                }
            }

            position = default;
            return false;
        }
    }
}
