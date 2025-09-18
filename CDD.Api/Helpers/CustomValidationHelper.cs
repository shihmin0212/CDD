using System.ComponentModel.DataAnnotations;
using CDD.Api.ActionFilters;
using CDD.Api.Libs;

namespace CDD.Api.Helpers
{
    public class CustomValidationHelper
    {
        /// <summary>
        /// 
        /// TODO : MVC 手動驗證model binding參數 
        /// Invoke model validation manually
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="checkObj"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool ValidateObject<T>(T checkObj, string controllerName, string actionName)
        {
            /*
            /// string jsonStrData =  CommonHelper.DecodeBase64(base64Req.base64Data);
            /// FillDataBasicReq req = JsonConvert.DeserializeObject<FillDataBasicReq>(jsonStrData) ?? throw new ArgumentNullException("FillDataBasicReq Decode return null");
            /// string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString() ?? String.Empty;
            /// string actionName = ControllerContext.ActionDescriptor.ActionName;
            /// CustomValidationHelper.ValidateObject(req, controllerName, actionName);
             */
            if (checkObj == null) { throw new ArgumentNullException(typeof(T).ToString() + "Is Null"); }
            // invoke model validation
            ValidationContext context = new ValidationContext(checkObj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(checkObj, context, validationResults, true);
            if (!isValid)
            {
                //List of errors 
                validationResults.Select(r => r.ErrorMessage);
                string modelStateError = string.Join
                (
                    "; ",
                    validationResults.Select(x => x.ErrorMessage)
                );
                ModelBindError bindError = new ModelBindError();
                // ControllerContext.RouteData.Values["controller"]?.ToString();
                bindError.ControllerName = controllerName;
                //ControllerContext.ActionDescriptor.ActionName;
                bindError.ActionName = actionName;
                bindError.ErrorMessage = modelStateError;
                bindError.AttemptedValue = String.Empty;

                throw MyExceptionList.ModelBindingError(bindError).GetException();
            }
            return isValid;
        }

        /// <summary>
        /// 
        /// TODO : MVC 手動驗證model binding參數 
        /// Invoke model validation manually
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="checkObj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Tuple<bool, string?> ValidateObject<T>(T checkObj)
        {
            /*
            /// string jsonStrData =  CommonHelper.DecodeBase64(base64Req.base64Data);
            /// FillDataBasicReq req = JsonConvert.DeserializeObject<FillDataBasicReq>(jsonStrData) ?? throw new ArgumentNullException("FillDataBasicReq Decode return null");
            /// CustomValidationHelper.ValidateObject(req);
             */
            if (checkObj == null) { throw new ArgumentNullException(typeof(T).ToString() + "Is Null"); }
            // invoke model validation
            ValidationContext context = new ValidationContext(checkObj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(checkObj, context, validationResults, true);
            string? modelStateError = null;
            if (!isValid)
            {
                //List of errors 
                validationResults.Select(r => r.ErrorMessage);
                modelStateError = string.Join
                (
                    "; ",
                    validationResults.Select(x => x.ErrorMessage)
                );
            }
            return new Tuple<bool, string?>(isValid, modelStateError);
        }
    }
}
