using CDD.Api.Models.External;

namespace CDD.Api.Services
{
    /// <summary>FlowStage 外部 API 用戶端介面</summary>
    public interface IExternalFlowApiClient
    {
        /// <summary>呼叫 GET /FlowStageVer2/GetFlowStatus?signId=...</summary>
        Task<FlowStatusRoot> GetFlowStatusAsync(string signId, CancellationToken ct = default);
    }
}
