using System.Security.Cryptography;
using Newtonsoft.Json;
using CDD.Web.Extensions;
using CDD.Web.Helpers;
using CDD.Web.Libs;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Response;

namespace CDD.Web.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandler> _logger;

        private readonly IIPHelper _ipHelper;

        private readonly ConfigurationSection _WebSetting;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger, IIPHelper ipHelper,
            IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _ipHelper = ipHelper;
            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (MyException e)
            {
                this.handleOOAException(httpContext, e);
            }
            catch (HttpRequestException e)
            {
                MyException _e = MyExceptionList.HttpError(JsonConvert.SerializeObject(e)).GetException();
                this.handleOOAException(httpContext, _e);
            }
            catch (TaskCanceledException e)
            {
                MyException _e = MyExceptionList.HttpError(JsonConvert.SerializeObject(e)).GetException();
                this.handleOOAException(httpContext, _e);
            }
            catch (CryptographicException e)
            {
                MyException _e = MyExceptionList.SessionExpired(JsonConvert.SerializeObject(e)).GetException();
                this.handleOOAException(httpContext, _e);
            }
            catch (Exception e)
            {
                MyException _e = MyExceptionList.Unknown(JsonConvert.SerializeObject(e)).GetException();

                if (e is BadHttpRequestException badRequestException && badRequestException.StatusCode == StatusCodes.Status413PayloadTooLarge)
                {
                    _e = MyExceptionList.PayloadTooLargeError(JsonConvert.SerializeObject(e)).GetException();
                }
                this.handleOOAException(httpContext, _e);
            }
        }

        private void handleOOAException(HttpContext httpContext, MyException ex)
        {
            #region Log Error 
            ExceptionLog log = new ExceptionLog();
            string historyActionId = httpContext?.Items[HttpContextItemKey.ActionId]?.ToString() ?? Guid.NewGuid().ToString();
            log.ActionId = new Guid(historyActionId);
            log.TraceIdentifier = httpContext?.TraceIdentifier ?? String.Empty;
            log.IP = _ipHelper.GetUserIP();
            log.Url = httpContext?.Request.Path.Value;
            log.CreateTime = string.Format("{0:u}", DateTime.Now);
            log.Method = httpContext?.Request.Method;
            log.RequestHeaders = (httpContext != null) ? string.Join("; ", httpContext.Request.Headers.ToList()) : String.Empty;
            log.ErrorCode = ex.Code;
            log.ErrorTitle = ex.Title;
            log.ErrorMessage = ex.Message;
            log.ExceptionObj = ex.DebugObj;
            this._logger.LogError(JsonConvert.SerializeObject(log));
            #endregion

            bool isGetRequest = String.Equals("GET", httpContext!.Request.Method, StringComparison.OrdinalIgnoreCase);
            if (isGetRequest)
            {
                this.HandleGetMethodError(httpContext, ex);
            }
            else
            {
                this.HandlePostMethodError(httpContext, ex);
            }

        }

        // 包含 PUT DELETE PATACH 排除 GET 
        // Incoming Page等表單submit頁，非xhr不能回 json format，須轉導
        // 1. Bank Login postBack page
        // 2. TWID postBack page
        private void HandlePostMethodError(HttpContext httpContext, MyException ex)
        {
            // PathString path = httpContext.Request.Path.ToString();
            string resp = JsonConvert.SerializeObject(
                new GeneralResp() { Status = false, Title = ex.Title, ErrorCode = ex.Code, Message = $"{ex.Message}" });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 400;
            httpContext.Response.WriteAsync(resp);
            return;
        }

        private void HandleGetMethodError(HttpContext httpContext, MyException ex)
        {
            httpContext.Response.StatusCode = 400;
            // 錯誤步驟跳頁 導回首頁
            if (ex.Code == MyExceptionList.AccessDenined().Code)
            {
                httpContext.Response.Redirect("/Login");
            }
            else if (ex.Code == MyExceptionList.SessionExpired().Code)
            {
                httpContext.Response.Redirect("/Login" + httpContext.Request.QueryString);
            }
            else
            {
                // Redirect To StateErrorPage
                httpContext.Session.SetObject(SessionKey.ExceptionGeneralResp, new GeneralResp() { Status = false, Title = ex.Title, ErrorCode = ex.Code, Message = $"{ex.Message}({ex.Code})" });
                httpContext.Response.StatusCode = 200;
                // 錯誤步驟跳頁 導回首頁
                // Redirect To StateErrorPage
                httpContext.Response.Redirect("/StateError");
            }
            return;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class OOAExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseMyExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandler>();
        }
    }
}
