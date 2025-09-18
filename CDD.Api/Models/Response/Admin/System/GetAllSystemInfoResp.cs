using Newtonsoft.Json;
using Sample.Api.Repositories.DTO;

namespace Sample.Api.Models.Response.Admin.System
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
