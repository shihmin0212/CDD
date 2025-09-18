using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using CDD.Web.Extensions;
using CDD.Web.Helpers;
using CDD.Web.Models.APIResponse;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Response;

namespace CDD.Web.Controllers
{
    /// <summary>
    /// 停止登入或系統公告Api
    /// </summary>
    [Route("api/Base")]
    [ApiController]
    public class ServiceAlertController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<ServiceAlertController> _logger;

        private readonly IConfiguration _config;

        private readonly IAntiforgery _antiforgery;

        private readonly APIHelper _apiHelper;

        private readonly ConfigurationSection _cookieConfig;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="antiforgery"></param>
        /// <param name="apiHelper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ServiceAlertController(
            IWebHostEnvironment env,
            ILogger<ServiceAlertController> logger,
            IConfiguration config,
            IAntiforgery antiforgery,
            APIHelper apiHelper
            )
        {
            this._env = env;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            this._antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
            this._cookieConfig = (ConfigurationSection)config.GetSection("CookieConfig") ?? throw new ArgumentNullException("CookieConfig");
        }

        /// <summary>
        /// 初始化/刷新 session cookie 、 XSRF cookie
        /// </summary>
        /// <returns></returns>
        private string? GetAntiforgeryTokenAndInitSession()
        {
            string? token = null;
            if (!HttpContext.Response.HasStarted)
            {
                // On POST requests it will validate the XSRF header
                // await this.antiforgery.IsRequestValidAsync(HttpContext)
                // 以非同步方式傳回值，指出要求是否通過反移驗證。 如果要求使用安全 HTTP 方法 (GET、HEAD、OPTIONS、TRACE) ，則不會驗證反分叉權杖
                // 如果 Task<TResult> 要求使用安全 HTTP 方法或包含有效的反移轉權杖，則完成時會傳回 true ，否則會傳回 false。
                string debugInfo = $"XSRF Cookies:{HttpContext.Request.Cookies[XSRFKey.CookieName]}; Headers:{HttpContext.Request.Headers[XSRFKey.HeaderName]}";
                this._logger.LogInformation(debugInfo);

                // SPA開發時，透過api response 取 xsrf token/ SPA開發完後整合到NET core wwwroot後則用cshtml注入
                // ApiClient.ts:44
                if (this._env.IsDevelopment())
                {
                    AntiforgeryTokenSet? tokens = this._antiforgery.GetAndStoreTokens(HttpContext);
                    this._antiforgery.SetCookieTokenAndHeader(HttpContext);                                  // header set-cookie antiforgery cookie
                    token = tokens.RequestToken;
                }
                HttpContext.Session.SetString(SessionKey.SessionCookieCreateTime, DateTime.Now.ToString()); // header set-cookie session cookie
            }
            else
            {
                string error = $"HttpContext.Response.HasStarted; {HttpContext.Response.HasStarted}";
                this._logger.LogError(error);
            }
            return token;
        }

        #region 停止登入訊息提示、逾時提示
        /// <summary>
        /// 取得停登訊息
        /// </summary>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("ServiceAlert")]
        public GetServiceAlertResp GetServiceAlert()
        {
            GetServiceAlertResp resp = new GetServiceAlertResp();
            resp.RequestToken = this.GetAntiforgeryTokenAndInitSession();
            resp.Status = true; 
            int IdleTimeoutHintMinutes = this._cookieConfig.GetValue("IdleTimeoutHintMinutes", 9); // 逾時提示時間

            resp.IdleTimeoutHintMinutes = IdleTimeoutHintMinutes;     // 逾時提醒時間
            return resp;
        }

        /// <summary>
        /// 延展 session idle timeout
        /// </summary>
        /// <returns></returns>
        [HttpPost("ConfirmIdleAlert")]
        public GeneralResp ConfirmIdleAlert()
        {
            return new GeneralResp() { Status = true };
        }

        /// <summary>
        /// 前端逾時轉導 redirect action
        /// </summary>
        /// <returns></returns>
        [HttpPost("IdleTimeout")]
        public GeneralResp IdleTimeout()
        {
            // Session 重置
            HttpContext.Session.Clear();
            return new GeneralResp() { Status = true };
        }

        /// <summary>
        /// 確認閱讀 停止登入 訊息
        /// </summary>
        /// <returns></returns>
        [HttpPost("ConfirmServiceAlert")]
        public GeneralResp ConfirmServiceAlert()
        {
            HttpContext.Session.Remove(SessionKey.ConfirmServiceAlert);
            string confirmedMsg = HttpContext.Session.GetString(SessionKey.ServiceAlertResp) ?? String.Empty;
            HttpContext.Session.SetString(SessionKey.ConfirmServiceAlert, confirmedMsg);
            return new GeneralResp() { Status = true };
        }
        #endregion

    }
}
