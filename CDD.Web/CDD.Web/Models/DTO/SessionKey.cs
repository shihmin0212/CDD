namespace CDD.Web.Models.DTO
{
    public class SessionKey
    {
        /// <summary>
        /// Session Cookie Name
        /// </summary>
        public static string SessionCookieName { get; } = ".CDD.Web";

        /// <summary>
        /// Session Cookie CreateTime Name
        /// </summary>
        public static string SessionCookieCreateTime { get; } = ".CDD.SessionCookieCreateTime";

        /// <summary>
        /// 取得 Service Alert Resp
        /// </summary>
        public static string ServiceAlertResp { get; } = ".CDD.ServiceAlertResp";

        /// <summary>
        /// 確認 Service Alert 訊息提示
        /// </summary>
        public static string ConfirmServiceAlert { get; } = ".CDD.ConfirmServiceAlert";

        /// <summary>
        /// exception store in session
        /// </summary>
        public static string ExceptionGeneralResp = ".CDD.ExceptionGeneralResp";

        /// <summary>
        /// last 圖形驗證碼
        /// </summary>
        public static string CaptchaCode { get; } = ".CDD.CaptchaCode";

        /// <summary>
        /// 使用者登入 資訊
        /// </summary>
        public static string UserLoginData { get; } = ".CDD.UserLoginData";
    }

}
