using CDD.Web.Helpers;
using CDD.Web.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace CDD.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseInfoController : ControllerBase
    {
        private readonly APIHelper _apiHelper;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="apiHelper">API 輔助工具</param>
        public CaseInfoController(
            APIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        /// <summary>
        /// 查詢盡職調查案件列表（Mock 資料）
        /// </summary>
        [HttpPost("Search")]
        public async Task<ApiResponse<List<CaseItem>>> Search([FromBody] SearchCasesReq req)
        {
            var mockItems = new List<CaseItem>
{
    new CaseItem
    {
        Id = "C001",
        FormId = "DD20250425014",
        Branch = "經紀本部",
        CustomerAccount = "尚未開戶",
        IdNumber = "A123456789",
        CustomerName = "王曉明",
        Salesperson = "陳靜香",
        ReviewStatus = "簽核中",
        Source = "證券開戶",
        EnhancedReview = "否",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "2025/08/04 18:40",
        CurrentProcessor = "林玉山",
        AmlStatus = "pending",
        B27Status = "pending",
        DetailsLink = "./CustomerDueDiligence_Review_1.html",
        Selectable = true,
        Selected = false
    },
    new CaseItem
    {
        Id = "C002",
        FormId = "DD20250425013",
        Branch = "經紀本部",
        CustomerAccount = "尚未開戶",
        IdNumber = "A123456790",
        CustomerName = "張曉明",
        Salesperson = "林大熊",
        ReviewStatus = "簽核中",
        Source = "證券開戶",
        EnhancedReview = "否",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "2025/08/04 18:40",
        CurrentProcessor = "林玉山",
        AmlStatus = "queried",
        B27Status = "queried",
        DetailsLink = "./CustomerDueDiligence_Review_1.html",
        Selectable = true,
        Selected = false
    },
    new CaseItem
    {
        Id = "C003",
        FormId = "DD20250425012",
        Branch = "經紀本部",
        CustomerAccount = "尚未開戶",
        IdNumber = "A123456791",
        CustomerName = "李曉明",
        Salesperson = "陳靜香",
        ReviewStatus = "簽核中",
        Source = "證券開戶",
        EnhancedReview = "否",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "2025/08/04 18:40",
        CurrentProcessor = "林玉山",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = "./CustomerDueDiligence_Review_1.html",
        Selectable = true,
        Selected = false
    },
    new CaseItem
    {
        Id = "C004",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801237",
        IdNumber = "A123456792",
        CustomerName = "林曉明",
        Salesperson = "林大熊",
        ReviewStatus = "未處理",
        Source = "期貨開戶",
        EnhancedReview = "",
        ApplicationDate = "2025/04/25",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C005",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801238",
        IdNumber = "A123456793",
        CustomerName = "王大明",
        Salesperson = "陳靜香",
        ReviewStatus = "未處理",
        Source = "期貨開戶",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C006",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801239",
        IdNumber = "A123456794",
        CustomerName = "張大明",
        Salesperson = "林大熊",
        ReviewStatus = "未處理",
        Source = "中風險定審",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C007",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801244",
        IdNumber = "A123456795",
        CustomerName = "陳曉",
        Salesperson = "陳靜香",
        ReviewStatus = "未處理",
        Source = "高風險定審",
        EnhancedReview = "是",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C008",
        FormId = "DD20250425003",
        Branch = "經紀本部",
        CustomerAccount = "9801245",
        IdNumber = "A123456796",
        CustomerName = "王明月",
        Salesperson = "林大熊",
        ReviewStatus = "簽核中",
        Source = "高風險定審",
        EnhancedReview = "是",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = "./CustomerDueDiligence_Review_2.html",
        Selectable = true,
        Selected = false
    },
    new CaseItem
    {
        Id = "C009",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801246",
        IdNumber = "A123456797",
        CustomerName = "王曉花",
        Salesperson = "陳靜香",
        ReviewStatus = "未處理",
        Source = "未成年轉正",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C010",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801247",
        IdNumber = "A123456798",
        CustomerName = "王OO",
        Salesperson = "林大熊",
        ReviewStatus = "未處理",
        Source = "期貨開戶",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C011",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801248",
        IdNumber = "A123456799",
        CustomerName = "李OO",
        Salesperson = "陳靜香",
        ReviewStatus = "未處理",
        Source = "信用開戶",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C012",
        FormId = null,
        Branch = "經紀本部",
        CustomerAccount = "9801249",
        IdNumber = "A123456800",
        CustomerName = "陳OO",
        Salesperson = "林大熊",
        ReviewStatus = "未處理",
        Source = "信用開戶",
        EnhancedReview = "",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = null,
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C013",
        FormId = "DD20250425002",
        Branch = "經紀本部",
        CustomerAccount = "尚未開戶",
        IdNumber = "A123456798",
        CustomerName = "張OO",
        Salesperson = "陳靜香",
        ReviewStatus = "完成",
        Source = "期貨開戶",
        EnhancedReview = "否",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "2025/08/01 19:45",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = "./CustomerDueDiligence_Review_1.html",
        Selectable = false,
        Selected = null
    },
    new CaseItem
    {
        Id = "C014",
        FormId = "DD20250425001",
        Branch = "經紀本部",
        CustomerAccount = "尚未開戶",
        IdNumber = "A123456798",
        CustomerName = "王OO",
        Salesperson = "林大熊",
        ReviewStatus = "完成",
        Source = "期貨開戶",
        EnhancedReview = "否",
        ApplicationDate = "2025/08/01",
        LastModifiedTime = "2025/08/01 19:45",
        CurrentProcessor = "-",
        AmlStatus = "pending",
        B27Status = "queried",
        DetailsLink = "./CustomerDueDiligence_Review_1.html",
        Selectable = false,
        Selected = null
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


        /// <summary>
        /// 前端呼叫：建立表單流程
        /// </summary>
        [HttpPost("CreateForm")]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormReq req)
        {
            // 呼叫 APIHelper 幫你轉呼叫 apiweb
            var apiResp = await _apiHelper.CallCreateFormApi(req);

            // 只回傳 message
            return Ok(new { message = apiResp?.Message });
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

    /// <summary>
    /// 案件查詢的請求模型 (Request DTO)
    /// </summary>
    public class SearchCasesReq
    {
        [JsonPropertyName("branch")]
        public string? Branch { get; set; }

        [JsonPropertyName("specialist")]
        public string? Specialist { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("idNumber")]
        public string? IdNumber { get; set; }

        [JsonPropertyName("dateFrom")]
        public string? DateFrom { get; set; }

        [JsonPropertyName("dateTo")]
        public string? DateTo { get; set; }

        [JsonPropertyName("reviewStatus")]
        public string? ReviewStatus { get; set; }

        [JsonPropertyName("processor")]
        public string? Processor { get; set; }

        [JsonPropertyName("enhancedReview")]
        public string? EnhancedReview { get; set; }

        [JsonPropertyName("formId")]
        public string? FormId { get; set; }
    }
}
