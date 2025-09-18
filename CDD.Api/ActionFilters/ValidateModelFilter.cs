using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Sample.Api.Libs;

namespace Sample.Api.ActionFilters
{
    /// <summary>
    /// ModelState 驗證
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ValidateModelAttribute> _logger;

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="logger"></param>
        public ValidateModelAttribute(ILogger<ValidateModelAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 執行參數驗證
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            IgnoreValidateModelAttribute? ignoreAction = (context.ActionDescriptor as ControllerActionDescriptor)?
                .MethodInfo.GetCustomAttributes<IgnoreValidateModelAttribute>().FirstOrDefault();

            if (ignoreAction == null && !context.ModelState.IsValid)
            {
                // 取得 Model 錯誤資訊
                var modelState = context.ModelState;
                string route = context.HttpContext.Request.PathBase.ToString() + context.HttpContext.Request.Path.ToString();
                string modelStateError = string.Join
                (
                    "; ",
                    modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                );
                string attemptedValue = string.Join(
                    "",
                    modelState.Keys.SelectMany(x => $"{x}:{modelState?[x]?.AttemptedValue};")
                );
                //string ErrorMesg = $"action={route},error=modelStateError:{modelStateError},attempt={attemptedValue}";

                ControllerActionDescriptor descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                string controllerName = descriptor.ControllerName;
                string actionName = descriptor.ActionName;

                ModelBindError bindError = new ModelBindError();
                bindError.ControllerName = controllerName;
                bindError.ActionName = actionName;
                bindError.ErrorMessage = modelStateError;
                bindError.AttemptedValue = attemptedValue;

                throw MyExceptionList.ModelBindingError(bindError).GetException();
            }
        }

    }

    public class ModelBindError
    {
        public string? ControllerName;

        public string? ActionName;

        public string? ErrorMessage;

        public string? AttemptedValue;
    }

    public class IgnoreValidateModelAttribute : Attribute
    {

    }

}
