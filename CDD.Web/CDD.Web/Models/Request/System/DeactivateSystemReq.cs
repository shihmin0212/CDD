using System.ComponentModel.DataAnnotations;

namespace CDD.Web.Models.Request.System
{
    /// <summary>
    /// 停用系統請求
    /// </summary>
    public class DeactivateSystemReq
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        [Required(ErrorMessage = "系統代號不可為空")]
        [StringLength(50, ErrorMessage = "系統代號長度不可超過50字元")]
        public string System { get; set; } = string.Empty;

        /// <summary>
        /// 停用/啟用狀態
        /// </summary>
        public bool IsActive { get; set; } = false;
    }

}
