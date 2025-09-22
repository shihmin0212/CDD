using Newtonsoft.Json;

namespace CDD.API.Models.Response
{
    /// <summary>
    /// Step 7. (FlowStage) 取得表單目前流程
    /// 外部 API: [GET] /api/FlowStageVer2/GetFlowStatus/{signId}
    /// 輸入參數:
    ///   - signId: 表單編號
    /// 回傳主體:
    ///   - GetFlowStatusResp: 是否成功、訊息，以及流程狀態物件
    /// </summary>
    public class GetFlowStatusResp
    {
        /// <summary>
        /// 流程狀態（FlowStatus）
        /// 內容包含：目前關卡、下一關卡(若有)、流程識別代碼、表單編號、是否可代理、目前關卡動作、流程狀態碼、歷程、加會資訊等。
        /// </summary>
        public FlowStatus? FlowStatus { get; set; }

        /// <summary>
        /// 是否成功（IsSuccess）
        /// 外部 API 執行結果是否成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 訊息（ValidationMsg）
        /// 外部 API 回傳的訊息集合（例如驗證失敗或提示）。
        /// </summary>
        public List<string>? ValidationMsg { get; set; }
    }

    /// <summary>
    /// 流程狀態（FlowStatus）
    /// 對應外部文件之「返回參數」清單。
    /// </summary>
    public class FlowStatus
    {
        /// <summary>
        /// 加會單位（CounterSignList）
        /// 外部文件：加會單位清單。因外部未提供明確結構，暫以 object 表示。
        /// </summary>
        public List<object>? CounterSignList { get; set; }

        /// <summary>
        /// 目前關卡（CurrentStep）
        /// 外部文件：目前流程所在的關卡序號。
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// 過往流程記錄（HistoryFlow）
        /// 外部文件：流程歷程紀錄清單。外部未提供明確結構，暫以 object 表示。
        /// </summary>
        public List<object>? HistoryFlow { get; set; }

        /// <summary>
        /// JBPM UID（JBPMUID）
        /// 外部文件：流程對應的 JBPM 唯一識別碼。
        /// </summary>
        public string JBPMUID { get; set; } = null!;

        /// <summary>
        /// 表單編號（SignID）
        /// 外部文件：表單編號。
        /// </summary>
        public string SignID { get; set; } = null!;

        /// <summary>
        /// 流程目前狀態（Status）
        /// 外部文件：流程狀態碼（數值），實際意義依外部系統定義。
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
