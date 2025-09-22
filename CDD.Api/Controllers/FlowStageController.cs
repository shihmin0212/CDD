using CDD.Api.Attributes;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.Response;
using CDD.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CDD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LogOptions()]
    [ApiKeyAuthentication]
    [AdminTokenAuth]
    public class FlowStageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IIPHelper _ipHelper;
        private readonly APIHelper _apiHelper;
        private readonly IRequest _request;
        private readonly IMemoryCacheHelper _memoryCacheHelper;
        private readonly IFlowStageService _flowStageService;
        private readonly ConfigurationSection _WebSetting;

        public FlowStageController(
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<FlowStageController> logger,
            APIHelper apiHelper,
            IIPHelper ipHelper,
            IRequest request,
            IMemoryCacheHelper memoryCacheHelper,
            IFlowStageService flowStageService)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
            _flowStageService = flowStageService ?? throw new ArgumentNullException(nameof(flowStageService));

            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        #region GetFlowStatus
        /// <summary>
        /// 查詢簽核流程狀態
        /// </summary>
        /// <param name="signId">簽核單號</param>
        /// <returns></returns>
        [HttpGet("GetFlowStatus")]
        public async Task<GeneralResp<FlowStageResult?>> GetFlowStatus([FromQuery] string signId)
        {
            var result = await _flowStageService.GetFlowStatusAsync(signId);

            if (result == null)
            {
                return new GeneralResp<FlowStageResult?>()
                {
                    Status = false,
                    Message = "查無流程資料",
                    Result = null
                };
            }

            return new GeneralResp<FlowStageResult?>()
            {
                Status = true,
                Message = "成功",
                Result = result
            };
        }
        #endregion

        #region GetProcessStatus
        /// <summary>
        /// 取得流程節點狀態（外部服務完整回傳）
        /// </summary>
        /// <param name="signId">簽核單號</param>
        [HttpGet("GetProcessStatus/{signId}")]
        public async Task<GeneralResp<ProcessStatusResult?>> GetProcessStatus([FromRoute] string signId)
        {
            var result = await _flowStageService.GetProcessStatusAsync(signId);

            if (result == null)
            {
                return new GeneralResp<ProcessStatusResult?>()
                {
                    Status = false,
                    Message = "查無流程資料",
                    Result = null
                };
            }

            // 你希望「完整回傳訊息」，這裡不再加工 IsSuccess / ValidationMsg
            return new GeneralResp<ProcessStatusResult?>()
            {
                Status = true,
                Message = "成功",
                Result = result
            };
        }
        #endregion

        #region FSA-011 GetMemberInfo
        /// <summary>
        /// 取得員工資訊（每次一筆）
        /// </summary>
        /// <param name="employeeID">員工編號</param>
        [HttpGet("GetMemberInfo/{employeeID}")]
        public async Task<GeneralResp<GetMemberInfoResp?>> GetMemberInfo([FromRoute] string employeeID)
        {
            var result = await _flowStageService.GetMemberInfoAsync(employeeID);

            if (result == null)
            {
                return new GeneralResp<GetMemberInfoResp?>()
                {
                    Status = false,
                    Message = "查無資料",
                    Result = null
                };
            }

            // 保持完整外部內容，不加工 IsSuccess/ValidationMsg
            return new GeneralResp<GetMemberInfoResp?>()
            {
                Status = true,
                Message = "成功",
                Result = result
            };
        }
        #endregion

    }
}
