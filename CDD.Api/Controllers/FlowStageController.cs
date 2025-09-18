using Microsoft.AspNetCore.Mvc;
using CDD.Api.Attributes;
using CDD.Api.Models.Response;
using CDD.Api.Services;
using CDD.Api.Models.External;

namespace CDD.Api.Controllers
{
    /// <summary>FlowStage 代理查詢 API</summary>
    [Route("api/FlowStage")]
    [ApiController]
    [LogOptions]
    [ApiKeyAuthentication]
    [AdminTokenAuth]
    public class FlowStageController(IExternalFlowApiClient client, ILogger<FlowStageController> logger) : ControllerBase
    {
        private readonly IExternalFlowApiClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger<FlowStageController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>FSA-002：GetFlowStatus</summary>
        /// <param name="signId">表單編號</param>
        [HttpGet("GetFlowStatus")]
        [Produces("application/json")]
        public async Task<GeneralResp<FlowStatusRoot>> GetFlowStatus([FromQuery] string signId)
        {
            var resp = new GeneralResp<FlowStatusRoot>();

            try
            {
                var data = await _client.GetFlowStatusAsync(signId);

                resp.Status = data.IsSuccess;
                resp.Code = data.IsSuccess ? 0 : 1; // 0:成功, 1:查無資料或外部回傳失敗
                resp.Message = (!data.IsSuccess && (data.ValidationMsg?.Count ?? 0) > 0)
                               ? string.Join("; ", data.ValidationMsg)
                               : string.Empty;
                resp.Result = data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFlowStatus exception. signId={signId}", signId);
                resp.Status = false;
                resp.Code = -1;                  // 系統/權限等錯誤
                resp.Exception = ex.ToString();
                resp.Message = ex.Message;
                resp.Result = new FlowStatusRoot
                {
                    IsSuccess = false,
                    FlowStatus = new FlowStatusData(),
                    ValidationMsg = new List<string> { ex.Message }
                };
            }

            return resp;
        }
    }
}
