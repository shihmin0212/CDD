using Microsoft.AspNetCore.Mvc;

namespace CDD.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DueDiligenceController : ControllerBase
    {
        /// <summary>
        /// 查詢盡職調查案件列表（Mock 資料）
        /// </summary>
        [HttpPost("Search")]
        public async Task<ApiResponse<List<CaseItem>>> Search([FromBody] SearchFilters filters)
        {
            var mockItems = new List<CaseItem>
            {
                new CaseItem
                {
                    Id = "C001",
                    FormId = "F001",
                    Branch = "台北分行~",
                    CustomerAccount = "1234567890",
                    IdNumber = "A123456789",
                    CustomerName = "王小明",
                    Salesperson = "李業務",
                    ReviewStatus = "待審核",
                    Source = "網銀",
                    EnhancedReview = "是",
                    ApplicationDate = "2025-09-01",
                    LastModifiedTime = "2025-09-17 10:00:00",
                    CurrentProcessor = "張處理人",
                    AmlStatus = "pending",
                    B27Status = "queried",
                    DetailsLink = null,
                    Selectable = true,
                    Selected = false
                },
                new CaseItem
                {
                    Id = "C002",
                    FormId = "F002",
                    Branch = "高雄分行",
                    CustomerAccount = "9876543210",
                    IdNumber = "B987654321",
                    CustomerName = "陳美麗",
                    Salesperson = "王業務",
                    ReviewStatus = "已審核",
                    Source = "臨櫃",
                    EnhancedReview = "否",
                    ApplicationDate = "2025-08-25",
                    LastModifiedTime = "2025-09-16 15:30:00",
                    CurrentProcessor = "林處理人",
                    AmlStatus = "queried",
                    B27Status = "pending",
                    DetailsLink = "https://example.com/details/C002",
                    Selectable = true,
                    Selected = true
                }
            };

            var response = new ApiResponse<List<CaseItem>>
            {
                Status = true,
                Data = mockItems,
                Message = "成功取得 mock 案件資料"
            };

            return await Task.FromResult(response);
        }
    }

    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
    }

    public class CaseItem
    {
        public string Id { get; set; }
        public string FormId { get; set; }
        public string Branch { get; set; }
        public string CustomerAccount { get; set; }
        public string IdNumber { get; set; }
        public string CustomerName { get; set; }
        public string Salesperson { get; set; }
        public string ReviewStatus { get; set; }
        public string Source { get; set; }
        public string EnhancedReview { get; set; } // "是" | "否" | ""
        public string ApplicationDate { get; set; }
        public string LastModifiedTime { get; set; }
        public string CurrentProcessor { get; set; }
        public string AmlStatus { get; set; } // "queried" | "pending"
        public string B27Status { get; set; } // "queried" | "pending"
        public string? DetailsLink { get; set; }
        public bool Selectable { get; set; }
        public bool? Selected { get; set; }
    }

    public class SearchFilters
    {
        public string Branch { get; set; }
        public string Specialist { get; set; }
        public string Source { get; set; }
        public string AccountNumber { get; set; }
        public string IdNumber { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string ReviewStatus { get; set; }
        public string Processor { get; set; }
        public string EnhancedReview { get; set; } // "all" | "yes" | "no"
        public string FormId { get; set; }
    }
}
