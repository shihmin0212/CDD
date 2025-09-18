using System.ComponentModel.DataAnnotations;

namespace CDD.Web.Models.Request.System
{
    /// <summary>
    /// 系統管理查詢請求
    /// </summary>
    public class SystemViewDataReq
    {
        /// <summary>
        /// 搜尋關鍵字
        /// </summary>
        [StringLength(100, ErrorMessage = "搜尋關鍵字長度不可超過100字元")]
        public string? SystemName { get; set; }

        /// <summary>
        /// 頁碼
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "頁碼必須大於0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每頁數量
        /// </summary>
        [Range(1, 1000, ErrorMessage = "每頁數量必須介於1到1000之間")]
        public int PageSize { get; set; } = 10;
    }
}
