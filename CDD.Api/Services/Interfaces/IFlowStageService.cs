using CDD.API.Models.Response;

namespace CDD.API.Services.Interfaces
{
    public interface IFlowStageService
    {
        Task<GetFlowStatusResp> GetFlowStatusAsync(string signId);
        Task<ProcessStatusResult> GetProcessStatusAsync(string signId);
        Task<GetMemberInfoResp> GetMemberInfoAsync(string employeeID);
        Task<GetBackApproverResp> GetBackApproverAsync(string signId, string stageDesignate);
    }
}