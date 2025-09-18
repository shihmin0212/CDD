using CDD.Web.FilterAttributes;

namespace CDD.Web.Libs
{
    public sealed class MyExceptionList
    {
        private static readonly string GeneralErrorTitle = "系統發生錯誤";
        private static readonly string GeneralErrorMsg = "很抱歉，目前網站忙碌中，請稍候重新進入線上服務，造成不便敬請見諒。";

        /// <summary>
        /// 服務維護中
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage ServiceMaintaining(string? title = null, string? message = null, object? debug = null)
        {
            return new MyExceptionMessage(0, title ?? "系統維護暫停登入", message ?? "親愛的顧客您好，為提供您更好的服務，系統暫停登入，造成您的不便，敬請見諒。", debug);
        }

        /// <summary>
        /// 取得service alert 錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage GetServiceAlertError(object? debug = null)
        {
            return new MyExceptionMessage(1, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 參數係結節錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static MyExceptionMessage ModelBindingError(ModelBindError? debug = null, string? title = null)
        {
            return new MyExceptionMessage(2, title ?? GeneralErrorTitle, debug?.ErrorMessage ?? GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 驗證碼錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage CaptchaError(object? debug = null)
        {
            return new MyExceptionMessage(3, "驗證碼錯誤", "很抱歉，驗證碼錯誤，請重新輸入。", debug);
        }

        /// <summary>
        /// 顧客操作太久作業逾時
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SessionExpired(object? debug = null)
        {
            return new MyExceptionMessage(4, "系統連線失敗", "很抱歉，作業已逾時，請重新登入，造成不便敬請見諒。", debug);
        }

        /// <summary>
        /// 步驟錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage IllegalFlow(object? debug = null)
        {
            return new MyExceptionMessage(5, GeneralErrorTitle, GeneralErrorMsg, debug);
        }


        /// <summary>
        /// 11.	API 打不通
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage HttpError(object? debug = null)
        {
            return new MyExceptionMessage(6, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 用戶斷線錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage TaskCanceledError(object? debug = null)
        {
            return new MyExceptionMessage(7, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 未預期錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage Unknown(object? debug = null)
        {
            return new MyExceptionMessage(8, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 取得 分公司資訊錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage VerifyAdminUserError(object? debug = null)
        {
            return new MyExceptionMessage(9, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 帳號或密碼無法登入 錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage Unauthorized(object? debug = null)
        {
            return new MyExceptionMessage(10, "錯誤", "帳號或密碼無法登入，請重新輸入。", debug);
        }



        /// <summary>
        /// Over RequestSize limit 
        /// </summary>
        /// <returns></returns>
        public static MyExceptionMessage PayloadTooLargeError(object? debug = null)
        {
            return new MyExceptionMessage(90, "錯誤", "很抱歉，您上傳的資料超過上串大小限制，請重新執行操作，造成不便敬請見諒。", debug);
        }

        /// <summary>
        /// ip 解析錯誤
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage GetClientIPError(object? debug = null)
        {
            return new MyExceptionMessage(91, GeneralErrorTitle, GeneralErrorMsg, debug);
        }

        /// <summary>
        /// 無通過otp驗證 跳頁觸發
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage AccessDenined(object? debug = null)
        {
            return new MyExceptionMessage(99, "系統連線失敗", "很抱歉，申請作業已逾時，請重新進入線購服務，造成不便敬請見諒。", debug);
        }

        /// <summary>
        /// 資源不存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SourceNotFoundError(object? debug = null)
        {
            return new MyExceptionMessage(404, "資源不存在", "很抱歉，資源不存在，請重新進入線上服務，造成不便敬請見諒。", debug);
        }


        /// <summary>
        /// 業務別管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessApiError(object? debug = null)
        {
            return new MyExceptionMessage(1002, "API呼叫失敗", "業務別管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 業務別不存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessNotFoundError(object? debug = null)
        {
            return new MyExceptionMessage(1003, "業務別不存在", "指定的業務別不存在，請確認業務別ID。", debug);
        }

        /// <summary>
        /// 業務別已存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessAlreadyExistsError(object? debug = null)
        {
            return new MyExceptionMessage(1004, "業務別已存在", "該業務別已存在，不可重複新增。", debug);
        }

        /// <summary>
        /// 業務別操作權限不足
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessPermissionDeniedError(object? debug = null)
        {
            return new MyExceptionMessage(1005, "權限不足", "您沒有權限執行此業務別操作。", debug);
        }


        /// <summary>
        /// 業務別授權管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessAllowApiError(object? debug = null)
        {
            return new MyExceptionMessage(1007, "API呼叫失敗", "業務別授權管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 業務別授權已存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessAllowAlreadyExistsError(object? debug = null)
        {
            return new MyExceptionMessage(1008, "授權已存在", "該業務別授權已存在，不可重複新增。", debug);
        }

        /// <summary>
        /// 業務別授權不存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage BusinessAllowNotFoundError(object? debug = null)
        {
            return new MyExceptionMessage(1009, "授權不存在", "指定的業務別授權不存在。", debug);
        }


        /// <summary>
        /// 系統管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemApiError(object? debug = null)
        {
            return new MyExceptionMessage(1011, "API呼叫失敗", "系統管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 系統不存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemNotFoundError(object? debug = null)
        {
            return new MyExceptionMessage(1012, "系統不存在", "指定的系統不存在，請確認系統代號。", debug);
        }

        /// <summary>
        /// 系統已存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemAlreadyExistsError(object? debug = null)
        {
            return new MyExceptionMessage(1013, "系統已存在", "該系統已存在，不可重複新增。", debug);
        }

        /// <summary>
        /// 系統操作權限不足
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemPermissionDeniedError(object? debug = null)
        {
            return new MyExceptionMessage(1014, "權限不足", "您沒有權限執行此系統操作。", debug);
        }

        /// <summary>
        /// 系統 Token 產生失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemTokenGenerationError(object? debug = null)
        {
            return new MyExceptionMessage(1015, "Token產生失敗", "系統Token產生失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 系統加密解密失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage SystemEncryptionError(object? debug = null)
        {
            return new MyExceptionMessage(1016, "加密解密失敗", "系統加密解密處理失敗，請確認密鑰設定。", debug);
        }

        /// <summary>
        /// 文件類型管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage DocTypeManagementApiError(object? debug = null)
        {
            return new MyExceptionMessage(1017, "API呼叫失敗", "文件類型管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 文件類型不存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage DocTypeNotFoundError(object? debug = null)
        {
            return new MyExceptionMessage(1018, "文件類型不存在", "指定的文件類型不存在，請確認文件類型ID。", debug);
        }

        /// <summary>
        /// 文件類型已存在
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage DocTypeAlreadyExistsError(object? debug = null)
        {
            return new MyExceptionMessage(1019, "文件類型已存在", "該文件類型已存在，不可重複新增。", debug);
        }

        /// <summary>
        /// 文件類型操作權限不足
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage DocTypePermissionDeniedError(object? debug = null)
        {
            return new MyExceptionMessage(1020, "權限不足", "您沒有權限執行此文件類型操作。", debug);
        }


        public static MyExceptionMessage FileInfosManagementApiError(object? debug = null)
        {
            return new MyExceptionMessage(1021, "API呼叫失敗", "檔案案件管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 檔案案件管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage FileCaseManagementApiError(object? debug = null)
        {
            return new MyExceptionMessage(1021, "API呼叫失敗", "檔案案件管理API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 檔案案件權限白名單管理 管理API呼叫失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage FileCasePermissionWhiteListApiError(object? debug = null)
        {
            return new MyExceptionMessage(1022, "API呼叫失敗", "檔案案件權限白名單管理 API呼叫失敗，請稍後再試。", debug);
        }

        /// <summary>
        /// 檔案案件預覽 失敗
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MyExceptionMessage FileInfosGetContentError(object? debug = null)
        {
            return new MyExceptionMessage(1023, "API呼叫失敗", "檔案案件預覽 失敗，請稍後再試。", debug);
        }
    }

}
