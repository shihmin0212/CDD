using Newtonsoft.Json;
using CDD.Web.Libs;
using CDD.Web.Models.APIResponse;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Request;
using CDD.Web.Models.Response;
using CDD.Web.Models.Response.System;
using CDD.Web.Serives;

namespace CDD.Web.Helpers
{
    public class APIHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly Libs.IRequest _request;
        private readonly ApiKeyService _apiKeyService;

        private readonly ConfigurationSection _URL;
        private readonly ConfigurationSection _APISetting;
        private readonly ExternalSystem _systemInfoForApiService;
        private readonly Dictionary<string, string> _apiKeyHeaders;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <param name="apiKeyService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public APIHelper(
            IWebHostEnvironment env,
            ILogger<APIHelper> logger,
            IConfiguration config,
            Libs.IRequest request,
            ApiKeyService apiKeyService
            )
        {
            _env = env;
            _logger = logger;
            _config = config;
            _request = request;
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _URL = (ConfigurationSection)config.GetSection("URL");
            _APISetting = (ConfigurationSection)config.GetSection("APISetting");


            // Init SystemInfoForApiService
            _systemInfoForApiService = _apiKeyService.GetSystemInfoForApiService();
            _apiKeyHeaders = _apiKeyService.GetApiKeyHeaders();
        }


        /// <summary>
        /// 取得停登訊息 [Common].[dbo].[ESUN_SystemPause]
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<API_GetServiceAlertResp> GetServiceAlert(string ip)
        {
            string url = _URL["GetServiceAlert"]?.Replace(":ip", ip) ?? throw new ArgumentNullException("GetServiceAlert not found in appsetting");
            try
            {
                if (this._env.IsEnvironment("Development"))
                {
                    return new API_GetServiceAlertResp { code = "0" };
                }
                API_GetServiceAlertResp resp = await _request.GetJSON<API_GetServiceAlertResp>(url, null);
                if (resp == null) { throw new Exception("ApiResponse Null"); }
                return resp;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HttpRequestException:" + JsonConvert.SerializeObject(ex));
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError("TaskCanceledException:" + JsonConvert.SerializeObject(ex));
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception:" + JsonConvert.SerializeObject(ex));
                throw MyExceptionList.GetServiceAlertError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyAdminUser(LoginData postData)
        {
            string url = _URL["Login"] ?? throw new ArgumentNullException("VerifyAdminUser not found in appsetting");
            API_GeneralResp resp;
            try
            {
                resp = await _request.PostJSON<API_GeneralResp>(url, JsonConvert.SerializeObject(postData), _apiKeyHeaders);
                if (resp != null && resp.Status)
                {
                    return true;
                }
                else
                {
                    throw MyExceptionList.Unauthorized().GetException();
                }
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.Unauthorized($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }


        #region System

        /// <summary>
        /// 系統管理-取得所有系統資訊
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<API_GetAllSystemInfoResp> GetAllSystemInfo()
        {
            string url = _URL["GetAllSystemInfo"] ?? throw new ArgumentNullException("GetAllSystemInfo not found in appsetting");

            try
            {
                API_GetAllSystemInfoResp resp = await _request.PostJSON<API_GetAllSystemInfoResp>(url, string.Empty, _apiKeyHeaders);
                if (resp == null) throw new Exception("ApiResponse Null");

                return resp;
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SystemApiError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }

        /// <summary>
        /// 系統管理-取得視圖資料
        /// </summary>
        /// <param name="request">查詢請求</param>
        /// <returns></returns>
        public async Task<API_SystemViewDataResp> GetSystemViewData(Models.Request.System.SystemViewDataReq request)
        {
            string url = _URL["SystemViewData"] ?? throw new ArgumentNullException("SystemViewData not found in appsetting");

            try
            {
                API_SystemViewDataResp resp = await _request.PostJSON<API_SystemViewDataResp>(url, JsonConvert.SerializeObject(request), _apiKeyHeaders);
                if (resp == null) throw new Exception("ApiResponse Null");

                return resp;
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SystemApiError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }

        /// <summary>
        /// 系統管理-新增系統
        /// </summary>
        /// <param name="request">新增請求</param>
        /// <returns></returns>
        public async Task<GeneralResp> CreateSystem(Models.Request.System.CreateSystemReq request)
        {
            string url = _URL["SystemCreate"] ?? throw new ArgumentNullException("SystemCreate not found in appsetting");

            try
            {
                API_GeneralResp resp = await _request.PostJSON<API_GeneralResp>(url, JsonConvert.SerializeObject(request), _apiKeyHeaders);
                if (resp == null) throw new Exception("ApiResponse Null");

                return new GeneralResp
                {
                    Status = resp.Status,
                    Message = resp.Message
                };
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SystemApiError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }

        /// <summary>
        /// 系統管理-更新系統
        /// </summary>
        /// <param name="request">更新請求</param>
        /// <returns></returns>
        public async Task<GeneralResp> UpdateSystem(Models.Request.System.UpdateSystemReq request)
        {
            string url = _URL["SystemUpdate"] ?? throw new ArgumentNullException("SystemUpdate not found in appsetting");

            try
            {
                API_GeneralResp resp = await _request.PostJSON<API_GeneralResp>(url, JsonConvert.SerializeObject(request), _apiKeyHeaders);
                if (resp == null) throw new Exception("ApiResponse Null");

                return new GeneralResp
                {
                    Status = resp.Status,
                    Message = resp.Message
                };
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SystemApiError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }



        /// <summary>
        /// 系統管理-停用系統
        /// </summary>
        /// <param name="request">停用請求</param>
        /// <returns></returns>
        public async Task<GeneralResp> DeactivateSystem(Models.Request.System.DeactivateSystemReq request)
        {
            string url = _URL["SystemDeactivate"] ?? throw new ArgumentNullException("SystemDeactivate not found in appsetting");

            try
            {
                API_GeneralResp resp = await _request.PostJSON<API_GeneralResp>(url, JsonConvert.SerializeObject(request), _apiKeyHeaders);
                if (resp == null) throw new Exception("ApiResponse Null");

                return new GeneralResp
                {
                    Status = resp.Status,
                    Message = resp.Message
                };
            }
            catch (HttpRequestException ex)
            {
                throw MyExceptionList.HttpError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (TaskCanceledException ex)
            {
                throw MyExceptionList.TaskCanceledError($"[timed out];getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SystemApiError($"getURL:{url};exMsg:{JsonConvert.SerializeObject(ex)};").GetException();
            }
        }

        #endregion

    }

}
