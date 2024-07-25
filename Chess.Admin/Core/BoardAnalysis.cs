using Chess.Admin.Models;
using Chess.Admin.Services;
namespace Chess.Core
{
    public class BoardAnalysis() : IAnalysis
    {
        private Board _board = new();

        public Board Analysis(Board board)
        {
            _board = board;

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    switch (_board[row, column].Figure)
                    {
                        case Figure.Pawn:
                            PawnAnalysis(row, column);
                            break;

                        case Figure.Queen:
                            QueenAnalysis(row, column);
                            break;

                        case Figure.King:
                            KingAnalysis(row, column);
                            break;

                        case Figure.Knight:
                            KnightAnalysis(row, column);
                            break;

                        case Figure.Bishop:
                            BishopAnalysis(row, column);
                            break;

                        case Figure.Rook:
                            RookAnalysis(row, column);
                            break;

                        default:
                            break;
                    }
                }
            }
            return _board;
        }

        private void RookAnalysis(int row, int column)
        {
            // Cells that the rook can attack or defend 
            // [(0...row-1), column] [(row+1 ... 7), column]
            // [row, (0 ... column-1)] [row, (column+1 ... 7)]

            var color = _board[row, column].Color;

            var iterator = row;

            bool isCanCapture = true;

            //go to top
            while (CellCorrect(--iterator, column))
            {
                if (!CheckigVerticalHorizontalCells(iterator, column, ref color, ref isCanCapture)) break;
            }

            iterator = row;

            isCanCapture = true;

            //go to down
            while (CellCorrect(++iterator, column))
            {
                if (!CheckigVerticalHorizontalCells(iterator, column, ref color, ref isCanCapture)) break;
            }

            iterator = column;

            isCanCapture = true;

            //go to left
            while (CellCorrect(row, --iterator))
            {
                if (!CheckigVerticalHorizontalCells(row, iterator, ref color, ref isCanCapture)) break;
            }

            iterator = column;

            isCanCapture = true;

            //go to right
            while (CellCorrect(row, ++iterator))
            {
                if (!CheckigVerticalHorizontalCells(row, iterator, ref color, ref isCanCapture)) break;
            }
        }

        private void BishopAnalysis(int row, int column)
        {
            // Cells that the bishop can attack or defend (while correct indexies)
            // [--row, --column] [--row, ++column] [++row, --column] [++row, ++column]

            // We go diagonally until we meet a figure and analyze the attacks/defenses.
            // If the piece is the same color and can also attack or defend diagonally, move on.
            // All others cases break

            var color = _board[row, column].Color;

            var rowIterator = row;

            var columnIterator = column;

            bool isCanCapture = true;

            //go to top and left
            while (CellCorrect(--rowIterator, --columnIterator))
            {
                // cell is empty - move on
                if (!CellIsNotEmpty(rowIterator, columnIterator)) continue;

                // else we analyze attacks/defends
                AttacksDefends(rowIterator, columnIterator, color, isCanCapture);

                // if there was an attack or defense of a piece that is not diagonal, we stop the movement since rentgen is impossible
                if (ColorIsDifferent(color, rowIterator, columnIterator) || FigureIsNotDiagonal(rowIterator, columnIterator)) { break; }

                // rentgen continues but capturing the enemy piece with the bishop is no longer possible
                isCanCapture = false;

                // we check the situation when there is a pawn on the diagonal
                if (_board[rowIterator, columnIterator].Figure == Figure.Pawn)
                {
                    // A pawn can attack or defend in this direction only if it is white
                    if (color == FigureColor.White) PawnIsDiagonalCheckingNextCell(rowIterator - 1, columnIterator - 1, color);

                    break;
                }
            }

            rowIterator = row;

            columnIterator = column;

            isCanCapture = true;

            //go to top and right
            while (CellCorrect(--rowIterator, ++columnIterator))
            {
                if (!CellIsNotEmpty(rowIterator, columnIterator)) continue;

                AttacksDefends(rowIterator, columnIterator, color, isCanCapture);

                if (ColorIsDifferent(color, rowIterator, columnIterator) || FigureIsNotDiagonal(rowIterator, columnIterator)) { break; }

                isCanCapture = false;

                if (_board[rowIterator, columnIterator].Figure == Figure.Pawn)
                {
                    // A pawn can attack or defend in this direction only if it is white
                    if (color == FigureColor.White) PawnIsDiagonalCheckingNextCell(rowIterator - 1, columnIterator + 1, color);

                    break;
                }
            }

            rowIterator = row;

            columnIterator = column;

            isCanCapture = true;

            //go to down and left

            while (CellCorrect(++rowIterator, --columnIterator))
            {
                if (!CellIsNotEmpty(rowIterator, columnIterator)) continue;

                AttacksDefends(rowIterator, columnIterator, color, isCanCapture);

                if (ColorIsDifferent(color, rowIterator, columnIterator) || FigureIsNotDiagonal(rowIterator, columnIterator)) { break; }

                isCanCapture = false;

                if (_board[rowIterator, columnIterator].Figure == Figure.Pawn)
                {
                    // A pawn can attack or defend in this direction only if it is black
                    if (color == FigureColor.Black) PawnIsDiagonalCheckingNextCell(rowIterator + 1, columnIterator - 1, color);

                    break;
                }
            }

            rowIterator = row;

            columnIterator = column;

            isCanCapture = true;

            //go to down and right
            while (CellCorrect(++rowIterator, ++columnIterator))
            {
                if (!CellIsNotEmpty(rowIterator, columnIterator)) continue;

                AttacksDefends(rowIterator, columnIterator, color, isCanCapture);

                if (ColorIsDifferent(color, rowIterator, columnIterator) || FigureIsNotDiagonal(rowIterator, columnIterator)) { break; }

                isCanCapture = false;

                if (_board[rowIterator, columnIterator].Figure == Figure.Pawn)
                {
                    // A pawn can attack or defend in this direction only if it is black
                    if (color == FigureColor.Black) PawnIsDiagonalCheckingNextCell(rowIterator + 1, columnIterator + 1, color);

                    break;
                }
            }
        }

        private void QueenAnalysis(int row, int column)
        {
            // Queen is combination rook and bishop 

            RookAnalysis(row, column);

            BishopAnalysis(row, column);
        }

        private void KnightAnalysis(int row, int column)
        {
            // Cells that the knight(horse) can attack or defend
            // [row - 1, column - 2] [row - 1, column + 2] [row - 2, column + 1] [row - 2, column - 1]
            // [row + 1, column - 2] [row + 1, column + 2] [row + 2, column + 1] [row + 2, column - 1]

            var color = _board[row, column].Color;

            if (CellCorrectAndIsNotEmpty(row - 1, column - 2)) AttacksDefends(row - 1, column - 2, color);

            if (CellCorrectAndIsNotEmpty(row - 1, column + 2)) AttacksDefends(row - 1, column + 2, color);

            if (CellCorrectAndIsNotEmpty(row - 2, column + 1)) AttacksDefends(row - 2, column + 1, color);

            if (CellCorrectAndIsNotEmpty(row - 2, column - 1)) AttacksDefends(row - 2, column - 1, color);

            if (CellCorrectAndIsNotEmpty(row + 1, column - 2)) AttacksDefends(row + 1, column - 2, color);

            if (CellCorrectAndIsNotEmpty(row + 1, column + 2)) AttacksDefends(row + 1, column + 2, color);

            if (CellCorrectAndIsNotEmpty(row + 2, column + 1)) AttacksDefends(row + 2, column + 1, color);

            if (CellCorrectAndIsNotEmpty(row + 2, column - 1)) AttacksDefends(row + 2, column - 1, color);
        }

        private void KingAnalysis(int row, int column)
        {
            // Cells that the king can attack or defend
            // [row - 1, column - 1] [row - 1, column] [row - 1, column + 1] [row, column - 1]
            // [row + 1, column - 1] [row + 1, column] [row + 1, column + 1] [row, column + 1]

            var color = _board[row, column].Color;

            if (CellCorrectAndIsNotEmpty(row - 1, column - 1)) AttacksDefends(row - 1, column - 1, color);

            if (CellCorrectAndIsNotEmpty(row - 1, column)) AttacksDefends(row - 1, column, color);

            if (CellCorrectAndIsNotEmpty(row - 1, column + 1)) AttacksDefends(row - 1, column + 1, color);

            if (CellCorrectAndIsNotEmpty(row, column - 1)) AttacksDefends(row, column - 1, color);

            if (CellCorrectAndIsNotEmpty(row + 1, column - 1)) AttacksDefends(row + 1, column - 1, color);

            if (CellCorrectAndIsNotEmpty(row + 1, column)) AttacksDefends(row + 1, column, color);

            if (CellCorrectAndIsNotEmpty(row + 1, column + 1)) AttacksDefends(row + 1, column + 1, color);

            if (CellCorrectAndIsNotEmpty(row, column + 1)) AttacksDefends(row, column + 1, color);
        }

        private void PawnAnalysis(int row, int column)
        {
            // attacks and defends can be only [row-1, column-1] and [row-1, column+1] for white pawn
            // and [row+1, column-1] and [row+1, column+1] for black pawn

            // Pawn is white
            if (_board[row, column].Color == FigureColor.White)
            {
                if (CellCorrectAndIsNotEmpty(row - 1, column - 1))
                {
                    AttacksDefends(row - 1, column - 1, FigureColor.White);
                }

                if (CellCorrectAndIsNotEmpty(row - 1, column + 1))
                {
                    AttacksDefends(row - 1, column + 1, FigureColor.White);
                }
            }
            // Pawn is black
            else
            {
                if (CellCorrectAndIsNotEmpty(row + 1, column - 1))
                {
                    AttacksDefends(row + 1, column - 1, FigureColor.Black);
                }

                if (CellCorrectAndIsNotEmpty(row + 1, column + 1))
                {
                    AttacksDefends(row + 1, column + 1, FigureColor.Black);
                }
            }
        }

        /// <summary>
        /// increment of count defends or enemy attacks for cell with indexies[row, column]
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void AttacksDefends(int row, int column, FigureColor color, bool isCanCapture = true)
        /// <param name="color"></param>
        {
            // figure is king
            if (_board[row, column].Figure == Figure.King) { return; }

            // if color is the same - increment defends, else increment enemy attacks
            if (_board[row, column].Color == color)
            {
                _board[row, column].ProtectionsCount++;
            }
            else
            {
                if (isCanCapture) _board[row, column].Capture++;

                _board[row, column].EnemyAttacksCount++;
            }
        }

        /// <summary>
        /// if cell has correct indexies and not empty return true, else - false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool CellCorrectAndIsNotEmpty(int row, int column)
        {
            return row >= 0 && row < 8 && column >= 0 && column < 8 && _board[row, column].Figure != Figure.Empty;
        }

        /// <summary>
        /// if cell has correct indexies return true, else - false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool CellCorrect(int row, int column)
        {
            return row >= 0 && row < 8 && column >= 0 && column < 8; ;
        }

        /// <summary>
        /// if cell is not empty return true, else - false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool CellIsNotEmpty(int row, int column)
        {
            return _board[row, column].Figure != Figure.Empty;
        }

        /// <summary>
        /// Check color in current cell and return true if color the cell and color param are different
        /// </summary>
        /// <param name="color"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool ColorIsDifferent(FigureColor color, int row, int column)
        {
            return _board[row, column].Color != color;
        }


        /// <summary>
        /// Checking whether figure is not diagonal (queen, bishop or pawn)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool FigureIsNotDiagonal(int row, int column)
        {
            return _board[row, column].Figure != Figure.Bishop
                            && _board[row, column].Figure != Figure.Queen
                            && _board[row, column].Figure != Figure.Pawn;
        }

        /// <summary>
        /// Checking whether figure is not horizontal/vertical (queen, rook)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool FigureIsNotHorizontalVertical(int row, int column)
        {
            return _board[row, column].Figure != Figure.Rook
                            && _board[row, column].Figure != Figure.Queen;
        }

        /// <summary>
        /// Checking next cell when pawn stay after other diagonal figure
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        private void PawnIsDiagonalCheckingNextCell(int row, int column, FigureColor color)
        {
            if (!CellCorrect(row, column) || !CellIsNotEmpty(row, column)) return;

            AttacksDefends(row, column, color, false);
        }


        /// <summary>
        /// Checking whether cell is horizontal/vertical figure
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        /// <param name="isCanCapture"></param>
        /// <returns></returns>
        private bool CheckigVerticalHorizontalCells(int row, int column, ref FigureColor color, ref bool isCanCapture)
        {
            if (CellIsNotEmpty(row, column))
            {
                AttacksDefends(row, column, color, isCanCapture);

                if (ColorIsDifferent(color, row, column)) { return false; }

                else
                {
                    if (FigureIsNotHorizontalVertical(row, column)) { return false; }

                    isCanCapture = false;
                }
            }
            return true;
        }
    }
}
