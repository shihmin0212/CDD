using CDD.Api.Libs;

namespace CDD.Api.Services
{
    public class FlowStage_Config
    {
        public string BaseUrl { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class FlowStage_Service
    {
        private readonly ILogger<FlowStage_Service> _logger;
        private readonly IConfiguration _config;
        private readonly IRequest _request;
        private FlowStage_Config _flowStageConfig;

        public FlowStage_Service(
            ILogger<FlowStage_Service> logger,
            IRequest request,
            IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _flowStageConfig = GetFlowStageConfig();
        }

        private FlowStage_Config GetFlowStageConfig()
        {
            var config = new FlowStage_Config();
            _config.GetSection("FlowStage_Config").Bind(config);
            return config;
        }

        public async Task<FlowStageResult?> GetFlowStatusAsync(string signId)
        {
            string url = $"{_flowStageConfig.BaseUrl}/FlowStageVer2/GetFlowStatus?signId={signId}";

            try
            {
                // 直接呼叫共用 Request 封裝
                var resp = await _request.GetJSON<FlowStageResult>(url,[]);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage API 呼叫失敗");
                throw;
            }
        }

        public async Task<ProcessStatusResult?> GetProcessStatusAsync(string signId)
        {
            // 路由樣式依你提供的外部 API 範例
            string url = $"{_flowStageConfig.BaseUrl}/api/ver2//GetProcessStatus/{signId}";

            try
            {
                // 最簡模式：不帶 headers、不帶 log pattern
                var resp = await _request.GetJSON<ProcessStatusResult>(url, []);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetProcessStatus 呼叫失敗, signId={SignId}", signId);
                throw;
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

}
