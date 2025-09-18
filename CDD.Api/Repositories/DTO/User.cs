namespace CDD.Api.Repositories.DTO
{
    /// <summary>
    /// 使用者資料表 
    /// </summary>
    public class User
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

        /// <summary>
        /// 雜湊後密碼(Token) 驗算後再存入欄位，用CDD系統 sha(Base64PasswordSignature) 成Token 給外部系統留存 往後比對
        /// </summary>
        public string Base64PasswordSignature { get; set; } = null!;

        /// <summary>
        /// hash Salt 
        /// </summary>
        public string Base64Salt { get; set; } = null!;

        /// <summary>
        /// hash Pepper 
        /// </summary>
        public string Pepper { get; set; } = null!;

        public DateTime CreatedTime { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
