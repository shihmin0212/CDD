using Newtonsoft.Json;

namespace CDD.API.Models.Response
{
    /// <summary>
    /// FSA-011 GetMemberInfo
    /// </summary>
    public class GetMemberInfoResp
    {
        /// <summary>員工資訊</summary>
        [JsonProperty("memberInfo")]
        public MemberInfo? MemberInfo { get; set; }

        /// <summary>單位資料清單</summary>
        [JsonProperty("UnitDataList")]
        public List<UnitData>? UnitDataList { get; set; }

        /// <summary>是否連接成功</summary>
        [JsonProperty("IsSuccess")]
        public bool IsSuccess { get; set; }

        /// <summary>驗證/訊息</summary>
        [JsonProperty("ValidationMsg")]
        public List<string>? ValidationMsg { get; set; }
    }

    /// <summary>
    /// 員工資訊
    /// </summary>
    public class MemberInfo
    {
        /// <summary>員編</summary>
        [JsonProperty("EmpID")]
        public string? EmpID { get; set; }

        /// <summary>員工姓名</summary>
        [JsonProperty("EmpName")]
        public string? EmpName { get; set; }

        /// <summary>主要單位編號</summary>
        [JsonProperty("PrimaryUnitID")]
        public string? PrimaryUnitID { get; set; }

        /// <summary>主要單位名稱</summary>
        [JsonProperty("PrimaryUnitName")]
        public string? PrimaryUnitName { get; set; }

        /// <summary>主要角色編號</summary>
        [JsonProperty("PrimaryRole")]
        public string? PrimaryRole { get; set; }

        /// <summary>主要角色名稱</summary>
        [JsonProperty("PrimaryRoleName")]
        public string? PrimaryRoleName { get; set; }

        /// <summary>組織樹編號（逗點分隔）</summary>
        [JsonProperty("UnitTreeID")]
        public string? UnitTreeID { get; set; }

        /// <summary>組織樹名稱（逗點分隔）</summary>
        [JsonProperty("UnitTreeName")]
        public string? UnitTreeName { get; set; }

        /// <summary>組織樹階層（例：L20,L30,...）</summary>
        [JsonProperty("UnitTreeLevel")]
        public string? UnitTreeLevel { get; set; }

        /// <summary>組織樹階層（數字型態字串，例：2,3,4 或 20,30,40）</summary>
        [JsonProperty("TransTreeLevel")]
        public string? TransTreeLevel { get; set; }

        /// <summary>工作區域</summary>
        [JsonProperty("WorkZone")]
        public string? WorkZone { get; set; }

        /// <summary>扮演角色對照（roleCode → roleName）</summary>
        [JsonProperty("Role")]
        public Dictionary<string, string>? Role { get; set; }

        /// <summary>人員狀態</summary>
        [JsonProperty("Status")]
        public bool? Status { get; set; }

        /// <summary>人員職稱</summary>
        [JsonProperty("JobTitle")]
        public string? JobTitle { get; set; }
    }

    /// <summary>
    /// 單位資料
    /// </summary>
    public class UnitData
    {
        /// <summary>單位階層（例：L90、L60）</summary>
        [JsonProperty("unit_level")]
        public string? UnitLevel { get; set; }

        /// <summary>單位階層（數字型態）</summary>
        [JsonProperty("unit_level_trans")]
        public int UnitLevelTrans { get; set; }

        /// <summary>單位編號</summary>
        [JsonProperty("unit_id")]
        public string? UnitId { get; set; }

        /// <summary>單位名稱</summary>
        [JsonProperty("unit_name")]
        public string? UnitName { get; set; }

        /// <summary>父單位編號</summary>
        [JsonProperty("parent_unit_id")]
        public string? ParentUnitId { get; set; }

        /// <summary>父單位名稱</summary>
        [JsonProperty("parent_unit_name")]
        public string? ParentUnitName { get; set; }

        /// <summary>HR 單位代碼</summary>
        [JsonProperty("unit_code")]
        public string? UnitCode { get; set; }

        /// <summary>停用/起用（例：enabled）</summary>
        [JsonProperty("status")]
        public string? Status { get; set; }

        /// <summary>單位屬性代碼</summary>
        [JsonProperty("property_code")]
        public string? PropertyCode { get; set; }

        /// <summary>單位屬性名稱</summary>
        [JsonProperty("property_name")]
        public string? PropertyName { get; set; }
    }
}
