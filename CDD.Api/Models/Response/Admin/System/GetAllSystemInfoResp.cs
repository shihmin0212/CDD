using Newtonsoft.Json;
using CDD.Api.Repositories.DTO;

namespace CDD.Api.Models.Response.Admin.System
{
    public class GetAllSystemInfoResp
    {
        /// <summary>
        /// 系統資料清單
        /// </summary>
        [JsonProperty("items")]
        public List<SystemDTO>? Items { get; set; } = new List<SystemDTO>();
    }
}
