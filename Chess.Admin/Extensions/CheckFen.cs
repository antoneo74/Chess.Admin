using System;

namespace Chess.Admin.Extensions
{
    public static class CheckFen
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
    }
}
