using System.ComponentModel.DataAnnotations;

namespace Sample.Api.Models.Request.Admin
{
    public class LoginReq
    {
        /// <summary>
        /// 加密後的 帳號
        /// </summary>
        [Required()]
        public string Account { get; set; } = null!;

        /// <summary>
        /// 加密後的 密碼
        /// </summary>
        [Required()]
        public string Password { get; set; } = null!;
    }
}
