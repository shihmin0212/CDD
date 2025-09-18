namespace Sample.Api.Repositories.DTO
{
    /// <summary>
    /// 業務端 外部系統 串接資訊
    /// </summary>
    public class SystemDTO
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
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 只有appsetting.json 才有這個欄位
        /// </summary>
        public bool IsAdmin { get; set; } = false; // 是否為管理員系統

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CreatedTime { get; set; } = null;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? LastUpdate { get; set; } = null;
    }
}
