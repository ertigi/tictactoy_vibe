using System;

namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Значимый тип координаты клетки на поле 3x3.
    /// </summary>
    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        public const int DefaultBoardSize = 3;

        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public int Index => Row * DefaultBoardSize + Column;

        public bool IsValid => Row >= 0 && Row < DefaultBoardSize && Column >= 0 && Column < DefaultBoardSize;

        public static BoardPosition FromIndex(int index)
        {
            if (index < 0 || index >= DefaultBoardSize * DefaultBoardSize)
            {
                return new BoardPosition(-1, -1);
            }

            return new BoardPosition(index / DefaultBoardSize, index % DefaultBoardSize);
        }

        public bool Equals(BoardPosition other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Column;
            }
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}
