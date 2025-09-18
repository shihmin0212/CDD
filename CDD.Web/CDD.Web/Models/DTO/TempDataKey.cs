namespace CDD.Web.Models.DTO
{
    public class TempDataKey
    {
        /// <summary>
        /// Moauth 交易識別碼
        /// </summary>
        public static string OauthTxID { get; } = "OauthTxID";


        #region 交割帳號綁定 錯誤訊息key
        public static string BankAccountBindErrorMsg { get; } = "BankAccountBindErrorMsg";
        #endregion
    }
}
