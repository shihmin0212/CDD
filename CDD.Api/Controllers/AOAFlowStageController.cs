using CDD.Api.Attributes;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.Response;
using CDD.Api.Services;
using CDD.API.Models.Response;
using CDD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CDD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LogOptions()]
    [ApiKeyAuthentication]
    [AdminTokenAuth]
    public class AOAFlowStageController : ControllerBase
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
        private readonly AOAFlowStageService _aoaflowStageService;

        public AOAFlowStageController(
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<FlowStageController> logger,
            APIHelper apiHelper,
            IIPHelper ipHelper,
            IRequest request,
            IMemoryCacheHelper memoryCacheHelper,
            IFlowStageService flowStageService,
            AOAFlowStageService aoaflowStageService)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
            _flowStageService = flowStageService ?? throw new ArgumentNullException(nameof(flowStageService));
            _aoaflowStageService = aoaflowStageService ?? throw new ArgumentNullException(nameof(aoaflowStageService));

            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        /// <summary>
        /// 取得表單狀態
        /// </summary>
        /// <param name="formNum">表單編號</param>
        /// <returns>退回關卡資訊</returns>
        [HttpGet]
        [Route("GetFlowStatus/{formNum}")]
        public ActionResult<GetFlowStatusResp> GetFlowStatus(string formNum)
        {
            return Ok(_aoaflowStageService.GetFlowStatus(formNum));
        }

        /// <summary>
        /// 取得單據的所有簽核關卡及簽核歷程
        /// </summary>
        /// <param name="formNum">單據編號</param>
        /// <returns>簽核鏈</returns>
        [HttpGet]
        [Route("GetProcessStatus/{formNum}")]
        public ActionResult<ResponseProcessStages> GetProcessStatus(string formNum)
        {
            return Ok(_aoaflowStageService.GetProcessStatus(formNum));
        }

    }
}
