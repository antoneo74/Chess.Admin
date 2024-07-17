using Avalonia.Media;
using System.Collections.Generic;

namespace Chess.Admin.Models
{
    public class Board
    {
        private readonly Cell[,] _cells;

        public Cell this[int row, int column]
        {
            get => _cells[row, column];
            set => _cells[row, column] = value;
        }

        public Board()
        {
            _cells = new Cell[8, 8];
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j] = new Cell();
                    // even rows
                    if (i % 2 == 0)
                    {
                        _cells[i, j].BackgroundColor = new SolidColorBrush(j % 2 == 0 ? Color.Parse("#f0d9b5") : Color.Parse("#b58863"));
                    }
                    // uneven rows
                    else
                    {
                        _cells[i, j].BackgroundColor = new SolidColorBrush(j % 2 == 0 ? Color.Parse("#b58863") : Color.Parse("#f0d9b5"));
                    }
                }
            }
        }

        public List<Cell> BoardToList()
        {
            List<Cell> list = [];
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    list.Add(_cells[i, j]);
                }
            }
            return list;
        }
    }
}
