using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using CDD.Web.Libs;
using CDD.Web.Services;

namespace CDD.Web.FilterAttributes
{
    /// <summary>
    /// 登入attribute
    /// </summary>
    public class RequireLogin : Attribute
    {
    }

    public class MyAllowAnonymous : Attribute
    {

    }

    public class LoginAuthenticationFilter : ActionFilterAttribute
    {
        private readonly ILogger<LoginAuthenticationFilter> _logger;

        private readonly IHttpContextAccessor contextAccessor;

        private readonly ConfigurationSection _WebSetting;

        private HttpContext httpContext
        {
            get
            {
                return contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor.HttpContext));
            }
        }

        private readonly UserService _userServie;

        public LoginAuthenticationFilter(
            IConfiguration config,
            ILogger<LoginAuthenticationFilter> logger, IHttpContextAccessor accessor,
            UserService userService)
        {
            this._logger = logger;
            this.contextAccessor = accessor;
            this._userServie = userService;
            this._WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            RequireLogin? _requireLoginController = (context.ActionDescriptor as ControllerActionDescriptor)?
                .ControllerTypeInfo.GetCustomAttributes<RequireLogin>().FirstOrDefault();

            RequireLogin? _requireLoginMethod = (context.ActionDescriptor as ControllerActionDescriptor)?
                .MethodInfo.GetCustomAttributes<RequireLogin>().FirstOrDefault();

            MyAllowAnonymous? _allowAnonymous = (context.ActionDescriptor as ControllerActionDescriptor)?
               .MethodInfo.GetCustomAttributes<MyAllowAnonymous>().FirstOrDefault();

            // 登入檢查
            if (_allowAnonymous == null && (_requireLoginController != null || _requireLoginMethod != null))
            {
                if (this._userServie.IsLogin() == false)
                {

                    throw MyExceptionList.SessionExpired().GetException();
                }
            }
            // 繼續執行 pipline 
            ActionExecutedContext actionExecutedContext = await next();

        }
    }
}
