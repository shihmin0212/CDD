using System.ComponentModel.DataAnnotations;

namespace CDD.Api.Models.Request.Admin.System
{
    /// <summary>
    /// 停用系統 Req
    /// </summary>
    public class SystemDisableSwitchingReq
    {
        /// <summary>
        /// 系統 ex.OOA
        /// </summary>
        [StringLength(100, MinimumLength = 0, ErrorMessage = "業務別 只能輸入{1}~{2}個字")]
        [Required(ErrorMessage = "欲停用系統 為必填")]
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// 啟用/停用
        /// </summary>
        [Required(ErrorMessage = "啟用/停用 為必填")]
        public bool IsActive { get; set; } = false;
    }
}
