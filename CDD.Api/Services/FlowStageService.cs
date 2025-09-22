using System.Net;
using CDD.Api.Libs;
using Newtonsoft.Json;

namespace CDD.Api.Services
{
    public interface IFlowStageService
    {
        Task<FlowStageResult?> GetFlowStatusAsync(string signId);
        Task<ProcessStatusResult?> GetProcessStatusAsync(string signId);
        Task<GetMemberInfoResp?> GetMemberInfoAsync(string employeeID);
    }

    public class FlowStage_Config
    {
        /// <summary>
        /// 純網域根，例如：https://FlowStageAPISIT.testesunsec.com.tw
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>預留（目前不用）</summary>
        public string Token { get; set; } = null!;
    }

    public class FlowStageService : IFlowStageService
    {
        private readonly ILogger<FlowStageService> _logger;
        private readonly IConfiguration _config;
        private readonly IRequest _request;
        private readonly FlowStage_Config _flowStageConfig;

        // 將 BaseUrl 正規化（去掉尾斜線），避免組出 // 的路徑
        private readonly string _baseUrl;

        public FlowStageService(
            ILogger<FlowStageService> logger,
            IRequest request,
            IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _flowStageConfig = new FlowStage_Config();
            _config.GetSection("FlowStage_Config").Bind(_flowStageConfig);

            _baseUrl = (_flowStageConfig.BaseUrl ?? string.Empty).TrimEnd('/');
        }

        /// <summary>
        /// Step 7: 取得表單目前流程（外部：GET /api/FlowStageVer2/GetFlowStatus?signId={signId}）
        /// </summary>
        public async Task<FlowStageResult?> GetFlowStatusAsync(string signId)
        {
            string url = $"{_baseUrl}/api/FlowStageVer2/GetFlowStatus?signId={signId}";
            try
            {
                FlowStageResult resp = await _request.GetJSON<FlowStageResult>(url, headers: null);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetFlowStatus 呼叫失敗, signId={SignId}", signId);
                throw;
            }
        }

        /// <summary>
        /// 取得流程各節點狀態（外部：GET /api/ver2/GetProcessStatus/{signId}）
        /// </summary>
        public async Task<ProcessStatusResult?> GetProcessStatusAsync(string signId)
        {
            string url = $"{_baseUrl}/api/ver2/GetProcessStatus/{signId}";
            try
            {
                ProcessStatusResult resp = await _request.GetJSON<ProcessStatusResult>(url, headers: null);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetProcessStatus 呼叫失敗, signId={SignId}", signId);
                throw;
            }
        }

        /// <summary>
        /// FSA-011: 取得員工資訊（外部：GET /FlowStageVer2/GetMemberInfo/{employeeID}）
        /// </summary>
        public async Task<GetMemberInfoResp?> GetMemberInfoAsync(string employeeID)
        {
            string url = $"{_baseUrl}/FlowStageVer2/GetMemberInfo/{employeeID}";
            try
            {
                GetMemberInfoResp resp = await _request.GetJSON<GetMemberInfoResp>(url, headers: null);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetMemberInfo 呼叫失敗, employeeID={EmployeeID}", employeeID);
                throw;
            }
        }
    }
}


    /// <summary>
/// Step 7. (FlowStage) 取得表單目前流程
/// 外部 API: [GET] /api/FlowStageVer2/GetFlowStatus/{signId}
/// 輸入參數:
///   - signId: 表單編號
/// 回傳主體:
///   - FlowStageResult: 是否成功、訊息，以及流程狀態物件
/// </summary>
    public class FlowStageResult
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

    #region GetProcessStatus
    public class ProcessStatusResult
    {
        public List<ProcessStage>? Stages { get; set; }
        public bool HasNextFlow { get; set; }
        public bool IsInCounterSign { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? ValidationMsg { get; set; }
    }

    public class ProcessStage
    {
        public int Index { get; set; }
        public decimal StepSequence { get; set; }           // 1.0 / 2.0
        public string? CustomFlowKey { get; set; }
        public string? SignedTitle { get; set; }
        public string? SignedEmpName { get; set; }
        public string? SignedEmpNum { get; set; }
        public string? SignedTodo { get; set; }
        public string? SignedDate { get; set; }             // 例："2025/09/19"（為避免格式差異，先用字串）
        public int Status { get; set; }
    }
    #endregion

    /// <summary>
    /// FSA-011 GetMemberInfo 回傳根物件
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