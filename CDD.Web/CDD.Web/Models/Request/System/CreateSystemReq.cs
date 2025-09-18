using System.ComponentModel.DataAnnotations;

namespace CDD.Web.Models.Request.System
{
    /// <summary>
    /// 新增系統請求
    /// </summary>
    public class CreateSystemReq
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [Required(ErrorMessage = "系統代號不可為空")]
        [StringLength(25, ErrorMessage = "系統代號長度不可超過50字元")]
        public string SystemName { get; set; } = string.Empty;

        /// <summary>
        /// API 金鑰
        /// </summary> 
        public Guid? ApiKey { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
