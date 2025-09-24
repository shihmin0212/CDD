using Newtonsoft.Json;

namespace CDD.Api.Models.Shared
{
    /// <summary>
    /// 預設 回應Ajax格式
    /// </summary>
    public class GeneralResp
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; } = false;

        /// <summary>
        /// 0;-1: No permission 0: Successful 1: No data
        /// </summary>
        public int Code { get; set; } = -1;

        /// <summary>
        /// null
        /// </summary>
        public string? Exception { get; set; } = string.Empty;

        /// <summary>
        /// null
        /// </summary>
        public string? Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 泛型 回應Ajax格式
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    public class GeneralResp<T> : GeneralResp
    {
        /// <summary>
        /// 資料
        /// </summary>
        [JsonProperty("result")]
        public T? Result { get; set; }
    }
}
