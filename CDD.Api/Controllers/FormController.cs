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
    public class FormController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IIPHelper _ipHelper;
        private readonly APIHelper _apiHelper;
        private readonly IRequest _request;
        private readonly IMemoryCacheHelper _memoryCacheHelper;
        private readonly FlowService _flowService;
        private readonly ConfigurationSection _WebSetting;

        public FormController(
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<FlowStageController> logger,
            APIHelper apiHelper,
            IIPHelper ipHelper,
            IRequest request,
            IMemoryCacheHelper memoryCacheHelper,
            FlowService flowService)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
            _flowService = flowService ?? throw new ArgumentNullException(nameof(flowService));

            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        #region 表單起單
        /// <summary>
        /// 表單起單
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<GeneralResp<Guid>> StartProcess([FromBody] FlowCreateReq req)
        {
            Tuple<bool, string, Guid> res = await _flowService.CreateNewProcess(req);
            return new GeneralResp<Guid>() { Status = false, Message = res.Item2, Result = res.Item3 };
        }
        #endregion
    }
}