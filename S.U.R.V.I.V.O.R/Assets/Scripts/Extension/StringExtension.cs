using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Extension
{
    public static class StringExtension
    {
        
        public static int LevenshteinDistanceTo(this string our, string other)
        {
            var n = our.Length;
            var m = other.Length;
            var d = new int[n + 1][];
            for (int index = 0; index < n + 1; index++)
            {
                d[index] = new int[m + 1];
            }

            if (n == 0) return m;
            
            if (m == 0) return n;

            for (var i = 0; i <= n; d[i][0] = i++) {}
 
            for (var j = 0; j <= m; d[0][j] = j++) {}
            
            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = (other[j - 1] == our[i - 1]) ? 0 : 1;
                    
                    d[i][j] = Math.Min(
                        Math.Min(d[i - 1][j] + 1, d[i][j - 1] + 1),
                        d[i - 1][j - 1] + cost);
                }
            }
            return d[n][m];
        }
        
        /// <summary>
        /// Преобразует hex строку в BitArray
        /// </summary>
        /// <param name="hexData"></param>
        /// <returns></returns>
        public static BitArray ToBitArray(this string hexData)
        {
            var result = new BitArray(4 * hexData.Length);
            for (var i = 0; i < hexData.Length; i++)
            {
                var b = byte.Parse(hexData[i].ToString(), NumberStyles.HexNumber);
                for (var j = 0; j < 4; j++)
                {
                    result.Set(i * 4 + j, (b & (1 << (3 - j))) != 0);
                }
            }
            return result;
        }
        
        /// <summary>
        /// Вставляет символ переноса строки после каждого n символа
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string SplitToLines(this string str, int n)
        {
            return Regex.Replace(str, ".{"+n+"}(?!$)", "$0\n");
        }
    }
}