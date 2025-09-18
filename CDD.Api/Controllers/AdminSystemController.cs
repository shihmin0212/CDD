using System.Data;
using Microsoft.AspNetCore.Mvc;
using CDD.Api.Attributes;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.Request.Admin.System;
using CDD.Api.Models.Response;
using CDD.Api.Models.Response.Admin.System;
using CDD.Api.Repositories;
using CDD.Api.Repositories.DTO.Admin;

namespace CDD.Api.Controllers
{
    [Route("api/Admin/System")]
    [ApiController]
    [LogOptions()]
    [ApiKeyAuthentication]
    [AdminTokenAuth]
    public class AdminSystemController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;

        private readonly ILogger _logger;

        private readonly IIPHelper _ipHelper;

        private readonly APIHelper _apiHelper;

        private readonly SystemRepository _systemRepository; 

        private readonly AdminSystemRepository _adminSystemRepository;

        private readonly IRequest _request;

        private readonly IMemoryCacheHelper _memoryCacheHelper;

        private readonly ConfigurationSection _WebSetting;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="apiHelper"></param>
        /// <param name="ipHelper"></param>
        /// <param name="systemRepository"></param>
        /// <param name="adminSystemRepository"></param>
        /// <param name="request"></param>
        /// <param name="memoryCacheHelper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AdminSystemController(
                IWebHostEnvironment env, 
                IConfiguration config, 
                ILogger<AdminSystemController> logger, 
                APIHelper apiHelper,
                IIPHelper ipHelper,
                SystemRepository systemRepository, 
                AdminSystemRepository adminSystemRepository,
                Libs.IRequest request,
                IMemoryCacheHelper memoryCacheHelper)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _systemRepository = systemRepository ?? throw new ArgumentNullException(nameof(systemRepository)); 

            _adminSystemRepository = adminSystemRepository ?? throw new ArgumentNullException(nameof(adminSystemRepository));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
            _request = request ?? throw new ArgumentNullException(nameof(request));

            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");

        }


        /// <summary>
        /// 取得所有系統資訊 GetAllSystemInfo
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllSystemInfo")]
        public GeneralResp<GetAllSystemInfoResp> GetAllSystemInfo()
        {
            List<Repositories.DTO.SystemDTO>? _items = _systemRepository.GetSystemList()?.ToList();
            List<Repositories.DTO.SystemDTO> items = new List<Repositories.DTO.SystemDTO>();
            if (_items != null)
            {
                items = _items.Where(x => x.IsActive).Select(y =>
                {
                    return new Repositories.DTO.SystemDTO
                    {
                        System = y.System,
                        ApiKey = y.ApiKey,
                        IsActive = y.IsActive,
                        CreatedTime = y.CreatedTime,
                        LastUpdate = y.LastUpdate
                    };
                }).ToList();
            }
            return new GeneralResp<GetAllSystemInfoResp>()
            {
                Status = true,
                Result = new GetAllSystemInfoResp
                {
                    Items = items,
                }
            };
        }

        #region SystemListMangement 
        /// <summary>
        /// Create System Info
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public GeneralResp<object> CreateSystem([FromBody] SystemCreateReq req)
        {
            bool res = _adminSystemRepository.CreateSystem(req);
            return new GeneralResp<object>() { Status = res };
        }

        /// <summary>
        /// 停用系統 DisableSwitching
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("DisableSwitching")]
        public GeneralResp<object> DisableSwitching([FromBody] SystemDisableSwitchingReq req)
        {
            bool res = _adminSystemRepository.SystemDisableSwitching(req.SystemName, req.IsActive);
            return new GeneralResp<object>() { Status = res };
        }

        /// <summary>
        /// Update System Info
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public GeneralResp<object> UpdateSystem([FromBody] SystemUpdateReq req)
        {
            bool res = _adminSystemRepository.UpdateSystem(req);
            return new GeneralResp<object>() { Status = res };
        }

        /// <summary>
        /// 取得所有 System Info
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("ViewData")]
        public GeneralResp<SystemViewDataResp?> ViewData([FromBody] SystemViewDataReq req)
        {
            List<SystemAdminDTO>? systems = _adminSystemRepository.SystemViewData(req.SystemName, req.PageNumber, req.PageSize)?.ToList();
            return new GeneralResp<SystemViewDataResp?>()
            {
                Status = true,
                Result = new SystemViewDataResp
                {
                    Items = systems,
                    TotalCount = systems?.FirstOrDefault()?.TotalCount ?? 0,
                    PageNumber = req.PageNumber == 0 ? 1 : req.PageNumber,
                    PageSize = req.PageSize == 0 ? 10 : req.PageSize
                }
            };
        }

        #endregion

    }
}
