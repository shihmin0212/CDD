using CDD.Api.Libs;
using CDD.API.Models.Response;
using CDD.API.Services.Interfaces;

namespace CDD.Api.Services
{
    public class FlowStageConfig
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
        private readonly FlowStageConfig _flowStageConfig;

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
            _flowStageConfig = new FlowStageConfig();
            _config.GetSection("FlowStage_Config").Bind(_flowStageConfig);
            _baseUrl = (_flowStageConfig.BaseUrl ?? string.Empty).TrimEnd('/');
        }

        /// <summary>
        /// FSA-002: 由表單編號取得流程狀態
        /// GET /api/FlowStageVer2/GetFlowStatus?signId={signId}
        /// </summary>
        public async Task<GetFlowStatusResp> GetFlowStatusAsync(string signId)
        {
            string url = $"{_baseUrl}/api/FlowStageVer2/GetFlowStatus?signId={signId}";
            try
            {
                GetFlowStatusResp resp = await _request.GetJSON<GetFlowStatusResp>(url, headers: null);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetFlowStatus 呼叫失敗, signId={SignId}", signId);
                throw;
            }
        }

        /// <summary>
        /// 沒有文件規格
        /// GET /api/ver2/GetProcessStatus/{signId}
        /// </summary>
        public async Task<ProcessStatusResult> GetProcessStatusAsync(string signId)
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
        /// FSA-011: 取得員工資訊
        /// GET /FlowStageVer2/GetMemberInfo/{employeeID}
        /// </summary>
        public async Task<GetMemberInfoResp> GetMemberInfoAsync(string employeeID)
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

        /// <summary>
        /// FSA-004: 取得退回關卡資訊
        /// GET /FlowStageVer2/GetBackApprover/{signId}/{stageDesignate}
        /// </summary>
        public async Task<GetBackApproverResp> GetBackApproverAsync(string signId, string stageDesignate)
        {
            string url = $"{_baseUrl}/FlowStageVer2/GetBackApprover?signId={signId}&stageDesignate={stageDesignate}";
            try
            {
                GetBackApproverResp resp = await _request.GetJSON<GetBackApproverResp>(url, headers: null);
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "FlowStage GetProcessStatus 呼叫失敗, signId={SignId}", signId);
                throw;
            }
        }
    }
}