using Newtonsoft.Json;
using CDD.Web.Models.DTO;

namespace CDD.Web.Models.Response
{
    /// <summary>
    /// 取得 停登公告
    /// </summary>
    public class GetServiceAlertResp : GeneralResp
    {
        /// <summary>
        /// XSRF token
        /// </summary>
        [JsonProperty("requestToken")]
        public string? RequestToken { get; set; } 

        /// <summary>
        /// 逾時提醒時間
        /// </summary>
        [JsonProperty("idleTimeoutHintMinutes")]
        public int IdleTimeoutHintMinutes { get; set; }

    }
}
