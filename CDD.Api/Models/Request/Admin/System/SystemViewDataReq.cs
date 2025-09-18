using System.ComponentModel.DataAnnotations;

namespace CDD.Api.Models.Request.Admin.System
{
    public class SystemViewDataReq
    {
        [StringLength(25, MinimumLength = 0, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string? SystemName { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

    }
}
