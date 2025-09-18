using Microsoft.AspNetCore.Mvc;
using CDD.Web.FilterAttributes;
using CDD.Web.Helpers;
using CDD.Web.Models.Request.System;
using CDD.Web.Models.Response;
using CDD.Web.Models.Response.System;
using CDD.Web.Serives;

namespace CDD.Web.Controllers
{
    /// <summary>
    /// 系統管理控制器
    /// </summary>
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ServiceFilter(typeof(SecurityHeadersAttribute))]
    [ServiceFilter(typeof(AutoLogAttribute))]
    //[RequireLogin]
    [ServiceFilter(typeof(ValidateModelAttribute))]
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        private readonly APIHelper _apiHelper;
        private readonly ILogger<SystemController> _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="apiHelper"></param>
        /// <param name="logger"></param>
        public SystemController(APIHelper apiHelper, ILogger<SystemController> logger)
        {
            _apiHelper = apiHelper;
            _logger = logger;
        }

        /// <summary>
        /// Index頁
        /// </summary>
        /// <returns></returns>
        [HttpGet("/system")]
        public IActionResult SysManagement()
        {
            return View("SPA");
        }

        /// <summary>
        /// 取得所有系統管理資料
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllSystemInfo")]
        public async Task<GeneralResp<GetAllSystemInfoResult>> GetAllSystemInfo()
        {
            API_GetAllSystemInfoResp apiResp = await _apiHelper.GetAllSystemInfo();
            GeneralResp<GetAllSystemInfoResult> resp = new GeneralResp<GetAllSystemInfoResult>
            {
                Status = apiResp.Status,
                Title = "系統管理資料",
                Message = "成功取得系統管理資料",
                Result = apiResp.Result
            };
            return resp;
        }

        /// <summary>
        /// 取得系統管理視圖資料
        /// </summary>
        /// <param name="request">查詢請求</param>
        /// <returns></returns>
        [HttpPost("ViewData")]
        public async Task<GeneralResp<SystemViewDataResult>> GetSystemViewData([FromBody] SystemViewDataReq request)
        {
            API_SystemViewDataResp apiResp = await _apiHelper.GetSystemViewData(request);
            GeneralResp<SystemViewDataResult> resp = new GeneralResp<SystemViewDataResult>
            {
                Status = apiResp.Status,
                Title = "系統管理視圖資料",
                Message = "成功取得系統管理視圖資料",
                Result = apiResp.Result
            };
            return resp;
        }

        /// <summary>
        /// 新增系統
        /// </summary>
        /// <param name="request">新增請求</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<GeneralResp> CreateSystem([FromBody] CreateSystemReq request)
        {
            return await _apiHelper.CreateSystem(request);
        }

        /// <summary>
        /// 更新系統
        /// </summary>
        /// <param name="request">更新請求</param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<GeneralResp> UpdateSystem([FromBody] UpdateSystemReq request)
        {
            return await _apiHelper.UpdateSystem(request);
        }

        /// <summary>
        /// 停用系統
        /// </summary>
        /// <param name="request">停用請求</param>
        /// <returns></returns>
        [HttpPost("Deactivate")]
        public async Task<GeneralResp> DeactivateSystem([FromBody] DeactivateSystemReq request)
        {
            return await _apiHelper.DeactivateSystem(request);
        }

        /// <summary>
        /// 產生系統Token
        /// </summary>
        /// <param name="req">產生Token請求</param>
        /// <returns></returns>
        [HttpPost("GenerateToken")]
        public GeneralResp<GenerateSystemTokenResp> GenerateSystemToken([FromBody] GenerateSystemTokenReq req)
        {
            Tuple<string, string> EncryptedApiKey = ApiKeyService.GetSystemWithEncryptedApiKey(req.System, req.ApiKey, req.HashKey, req.IVKEy);
            var Response = new GeneralResp<GenerateSystemTokenResp>
            {
                Status = true,
                Title = "Token 產生成功",
                Message = "系統 Token 已成功產生。",
                Result = new GenerateSystemTokenResp
                {
                    System = req.System,
                    EncryptedToken = EncryptedApiKey.Item2, // 這裡應該是實際的加密 Token
                    DecryptedContent = " 僅供測試用，實際應該不返回", // 僅供測試用，實際應該不返回
                    CreatedTime = DateTime.UtcNow.ToString("o") // ISO 8601 格式的時間
                }
            };
            return Response;
        }
    }
}