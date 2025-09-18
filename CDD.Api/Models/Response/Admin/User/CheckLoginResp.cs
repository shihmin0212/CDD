namespace CDD.Api.Models.Response.Admin.User
{
    /// <summary>
    /// 登入
    /// </summary>
    public class CheckLoginResp : GeneralResp<object>
    {
        public UserSummary? UserInfo { get; set; }
    }

    public class UserSummary
    {
        /// <summary>
        /// 員編
        /// </summary>
        public string EmployeeNo { get; set; } = null!;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// 是否admin user
        /// </summary>
        public bool IsAdminUser { get; set; } = false;

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 是否刪除:1刪除
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedTime { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
