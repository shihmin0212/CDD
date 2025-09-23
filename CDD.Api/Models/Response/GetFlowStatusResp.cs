using Newtonsoft.Json;

namespace CDD.API.Models.Response
{
    /// <summary>
    /// FSA-002 GetFlowStatus
    /// GET /FlowStageVer2/GetFlowStatus/{signId}
    /// 輸入參數:
    ///   - signId: 表單編號
    /// 回傳主體:
    ///   - GetFlowStatusResp: 是否成功、訊息，以及流程狀態物件
    /// </summary>
    public class GetFlowStatusResp
    {
        /// <summary>
        /// 表單流程資訊
        /// </summary>
        public FlowStatus? FlowStatus { get; set; }

        /// <summary>
        /// 是否連接成功
        /// ex: TRUE
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 驗證訊息
        /// ex: 成功
        /// </summary>
        public List<string>? ValidationMsg { get; set; }
    }

    /// <summary>
    /// 表單流程資訊
    /// </summary>
    public class FlowStatus
    {
        /// <summary>
        /// 1.11 
        /// 加會單位清單
        /// </summary>
        public List<string[]>? CounterSignList { get; set; }

        /// <summary>
        /// 1.2 
        /// 目前關卡數
        /// ex: 2
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// 1.7 
        /// 歷史流程
        /// </summary>
        public List<string[]>? HistoryFlow { get; set; }

        /// <summary>
        /// 1.6
        /// JBPM表單流程UID
        /// ex: b00102615f4dbf02a62b21d4de39c3
        /// </summary>
        public string? JBPMUID { get; set; }

        /// <summary>
        /// 1.4
        /// 表單編號
        /// </summary>
        public string? SignID { get; set; }

        /// <summary>
        /// 1.1
        /// 流程狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 目前關卡顯示標題（SignedTitle）
        /// 外部文件未列出此欄，但常見作為目前節點名稱/角色稱呼（例：後檯經辦、後檯主管）。
        /// </summary>
        public string SignedTitle { get; set; } = null!;

        /// <summary>
        /// 目前流程識別代碼（CustomFlowKey）
        /// 外部文件：目前流程識別代碼（例：KYC_DueDiligence）。
        /// </summary>
        public string CustomFlowKey { get; set; } = null!;

        /// <summary>
        /// 是否可為代理人處理（AllowAgent）
        /// 外部文件：是否允許代理人執行此流程作業。
        /// </summary>
        public bool AllowAgent { get; set; }

        /// <summary>
        /// 是否可加會（GoCounterSigned）
        /// 外部文件：目前節點是否允許加會。
        /// </summary>
        public bool GoCounterSigned { get; set; }

        /// <summary>
        /// 目前關卡按鈕動作（StageAction）
        /// 外部文件：此關卡可執行之動作代碼集合，實際代碼意義依外部系統定義。
        /// 此處1、4、8為動作編號
        /// </summary>
        public List<int>? StageAction { get; set; }

        /// <summary>
        /// 是否可修改加會單位
        /// </summary>
        public bool AuthAddCSUnit { get; set; }

        /// <summary>
        /// 加會出口關卡
        /// </summary>
        public int ConterSignedExit { get; set; }
    }
}
