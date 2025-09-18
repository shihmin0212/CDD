using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CDD.Web.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 取代所有　空白
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAllWhitespaces(this string str)
        {
            return str.ConvertWhitespacesToSingleSpaces().Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }


        /// <summary>
        /// 擴充方法　全形轉半形
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToHalfwidthString(this string str)
        {
            str = str.TrimAllWhitespaces();
            char[] c = str.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                // c[i] == 12288 全形空格為12288，半形空格為32
                if (c[i] > 65280 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }

        public static T? GetObject<T>(this string str)
        {
            string? value = null;
            return value == null ? default : JsonConvert.DeserializeObject<T>(str);
        }

        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
