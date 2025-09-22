using Newtonsoft.Json;
namespace CDD.API.Models.Response
{
    /// <summary>
    /// (FlowStepProfile) 關卡資訊清單 + 成功旗標 + 訊息
    /// 外部 API 回傳主體：
    ///   - FlowStepProfile：關卡資訊（通常為多筆）
    ///   - IsSuccess：是否連接成功
    ///   - ValidationMsg：驗證訊息（字串陣列）
    /// </summary>
    public class GetBackApproverResp
    {
        /// <summary>
        /// 關卡資訊清單（FlowStepProfile）
        /// </summary>
        [JsonProperty("FlowStepProfile")]
        public FlowStepProfile FlowStepProfile { get; set; }

        /// <summary>
        /// 是否連接成功（IsSuccess）
        /// </summary>
        [JsonProperty("IsSuccess")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 驗證訊息（ValidationMsg）
        /// 例如：["成功"]、或錯誤/警示訊息集合
        /// </summary>
        [JsonProperty("ValidationMsg")]
        public List<string> ValidationMsg { get; set; }
    }

    /// <summary>
    /// 關卡資訊（FlowStepProfile）
    /// 對應「二、返回參數」表中的欄位 1.1 ~ 1.11
    /// </summary>
    public class FlowStepProfile
    {
        /// <summary>
        /// 關卡數/序號（StepSequence）
        /// 例：1.0
        /// </summary>
        [JsonProperty("StepSequence")]
        public string StepSequence { get; set; }

        /// <summary>
        /// 預設簽核意見罐頭文字（SignedComment）
        /// 例："罐頭文字"
        /// </summary>
        [JsonProperty("SignedComment")]
        public string? SignedComment { get; set; }

        /// <summary>
        /// 關卡處理天數（OverdueDays）
        /// 例：5
        /// </summary>
        [JsonProperty("OverdueDays")]
        public int OverdueDays { get; set; }

        /// <summary>
        /// 關卡名稱（SignedTitle）
        /// 例："申請人"
        /// </summary>
        [JsonProperty("SignedTitle")]
        public string? SignedTitle { get; set; }

        /// <summary>
        /// 關卡簽核人員員編（SignedID）
        /// 例：["16073"]
        /// </summary>
        [JsonProperty("SignedID")]
        public List<string>? SignedID { get; set; }

        /// <summary>
        /// 關卡簽核人員姓名（SignedName）
        /// 例：["李OO"]
        /// </summary>
        [JsonProperty("SignedName")]
        public List<string>? SignedName { get; set; }

        /// <summary>
        /// 關卡簽核類別（SignedType）
        /// 外部定義之代碼（例：2）
        /// </summary>
        [JsonProperty("SignedType")]
        public int SignedType { get; set; }

        /// <summary>
        /// 客製化流程代碼（CustomFlowKey）
        /// 例："ENP_P1_01"
        /// </summary>
        [JsonProperty("CustomFlowKey")]
        public string? CustomFlowKey { get; set; }

        /// <summary>
        /// 加會出口關卡（CounterSignedExit）
        /// 例：1
        /// </summary>
        [JsonProperty("CounterSignedExit")]
        public int CounterSignedExit { get; set; }

        /// <summary>
        /// 該關卡副本通知人員員編（NoticeUserID）
        /// 例：["17845"]
        /// </summary>
        [JsonProperty("NoticeUserID")]
        public List<string>? NoticeUserID { get; set; }

        /// <summary>
        /// 該關卡副本通知人員姓名（NoticeUserName）
        /// 例：["黃OO"]
        /// </summary>
        [JsonProperty("NoticeUserName")]
        public List<string>? NoticeUserName { get; set; }
    }
}
