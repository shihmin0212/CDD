namespace CDD.Api.Libs
{
    public sealed class MyExceptionList
    {
        private static readonly string GeneralErrorTitle = "系統發生非預期錯誤";
        private static readonly string GeneralErrorMsg = "很抱歉，系統發生非預期錯誤，請重新進入服務，造成不便敬請見諒。";

        /// <summary>
        /// 參數係結節錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage ModelBindingError(CDD.Api.ActionFilters.ModelBindError? debug = null)
        {
            return new MyExceptionMessage(0, GeneralErrorTitle, debug?.ErrorMessage ?? GeneralErrorMsg, debug);
        }

        public static MyExceptionMessage ModelBindingError(string errorMsg, object? debug = null)
        {
            return new MyExceptionMessage(0, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage CallAPIError(object? debug = null)
        {
            return new MyExceptionMessage(1, GeneralErrorTitle, $"CallAPIError", debug);
        }

        public static MyExceptionMessage UnknownError(string? debug = null)
        {
            return new MyExceptionMessage(2, GeneralErrorTitle, debug ?? GeneralErrorMsg, debug);
        }

        public static MyExceptionMessage HttpError(object? debug = null)
        {
            return new MyExceptionMessage(3, GeneralErrorTitle, "取得網路資料錯誤 HttpError", debug);
        }

        public static MyExceptionMessage GetClientIPError(object? debug = null)
        {
            return new MyExceptionMessage(4, GeneralErrorTitle, "取得IP失敗", debug);
        }

        public static MyExceptionMessage InValidData(string errorMsg, object? debug = null)
        {
            return new MyExceptionMessage(5, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage InValidActionResult(string errorMsg, object? debug = null)
        {
            return new MyExceptionMessage(6, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage IsInValidApiKeyPattern(object? debug = null)
        {
            return new MyExceptionMessage(7, GeneralErrorTitle, "api key 格式錯誤", debug);
        }

        public static MyExceptionMessage UserAlreadyExists(object? debug = null)
        {
            return new MyExceptionMessage(8, GeneralErrorTitle, "用戶已經存在", debug);
        }

        public static MyExceptionMessage CheckOrCreateFileFolder(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(9, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage EncryptBase64ToFileError(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(10, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage DecryptFileToBase64Error(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(11, GeneralErrorTitle, errorMsg, debug);
        }


        public static MyExceptionMessage GetImageTypeError(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(12, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage UploadFileError(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(13, GeneralErrorTitle, errorMsg, debug);
        }

        public static MyExceptionMessage BusinessUnauthorized(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(14, GeneralErrorTitle, $"Business 未授權" + errorMsg, debug);
        }

        public static MyExceptionMessage DeleteFileError(string errorMsg = "", object? debug = null)
        {
            return new MyExceptionMessage(15, GeneralErrorTitle, $"檔案刪除發生錯誤:" + errorMsg, debug);
        }

        public static MyExceptionMessage Unauthorized(object? debug = null)
        {
            return new MyExceptionMessage(401, GeneralErrorTitle, "未授權", debug);
        }

        public static MyExceptionMessage Forbidden(object? debug = null)
        {
            return new MyExceptionMessage(403, GeneralErrorTitle, "無權限禁止訪問", debug);
        }
    }
}
