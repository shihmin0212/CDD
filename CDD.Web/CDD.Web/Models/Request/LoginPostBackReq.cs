using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CDD.Web.Models.Request
{
    /// <summary>
    /// Hybrid 加密登入資料
    /// </summary>
    public class LoginPostBackReq
    {
        [Required(ErrorMessage = "登入 AesEncryptedData為必填")]
        [JsonProperty(PropertyName = "aesEncryptedData")]
        public string AesEncryptedData { get; set; }

        [Required(ErrorMessage = "AES key 為必填")]
        [JsonProperty(PropertyName = "encryptedAesKey")]
        public string EncryptedAesKey { get; set; }


        /// <summary>
        /// 將 IV 轉為 Base64 傳給後端
        /// </summary>
        [Required(ErrorMessage = "base64Iv 為必填")]
        [JsonProperty(PropertyName = "base64Iv")]
        public string Base64Iv { get; set; } // Base64 格式的 IV

        [Required(ErrorMessage = "驗證碼 為必填")]
        public string CaptchaCode { get; set; } = string.Empty;

    }

    public class LoginData
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
