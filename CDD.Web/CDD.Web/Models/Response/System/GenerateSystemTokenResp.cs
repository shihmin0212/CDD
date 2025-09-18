using Newtonsoft.Json;

namespace CDD.Web.Models.Response.System
{
    /// <summary>
    /// 產生系統 Token 回應
    /// </summary>
    public class GenerateSystemTokenResp
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [JsonProperty("system")]
        public string System { get; set; } = string.Empty;

        /// <summary>
        /// 加密後的 Token
        /// </summary>
        [JsonProperty("encryptedToken")]
        public string EncryptedToken { get; set; } = string.Empty;

        /// <summary>
        /// 解密後的內容 (僅供測試用)
        /// </summary>
        [JsonProperty("decryptedContent")]
        public string DecryptedContent { get; set; } = string.Empty;

        /// <summary>
        /// Token 建立時間
        /// </summary>
        [JsonProperty("createdTime")]
        public string CreatedTime { get; set; } = string.Empty;
    }
}
