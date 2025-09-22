using Newtonsoft.Json;

namespace CDD.Api.Models.External
{
    /// <summary>外部 FlowStage GetFlowStatus 回應根物件</summary>
    public class FlowStatusRoot
    {
        [JsonProperty("FlowStatus")]
        public FlowStatusData? FlowStatus { get; set; }

        [JsonProperty("IsSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("ValidationMsg")]
        public List<string> ValidationMsg { get; set; } = new();
    }

    /// <summary>FlowStatus 區段資料</summary>
    public class FlowStatusData
    {
        [JsonProperty("CounterSignList")]
        public List<string> CounterSignList { get; set; } = new();

        [JsonProperty("CurrentStep")]
        public int CurrentStep { get; set; }

        [JsonProperty("HistoryFlow")]
        public List<string> HistoryFlow { get; set; } = new();

        [JsonProperty("JBPMUID")]
        public string? JBPMUID { get; set; }

        [JsonProperty("SignID")]
        public string? SignID { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("SignedTitle")]
        public string? SignedTitle { get; set; }

        [JsonProperty("CustomFlowKey")]
        public string? CustomFlowKey { get; set; }

        [JsonProperty("AllowAgent")]
        public bool AllowAgent { get; set; }

        [JsonProperty("GoCounterSigned")]
        public bool GoCounterSigned { get; set; }

        [JsonProperty("StageAction")]
        public List<int> StageAction { get; set; } = new();

        [JsonProperty("AuthAdCDDUnit")]
        public bool AuthAdCDDUnit { get; set; }

        // 外部實際欄位名稱（少一個 'u'）
        [JsonProperty("ConterSignedExit")]
        public int ConterSignedExit { get; set; }

        // 內部別名（方便使用）
        public int CounterSignedExit => ConterSignedExit;
    }
}
