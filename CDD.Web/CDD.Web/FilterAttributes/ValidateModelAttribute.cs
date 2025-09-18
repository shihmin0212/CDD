using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using CDD.Web.Libs;

namespace CDD.Web.FilterAttributes
{
    /// <summary>
    /// ModelState 驗證
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ValidateModelAttribute> _logger;

        public ValidateModelAttribute(ILogger<ValidateModelAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
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
}
