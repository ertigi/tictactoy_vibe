using System;
using System.Collections.Generic;

namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Хранит состояние клеток и не зависит от Unity API.
    /// </summary>
    public sealed class TicTacToeBoard
    {
        public const int Size = 3;
        public const int CellsCount = Size * Size;

        private readonly BoardMark[] _cells = new BoardMark[CellsCount];

        public int OccupiedCellsCount => CellsCount - EmptyCellsCount;

        public int EmptyCellsCount
        {
            get
            {
                var count = 0;

                for (var i = 0; i < _cells.Length; i++)
                {
                    if (_cells[i] == BoardMark.None)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsFull => EmptyCellsCount == 0;

        public bool IsEmpty => OccupiedCellsCount == 0;

        public BoardMark GetMark(BoardPosition position)
        {
            return IsInside(position) ? _cells[position.Index] : BoardMark.None;
        }

        public bool CanSetMark(BoardPosition position)
        {
            return IsInside(position) && _cells[position.Index] == BoardMark.None;
        }

        public bool IsCellEmpty(BoardPosition position)
        {
            return CanSetMark(position);
        }

        public bool TrySetMark(BoardPosition position, BoardMark mark)
        {
            if (!CanSetMark(position) || mark == BoardMark.None)
            {
                return false;
            }

            _cells[position.Index] = mark;
            return true;
        }

        public void Reset()
        {
            Array.Clear(_cells, 0, _cells.Length);
        }

        public BoardMark[] GetCellsCopy()
        {
            var copy = new BoardMark[_cells.Length];
            Array.Copy(_cells, copy, _cells.Length);
            return copy;
        }

        public TicTacToeBoard Clone()
        {
            var clone = new TicTacToeBoard();
            Array.Copy(_cells, clone._cells, _cells.Length);
            return clone;
        }

        public IReadOnlyList<BoardPosition> GetAvailablePositions()
        {
            var positions = new List<BoardPosition>(CellsCount);

            for (var index = 0; index < _cells.Length; index++)
            {
                if (_cells[index] == BoardMark.None)
                {
                    positions.Add(BoardPosition.FromIndex(index));
                }
            }

            return positions;
        }

        public bool IsInside(BoardPosition position)
        {
            return position.IsValid && position.Index >= 0 && position.Index < _cells.Length;
        }
    }
}
