using Microsoft.AspNetCore.Mvc;
using CDD.Web.Extensions;
using CDD.Web.FilterAttributes;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Request;
using CDD.Web.Models.Response;

namespace CDD.Web.Controllers
{
    public class StateErrorController : Controller
    {
        private readonly ILogger<StateErrorController> _logger;

        private readonly ConfigurationSection _WebSetting;

        public StateErrorController(IConfiguration config, ILogger<StateErrorController> logger)
        {
            this._WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
            _logger = logger;
        }

        #region 錯誤頁

        /// <summary>
        /// 錯誤頁
        /// </summary>
        /// <returns></returns>
        [HttpGet("/StateError")]
        public IActionResult StateError()
        {
            return View("SPA");
        }

        /// <summary>
        /// 錯誤頁資料
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("/api/StateError/ViewData")]
        [LogOptions(IsRecord_ResponseInfo = true)]
        public GeneralResp API_StateErrorViewData([FromBody] StateErrorViewDataReq req)
        {

            GeneralResp _resp = new GeneralResp();
            _resp.Status = true;
            _resp.ErrorCode = String.Empty;
            _resp.Title = "系統發生非預期錯誤";
            _resp.Message = "很抱歉，系統發生非預期錯誤，請重新進入線上服務，造成不便敬請見諒。";
            try
            {
                GeneralResp? resp = HttpContext.Session.GetObject<GeneralResp>(SessionKey.ExceptionGeneralResp);
                if (resp == null)
                {
                    return _resp;
                }
                resp.Status = true;
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
            }
            return _resp;
        }
        #endregion
    }
}
