using System.ComponentModel.DataAnnotations;

namespace CDD.Web.Models.Request.System
{
    /// <summary>
    /// 產生系統 Token 請求
    /// </summary>
    public class GenerateSystemTokenReq
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [Required(ErrorMessage = "系統代號不可為空")]
        [StringLength(50, ErrorMessage = "系統代號長度不可超過50字元")]
        public string System { get; set; }

        /// <summary>
        /// ApiKey
        /// </summary>
        [Required(ErrorMessage = "來源識別碼不可為空")]
        public Guid ApiKey { get; set; }

        /// <summary>
        /// HashKey
        /// </summary>
        [Required(ErrorMessage = "HashKey")]
        public string HashKey { get; set; }


        /// <summary>
        /// 初始化向量金鑰
        /// </summary>
        [Required(ErrorMessage = "IVKEy")]
        public string IVKEy { get; set; }
    }
}
