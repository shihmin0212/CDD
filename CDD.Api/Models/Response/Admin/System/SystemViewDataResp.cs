using Newtonsoft.Json;
using CDD.Api.Repositories.DTO.Admin;

namespace CDD.Api.Models.Response.Admin.System
{
    public class SystemViewDataResp
    {
        /// <summary>
        /// 系統資料清單
        /// </summary>
        [JsonProperty("items")]
        public List<SystemAdminDTO>? Items { get; set; } = new List<SystemAdminDTO>();

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
}
