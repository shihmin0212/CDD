using SurveryProject.Service.Base;

namespace SurveryProject.Service.Base
{
    /// <summary>
    /// Http request url collection
    /// </summary>
    public class HTTPRequests
    {
        private readonly DataConfig _config;

        public HTTPRequests(DataConfig config)
        {
            _config = config;
        }

        public string GetFlowStatus(string formNum)
        {
            return $"https://FlowStageAPISIT.testesunsec.com.tw/FlowStageVer2/GetFlowStatus?signID={formNum}";
        }

        public string GetProcessStages(string customFlowKey)
        {
            return $"{_config.FlowStageDomain}/api/ver2/GetProcessStages/{customFlowKey}";
        }

        public string GetProcessStatus(string formNum)
        {
            return $"https://FlowStageAPISIT.testesunsec.com.tw/api/ver2/GetProcessStatus/{formNum}";
        }

        public string GetProcessLog(string uid)
        {
            return $"{_config.FlowStageDomain}/FlowStageVer2/GetProcessAuditLog?parameter={uid}&isUid=true";
        }

        public string GetBackApprover(string signID, int stageDesignate, bool toApplicant)
        {
            return $"{_config.FlowStageDomain}/FlowStageVer2/GetBackApprover?signID={signID}&stageDesignate={stageDesignate}&toApplicant={toApplicant}";
        }

        public string GetDefaultFlow(string formCode, string customFlowKey)
        {
            string uri = $"{_config.FlowStageDomain}/FlowStageVer2/GetDefaultFlow?docCode={formCode}";
            if (!string.IsNullOrEmpty(customFlowKey))
            {
                uri += $"&customFlowKey={customFlowKey}";
            }
            return uri;
        }

        public string GetAuditLog(string param, bool isUid)
        {
            return $"{_config.FlowStageDomain}/FlowStageVer2/GetAuditLog?parameter={param}&isUid={isUid}";
        }

        public string GetMemberInfo(string employeeID)
        {
            return $"{_config.FlowStageDomain}/FlowStageVer2/GetMemberInfo?employeeID={employeeID}";
        }

        public string UpdateFlowStatus()
        {
            return $"{_config.FlowStageDomain}/FlowStageVer2/UpdateFlowStatus";
        }
    }
}
