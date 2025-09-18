using System.ComponentModel.DataAnnotations;

namespace CDD.Web.Models.Request.System
{
    /// <summary>
    /// 更新系統請求
    /// </summary>
    public class UpdateSystemReq
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [Required(ErrorMessage = "系統代號不可為空")]
        [StringLength(50, ErrorMessage = "系統代號長度不可超過50字元")]
        public string SystemName { get; set; } = string.Empty;

        /// <summary>
        /// API 金鑰
        /// </summary>
        [Required(ErrorMessage = "API金鑰不可為空")]
        public Guid ApiKey { get; set; }

        /// <summary>
        /// 雜湊金鑰
        /// </summary>
        [Required(ErrorMessage = "雜湊金鑰不可為空")]
        [StringLength(200, ErrorMessage = "雜湊金鑰長度不可超過200字元")]
        public string HashKey { get; set; } = string.Empty;

        /// <summary>
        /// 初始化向量金鑰
        /// </summary>
        [Required(ErrorMessage = "初始化向量金鑰不可為空")]
        [StringLength(200, ErrorMessage = "初始化向量金鑰長度不可超過200字元")]
        public string IVKey { get; set; } = string.Empty;

        /// <summary>
        /// 啟用狀態
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

}
