using Newtonsoft.Json;
using CDD.Web.Models.APIResponse;

namespace CDD.Web.Models.Response.System
{
    public class API_SystemViewDataResp : API_GeneralResp
    {
        public SystemViewDataResult Result { get; set; }
    }
    /// <summary>
    /// 系統管理查詢回應
    /// </summary>
    public class SystemViewDataResult
    {
        /// <summary>
        /// 系統資料清單
        /// </summary>
        [JsonProperty("items")]
        public List<SystemDTO> Items { get; set; } = new List<SystemDTO>();

        /// <summary>
        /// 總數量
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// 目前頁碼
        /// </summary>
        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每頁數量
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// 系統資料傳輸物件
    /// </summary>
    public class SystemDTO
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [JsonProperty("system")]
        public string System { get; set; } = string.Empty;

        /// <summary>
        /// API 金鑰
        /// </summary>
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// 雜湊金鑰
        /// </summary>
        [JsonProperty("hashKey")]
        public string HashKey { get; set; } = string.Empty;

        /// <summary>
        /// 初始化向量金鑰
        /// </summary>
        [JsonProperty("iVKey")]
        public string IVKey { get; set; } = string.Empty;

        /// <summary>
        /// 啟用狀態 (Y/N)
        /// </summary>
        [JsonProperty("isActive")]
        public string IsActive { get; set; } = "Y";

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createdTime")]
        public string CreatedTime { get; set; } = string.Empty;

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [JsonProperty("lastUpdate")]
        public string LastUpdate { get; set; } = string.Empty;
    }

}
