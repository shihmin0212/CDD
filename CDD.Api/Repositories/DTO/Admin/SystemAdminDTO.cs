namespace Sample.Api.Repositories.DTO.Admin
{
    /// <summary>
    /// 業務端 外部系統 串接資訊
    /// </summary>
    public class SystemAdminDTO
    {
        /// <summary>
        /// 系統代號
        /// </summary>
        public string System { get; set; } = null!;

        /// <summary>
        /// ApiKey
        /// </summary>
        public Guid ApiKey { get; set; }

        /// <summary>
        /// AES加密 HashKey
        /// </summary>
        public string HashKey { get; set; } = null!;

        /// <summary>
        /// AES加密 IVKey
        /// </summary>
        public string IVKey { get; set; } = null!;

        /// <summary>
        /// 是否啟用 Y/N
        /// </summary>
        public string IsActive { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CreatedTime { get; set; } = null;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? LastUpdate { get; set; } = null;

        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { get; set; } = 0;
    }
}
