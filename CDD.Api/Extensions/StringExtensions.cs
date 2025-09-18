using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CDD.Api.Extensions
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

        /// <summary>
        /// 字串 轉 駝峰
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lowerCamelCase"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string input, bool lowerCamelCase = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // 切分單詞並去除空格
            string[] words = input.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                return string.Empty;
            }

            // 使用 StringBuilder 優化字串拼接
            var stringBuilder = new StringBuilder();

            // 處理第一個單詞
            string firstWord = lowerCamelCase ? words[0].ToLower(CultureInfo.InvariantCulture) : words[0];
            stringBuilder.Append(firstWord);

            // 處理其餘單詞
            for (int i = 1; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    stringBuilder.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(words[i].ToLower(CultureInfo.InvariantCulture)));
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 是否為合法json format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool IsValidJson<T>(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) return false;

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<T>(strInput);
                    return true;
                }
                catch // not valid
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否為合法json format
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidJson(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        /// <summary>
        /// 是否為合法email
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidEmailFormat(this string value)
        {
            try
            {
                MailAddress m = new MailAddress(value);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        /// <summary>
        /// 取得交集，過濾出在名單內的字元
        /// </summary>
        /// <param name="validArray"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] FilterValidCharacters(this string[] validArray, string input)
        {
            // 將輸入字串依逗號分割，並去除空格
            string[] inputChars = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // 取得交集，過濾出在名單內的字元
            return inputChars.Intersect(validArray).ToArray();
        }


        /// <summary>
        /// 檢查 base64 string 是否超過 XX mb
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="maxMB"></param>
        /// <returns></returns>
        public static bool IsBase64StringOverSizeLimit(this string base64String, int maxMB)
        {
            // 將 MB 轉換為位元組數 (1 MB = 1048576 bytes)
            int maxBytes = maxMB * 1048576;

            // Base64 字串長度大約為實際位元組數的 4/3，先去掉填充符號
            int padding = 0;
            if (base64String.EndsWith("=="))
            {
                padding = 2;
            }
            else if (base64String.EndsWith("="))
            {
                padding = 1;
            }

            // 計算 Base64 字串轉換為位元組後的大小
            int byteLength = (base64String.Length * 3) / 4 - padding;

            // 檢查是否超過指定的大小
            return byteLength > maxBytes;
        }
    }
}
