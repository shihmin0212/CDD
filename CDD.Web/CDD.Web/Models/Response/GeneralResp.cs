using Newtonsoft.Json;

namespace CDD.Web.Models.Response
{
    /// <summary>
    /// 預設 回應Ajax格式
    /// </summary>
    public class GeneralResp
    {

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("errorCode")]
        public string? ErrorCode { get; set; }
    }

    /// <summary>
    /// 泛型 回應Ajax格式
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    public class GeneralResp<T> : GeneralResp
    {
        [JsonProperty("result")]
        public T? Result { get; set; }
    }
}
