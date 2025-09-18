using CommonUtilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using CDD.Web.FilterAttributes;
using CDD.Web.Helpers;
using CDD.Web.Libs;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Request;
using CDD.Web.Models.Response;
using CDD.Web.Models.Response.Login;
using CDD.Web.Serives;
using CDD.Web.Services;

namespace CDD.Web.Controllers
{
    /// <summary>
    /// 2024-09-24 手機登入相關
    /// </summary>
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ServiceFilter(typeof(SecurityHeadersAttribute))]                       // auto add Security Headers when output is viewResult
    [ServiceFilter(typeof(AutoLogAttribute))]                               // auto log input request and output response
    public class LoginController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<LoginController> _logger;

        private readonly IConfiguration _config;

        private readonly APIHelper _apiHelper;

        private readonly UserService _userService;

        private readonly ICaptchaHepler _captchaHepler;

        private readonly ApiKeyService _apiKeyService;

        #region appsetting.json 設定值

        private readonly ConfigurationSection _WebSetting;

        private readonly ExternalSystem _systemInfoForApiService;
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="apiHelper"></param> 
        /// <param name="userService"></param>
        /// <param name="captchaHepler"></param>
        /// <param name="apiKeyService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LoginController(
            IWebHostEnvironment env,
            ILogger<LoginController> logger,
            IConfiguration config,
            APIHelper apiHelper,
            UserService userService,
            ICaptchaHepler captchaHepler,
            ApiKeyService apiKeyService
            )
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _captchaHepler = captchaHepler ?? throw new ArgumentNullException(nameof(captchaHepler));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));

            // _Web Setting 
            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");

            // Init SystemInfoForApiService
            _systemInfoForApiService = _apiKeyService.GetSystemInfoForApiService();
        }

        /// <summary>
        /// SAML ACS Endpoint (SP (Service Provider) 用來接收 IdP POST 回來的 SAML Response)
        /// </summary>
        /// <returns></returns>
        [HttpPost("/Auth/Login")]
        public async Task<IActionResult> AdfsLogin()
        {
            bool res = await _userService.LoginBySaml2();
            if (res == true)
            {
                return RedirectToAction("SysManagement", "System");
            }
            return RedirectToAction("Login", "Login");
        }

        /// <summary>
        /// 登入頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Login")]
        public IActionResult Login()
        {
            return View("SPA");
        }

        /// <summary>
        /// 登入頁 ViewData
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/Login/ViewData")]
        public LoginViewDataResp API_LoginViewData()
        {
            LoginViewDataResp resp = new LoginViewDataResp() { Status = true };
            resp.rsaPublicKey = _userService.GetRsaPublicKeyPem();
            string code;
            resp.CaptchaCodeBase64Src = "data:image/jpeg;charset=utf-8;base64," + this._captchaHepler.GenerateCaptchaJpegBase64(120, 50, out code);
            HttpContext.Session.SetString(SessionKey.CaptchaCode, code);
            return resp;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("/api/[controller]/PostBack")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<GeneralResp> API_LoginPostBack([FromBody] LoginPostBackReq req)
        {
            // 驗證碼
            if (!this.ValidateCaptcha(req.CaptchaCode, true)) { throw MyExceptionList.CaptchaError(req).GetException(); }
            GeneralResp resp = await this._userService.Login(req);
            return resp;
        }

        #region Captcha 圖形驗證碼
        /// <summary>
        /// 核對 驗證碼
        /// 1. 預設驗證後清除
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool ValidateCaptcha(string code, bool resetCode = false)
        {
            if (_env.IsEnvironment("Development"))
            {
                return true;
            }
            string CaptchaCode = HttpContext.Session.GetString(SessionKey.CaptchaCode) ?? String.Empty;
            bool isValidate = CaptchaCode == code;
            if (resetCode) { HttpContext.Session.Remove(SessionKey.CaptchaCode); }
            this._logger.LogInformation($"CaptchaCode:{CaptchaCode};code:{code}");
            return isValidate;
        }
        #endregion


        /// <summary>
        /// 登入檢查
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/[controller]/Check")]
        public GeneralResp API_CheckIfLogin()
        {
            return this._userService.CheckIfLogin();
        }
    }
}