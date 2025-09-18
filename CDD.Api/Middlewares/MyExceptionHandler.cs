using System.Net;
using Newtonsoft.Json;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.DTO;
using CDD.Api.Models.Response;

namespace CDD.Api.Middlewares
{
    public class MyExceptionHandler
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<MyExceptionHandler> _logger;

        private readonly IIPHelper _ipHelper;

        public MyExceptionHandler(RequestDelegate next, ILogger<MyExceptionHandler> logger, IIPHelper ipHelper)
        {
            _next = next;
            _logger = logger;
            _ipHelper = ipHelper;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (MyException e)
            {
                handleMyException(httpContext, e);
            }
            catch (Exception e)
            {
                MyException _e = MyExceptionList.UnknownError(JsonConvert.SerializeObject(e)).GetException();
                handleMyException(httpContext, _e);
            }

        }

        private void handleMyException(HttpContext httpContext, MyException ex)
        {

            #region Log format 
            ExceptionLog log = new ExceptionLog();
            string historyActionId = httpContext?.Items[HttpContextItemKey.ActionId]?.ToString() ?? Guid.NewGuid().ToString();
            log.ActionId = new Guid(historyActionId);
            log.TraceIdentifier = httpContext?.TraceIdentifier ?? String.Empty;
            log.IP = _ipHelper.GetUserIP();
            log.Url = httpContext?.Request.Path.Value?.Substring(0, ((httpContext.Request.Path.Value.Length > 1000) ? 1000 : httpContext.Request.Path.Value.Length));
            log.CreateTime = string.Format("{0:u}", DateTime.Now);
            log.Method = httpContext?.Request.Method;
            log.RequestHeaders = (
                httpContext?.Request.Headers != null &&
                httpContext!.Request.Headers.Count > 0) ? string.Join("; ", httpContext!.Request.Headers.ToArray()) : null;
            log.ErrorCode = ex.Code;
            log.ErrorTitle = ex.Title?.Substring(0, ((ex.Title.Length > 1000) ? 1000 : ex.Title.Length));
            log.ErrorMessage = ex.Message?.Substring(0, ((ex.Message.Length > 1000) ? 1000 : ex.Message.Length));
            log.ExceptionObj = ex.DebugObj;
            _logger.LogError(JsonConvert.SerializeObject(log));
            #endregion

            if (httpContext == null) { return; }
            string resp = JsonConvert.SerializeObject(new GeneralResp<object>() { Status = false, Message = ex.Message });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (ex.Code.Equals(MyExceptionList.Unauthorized().Code)) ? (int)HttpStatusCode.Unauthorized : (int)HttpStatusCode.BadRequest;
            httpContext.Response.WriteAsync(resp);
            return;
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseMyExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyExceptionHandler>();
        }
    }
}
