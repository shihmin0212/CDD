using System.ComponentModel.DataAnnotations;

namespace Sample.Api.Models.Request.Admin.System
{
    public class SystemCreateReq
    {
        [Required()]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string SystemName { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public Guid? ApiKey { get; set; }

        [StringLength(32, MinimumLength = 0, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string? HashKey { get; set; }

        [StringLength(16, MinimumLength = 0, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string? IVKey { get; set; }

    }
}
