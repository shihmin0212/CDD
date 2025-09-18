using System.Net.Mail;
using System.Web;
using Newtonsoft.Json;

namespace CDD.Api.Helpers
{
    public class CommonHelper
    {
        /// <summary>
        /// Base 64 轉碼
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncodeBase64(string input)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        ///  Base 64 解碼
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecodeBase64(string input)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static string DecodeBase64Url(string input)
        {
            return DecodeBase64(HttpUtility.UrlDecode(input));
        }


        public static string? SetObject(object? data)
        {
            return (data != null) ? JsonConvert.SerializeObject(data) : null;
        }

        public static T? GetObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 驗證 email format 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidEmailFormat(string value)
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
    }
}
