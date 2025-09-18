using Newtonsoft.Json;

namespace CDD.Web.Models.Response.Login
{
    /// <summary>
    /// 登入頁預設顯示資料
    /// </summary>
    public class LoginViewDataResp : GeneralResp
    {
        /// <summary>
        /// rsaPublicKey
        /// </summary>
        [JsonProperty("rsaPublicKey")]
        public string? rsaPublicKey { get; set; }

        [JsonProperty("captchaCodeBase64Src")]
        public string CaptchaCodeBase64Src { get; set; }
    }
}
