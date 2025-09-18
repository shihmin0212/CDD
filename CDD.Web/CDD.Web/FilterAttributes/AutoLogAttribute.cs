using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using CDD.Web.Helpers;
using CDD.Web.Models.DTO;

namespace CDD.Web.FilterAttributes
{
    /// <summary>
    /// Auto Log Request And Response 
    /// For Logging Use As ServiceFilter
    /// </summary>
    public class AutoLogAttribute : ActionFilterAttribute
    {
        private readonly ILogger<AutoLogAttribute> _logger;

        private readonly IIPHelper ipHelper;

        private readonly IHttpContextAccessor contextAccessor;

        private HttpContext httpContext
        {
            get
            {
                return contextAccessor.HttpContext;
            }
        }

        private WebClientLog log;

        public AutoLogAttribute(ILogger<AutoLogAttribute> logger, IIPHelper ipHelper, IHttpContextAccessor accessor)
        {
            this._logger = logger;
            this.ipHelper = ipHelper;
            this.contextAccessor = accessor;
            this.log = new WebClientLog();
            httpContext.Items[HttpContextItemKey.ActionId] = httpContext?.Items[HttpContextItemKey.ActionId]?.ToString() ?? Guid.NewGuid().ToString();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IgnoreLogging? ignoreController = (context.ActionDescriptor as ControllerActionDescriptor)?
                .ControllerTypeInfo.GetCustomAttributes<IgnoreLogging>().FirstOrDefault();

            IgnoreLogging? ignoreAction = (context.ActionDescriptor as ControllerActionDescriptor)?
                .MethodInfo.GetCustomAttributes<IgnoreLogging>().FirstOrDefault();

            LogOptions? LogOptions = (context.ActionDescriptor as ControllerActionDescriptor)?
                .MethodInfo.GetCustomAttributes<LogOptions>().FirstOrDefault();

            // Disable logging
            if (ignoreController != null || ignoreAction != null)
            {
                await next();
                return;
            }
            #region Log Request
            this.LogResquest(LogOptions, context);
            #endregion

            ActionExecutedContext actionExecutedContext = await next();

            #region Log Response
            this.LogResponse(LogOptions, actionExecutedContext);
            #endregion

        }

        private void LogResquest(LogOptions? LogOptions, ActionExecutingContext context)
        {
            ControllerActionDescriptor? descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string? actionName = descriptor?.ActionName;
            string? controllerName = descriptor?.ControllerName;
            log.Action = $"{controllerName}/{actionName}";

            string historyActionId = httpContext?.Items[HttpContextItemKey.ActionId]?.ToString() ?? Guid.NewGuid().ToString();
            log.ActionId = new Guid(historyActionId);
            log.IP = ipHelper.GetUserIP();
            log.TraceIdentifier = httpContext.TraceIdentifier;
            log.Url = httpContext.Request.Path.Value;

            log.CreateTime = string.Format("{0:u}", DateTime.Now);
            log.Method = httpContext.Request.Method;
            log.RequestHeaders = string.Join("; ", httpContext.Request.Headers.ToList());

            // Option attribute 是否紀錄 IncomeRequest Model
            // 1.預設不記錄
            // 2.若有指定option attribute，則指定 isRecord_RequestModel = true 才會記錄
            if (LogOptions != null && LogOptions.IsRecord_RequestModel)
            {
                var args = context.ActionArguments.Select(x => $"{JsonConvert.SerializeObject(x.Value)}");
                string requestModel = string.Join(",", args);
                string[] noLogPatterns = LogOptions?.NoLogReqPatterns ?? new string[] { };
                foreach (string regex in noLogPatterns)
                {
                    requestModel = Regex.Replace(requestModel, regex, (LogOptions?.ReplaceStr ?? regex));
                }
                log.RequestModel = requestModel;

            }
            string requestLog = JsonConvert.SerializeObject(log);
            _logger.LogInformation(requestLog);
        }

        private void LogResponse(LogOptions? LogOptions, ActionExecutedContext actionExecutedContext)
        {
            log.ResponseStatusCode = actionExecutedContext.HttpContext.Response.StatusCode;
            ViewResult? viewResult = actionExecutedContext.Result as ViewResult;
            ObjectResult? objResult = actionExecutedContext.Result as ObjectResult;

            // Option attribute 是否紀錄 Response log
            // 1.預設記錄
            // 2.若有指定option attribute 預設值 isActiveLogResponseFullInfo = true　不需要紀錄則指定isActiveLogResponseFullInfo = false
            if (actionExecutedContext.Exception == null && (LogOptions == null || (LogOptions != null && LogOptions.IsRecord_ResponseInfo)))
            {
                // Option attribute  回應log格式 是否紀錄 IncomeRequest Model + Request Headers
                // 1.預設不記錄
                // 2.若有指定option attribute，則指定 isActiveLogResponseFullInfo=true 才會記錄
                if (LogOptions == null || (LogOptions != null && !LogOptions.IsRecord_ResponseFullInfo))
                {
                    log.RequestModel = string.Empty;
                    log.RequestHeaders = string.Empty;
                }

                #region Log ViewBag Response 
                if (viewResult != null && viewResult.ViewData != null)
                {
                    log.LastUpdate = string.Format("{0:u}", DateTime.Now);
                    log.ResponseData = viewResult.ViewData;
                    string responseLog = JsonConvert.SerializeObject(log);
                    string[] noLogRespPatterns = LogOptions?.NoLogRespPatterns ?? new string[] { };
                    foreach (string regex in noLogRespPatterns)
                    {
                        responseLog = Regex.Replace(responseLog, regex, (LogOptions?.ReplaceStr ?? regex));
                    }
                    _logger.LogInformation(responseLog);
                }
                #endregion

                #region Log Json Response 
                if (objResult != null && objResult.Value != null)
                {
                    log.LastUpdate = string.Format("{0:u}", DateTime.Now);
                    log.ResponseData = objResult.Value;
                    string responseLog = JsonConvert.SerializeObject(log);
                    string[] noLogRespPatterns = LogOptions?.NoLogRespPatterns ?? new string[] { };
                    foreach (string regex in noLogRespPatterns)
                    {
                        responseLog = Regex.Replace(responseLog, regex, (LogOptions?.ReplaceStr ?? regex));
                    }
                    _logger.LogInformation(responseLog);
                }
                #endregion
            }
        }
    }

}
