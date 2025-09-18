using System.ComponentModel.DataAnnotations;

namespace CDD.Api.Models.Request.Admin
{
    /// <summary>
    /// 建立用戶
    /// </summary>
    public class CreateUserReq
    {
        [Required()]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "The {0} value cannot exceed {1}~{2} characters. ")]
        public string EmployeeNo { get; set; } = null!;

        [Required()]
        [EmailAddress()]
        public string Email { get; set; } = null!;

        [Required()]
        public string Password { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public bool IsAdminUser { get; set; } = false;
    }
}
