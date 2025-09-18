using Newtonsoft.Json;
using CDD.Web.Models.APIResponse;

namespace CDD.Web.Models.Response.System
{
    public class API_GetAllSystemInfoResp : API_GeneralResp
    {
        public GetAllSystemInfoResult Result { get; set; }
    }
    /// <summary>
    /// 系統管理查詢回應
    /// </summary>
    public class GetAllSystemInfoResult
    {
        /// <summary>
        /// 系統資料清單
        /// </summary>
        [JsonProperty("items")]
        public List<SystemDTO> Items { get; set; } = new List<SystemDTO>();
    }
}
