using System.Net;
using Newtonsoft.Json;

namespace CDD.Web.Models.APIResponse
{
    /// <summary>
    /// API Resp
    /// </summary>
    public class API_GeneralResp : IStatusCodeResp
    {
        public HttpStatusCode ResponseStatusCode { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// 0;-1: No permission 0: Successful 1: No data
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; } = -1;

        /// <summary>
        /// null
        /// </summary>
        [JsonProperty("exception")]
        public string? Exception { get; set; } = String.Empty;

        [JsonProperty("message")]
        public string? Message { get; set; }
    }

    /// <summary>
    /// 泛型 API Resp
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    public class API_GeneralResp<T> : API_GeneralResp
    {
        [JsonProperty("result")]
        public T? Result { get; set; }
    }
}
