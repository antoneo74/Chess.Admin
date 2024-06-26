using Chess.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Admin.Extensions
{
    public static class Extensions
    {
        public static bool IsCorrect(this string fen)
        {
            string figure = "rnbqkpRNBQKP";

            //split string by substring
            var array = fen.Split("/");

            //must be 8 substrings
            if (array.Length != 8) return false;

            foreach (var row in array)
            {
                int count = 0;
                foreach (var c in row)
                {
                    if (!Char.IsDigit(c) && !figure.Contains(c)) return false;

                    if (Char.IsDigit(c)) count += c - '0';

                    else count++;
                }
                if (count != 8) return false;
            }
            return true;
        }

        /// <summary>
        /// Remove spaces from start and end of string
        /// </summary>
        /// <param name="fen"></param>
        /// <returns>if there is space into string return first part of this string</returns>
        public static string Normalization(this string fen)
        {
            fen = fen.Trim();

            return fen.Split(' ')[0];
        }

        private static readonly Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int GetWeaknessBlack(this Board board)
        {
            return board.BoardToList()
                .Where(x => x.Color == FigureColor.Black
                            && x.ProtectionsCount <= x.EnemyAttacksCount
                            && x.Figure != Figure.King)
                .Count();
        }

        public static int GetWeaknessWhite(this Board board)
        {
            return board.BoardToList()
                .Where(x => x.Color == FigureColor.White
                            && x.ProtectionsCount <= x.EnemyAttacksCount
                            && x.Figure != Figure.King)
                .Count();
        }

        public static int GetBlackAttacks(this Board board)
        {
            return board.BoardToList()
                .Where(x => x.Color == FigureColor.White)
                .Sum(x => x.Capture);
        }

        public static int GetWhiteAttacks(this Board board)
        {
            return board.BoardToList()
                .Where(x => x.Color == FigureColor.Black)
                .Sum(x => x.Capture);
        }
    }
}
