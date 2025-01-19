using Chess.Admin.Models;
using Chess.Admin.Services;
using System;

namespace Chess.Admin.Parser
{
    public class FenParser : IParser
    {
        private string? Fen { get; set; }

        private Board? _board;

        /// <summary>
        /// Parsing input string
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public Board? Parse(string fen)
        {
            if (fen == null || fen == String.Empty) { return null; }


            // Remove spaces from start and end of string
            Fen = fen.Trim();

            // if there are spaces in the middle of the line, split string and take the first substring
            Fen = Fen.Split(' ')[0];

            _board = new Board();

            //split string by substring
            var array = Fen.Split("/");

            //must be 8 substrings
            if (CheckRows(array) == false) return null;

            try
            {
                int rowIndex = 0;

                foreach (var row in array)
                {
                    ParseRows(row, rowIndex++);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return _board;
        }

        /// <summary>
        /// parsing substring
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowIndex"></param>
        /// <exception cref="Exception"></exception>
        private void ParseRows(string row, int rowIndex)
        {
            int colIndex = 0;
            foreach (var item in row)
            {
                // char is number (empty cells)
                if (Char.IsNumber(item))
                {
                    var emptyCellsCount = int.Parse(item.ToString());

                    //Number must be from 1 to 8
                    if (emptyCellsCount < 1 || emptyCellsCount > 8) throw new Exception("Invalid empty cells count");

                    colIndex += emptyCellsCount;
                }
                else
                {
                    if (!FillCells(rowIndex, ref colIndex, item)) throw new Exception("Incorrect row format");
                    colIndex++;
                }
            }
            // in string must be symbols for 8 cells, else throw exception
            if (colIndex != 8) throw new Exception("Incorrect row format");
        }

        /// <summary>
        /// board filling. installing figure and its color for cell with current indexies
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool FillCells(int rowIndex, ref int colIndex, char item)
        {
            if (_board == null) return false;
            switch (item)
            {
                case 'r':
                    _board[rowIndex, colIndex].Figure = Figure.Rook;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'n':
                    _board[rowIndex, colIndex].Figure = Figure.Knight;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'b':
                    _board[rowIndex, colIndex].Figure = Figure.Bishop;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'q':
                    _board[rowIndex, colIndex].Figure = Figure.Queen;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'k':
                    _board[rowIndex, colIndex].Figure = Figure.King;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'p':
                    _board[rowIndex, colIndex].Figure = Figure.Pawn;

                    _board[rowIndex, colIndex].Color = FigureColor.Black;

                    return true;
                case 'R':
                    _board[rowIndex, colIndex].Figure = Figure.Rook;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                case 'N':
                    _board[rowIndex, colIndex].Figure = Figure.Knight;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                case 'B':
                    _board[rowIndex, colIndex].Figure = Figure.Bishop;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                case 'K':
                    _board[rowIndex, colIndex].Figure = Figure.King;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                case 'Q':
                    _board[rowIndex, colIndex].Figure = Figure.Queen;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                case 'P':
                    _board[rowIndex, colIndex].Figure = Figure.Pawn;

                    _board[rowIndex, colIndex].Color = FigureColor.White;

                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check count of substrings (must be 8)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static bool CheckRows(string[] array)
        {
            return array.Length == 8;
        }
    }
}
