using Azure;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.API.Models.Response;
using CDD.API.Services.Interfaces;
using SurveryProject.Service.Base;
using System.ComponentModel;

namespace CDD.Api.Services
{
    public class AOAFlowStageService
    {
        private readonly ILogger<FlowStageService> _logger;
        private readonly IConfiguration _config;
        private readonly IRequest _request;
        private readonly HTTPRequests _httpRequests;

        // 將 BaseUrl 正規化（去掉尾斜線），避免組出 // 的路徑
        private readonly string _baseUrl;

        public AOAFlowStageService(
            ILogger<FlowStageService> logger,
            IRequest request,
            IConfiguration config,
            HTTPRequests httpRequests)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpRequests = httpRequests;
        }


        /// <summary>
        /// 取得表單目前狀態
        /// </summary>
        /// <param name="formNum">單據編號</param>
        /// <returns>Flow Status</returns>
        public FlowStatus GetFlowStatus(string formNum)
        {
            string url = _httpRequests.GetFlowStatus(formNum);
            var response = _request.GetJSON<ResponseFlowStatus>(url, headers: null).Result;
            if (response.IsSuccess)
            {
                return response.FlowStatus;
            }

            throw new Exception($"Serveice FlowStage Error,GetFlowStatus : {response.IsSuccess} {string.Join(",", response?.ValidationMsg)}");
        }

        /// <summary>
        /// 取得單據的所有簽核關卡及簽核歷程
        /// </summary>
        /// <param name="formNum">單據編號</param>
        /// <returns>簽核鏈</returns>
        public ResponseProcessStages GetProcessStatus(string formNum)
        {
            string url = _httpRequests.GetProcessStatus(formNum);

            var response = _request.GetJSON<ResponseProcessStages>(url, headers: null).Result;
            if (response.IsSuccess)
            {
                return response;
            }

            throw new Exception($"Service FlowStage Error,GetProcessStatus: {response.IsSuccess} {string.Join(",", response?.ValidationMsg)}");
        }
    }
}



public abstract class ResponseFlowBase
{
    public bool IsSuccess { get; set; }

    public List<string> ValidationMsg { get; set; } = new List<string>();

}

public class ResponseFlowStatus : ResponseFlowBase
{
    public FlowStatus FlowStatus { get; set; } = new FlowStatus();

}




public enum EnumProcessStatus
{
    [Description("待處理")]
    Pending,
    [Description("處理中")]
    Current,
    [Description("加會中")]
    CounterSigned,
    [Description("已結束")]
    Ending
}
public class FlowProcessStage
{
    public int Index { get; set; }

    public double StepSequence { get; set; }

    public string CustomFlowKey { get; set; }

    public string SignedTitle { get; set; }

    public string SignedEmpName { get; set; }

    public string SignedEmpNum { get; set; }

    public string SignedTodo { get; set; }

    public string SignedDate { get; set; }

    public EnumProcessStatus Status { get; set; }
}

public class ResponseProcessStages : ResponseFlowBase
{
    public List<FlowProcessStage> Stages { get; set; }

    public bool HasNextFlow { get; set; }

    public bool IsInCounterSign { get; set; }
}