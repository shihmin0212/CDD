using CDD.Api.Libs;
using CDD.Api.Models.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CDD.Api.Services
{
    /// <summary>以 Libs.IRequest 呼叫外部 FlowStage API</summary>
    public class ExternalFlowApiClient_ByRequest : IExternalFlowApiClient
    {
        private readonly IRequest _request;
        private readonly IConfiguration _config;
        private readonly ILogger<ExternalFlowApiClient_ByRequest> _logger;

        public ExternalFlowApiClient_ByRequest(
            IRequest request,
            IConfiguration config,
            ILogger<ExternalFlowApiClient_ByRequest> logger)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FlowStatusRoot> GetFlowStatusAsync(string signId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(signId))
                throw new ArgumentException("signId is required", nameof(signId));

            var baseUrl = _config["ExternalApis:FlowStage:BaseUrl"];
            var url = $"{baseUrl.TrimEnd('/')}/FlowStageVer2/GetFlowStatus?signId={Uri.EscapeDataString(signId)}";

            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            };
            var apiKey = _config["ExternalApis:FlowStage:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
                headers["X-API-KEY"] = apiKey;

            var noLogReq = new[] { "signId=", signId };
            var noLogRes = new[] { "\"SignID\"", "\"JBPMUID\"", "\"SignedEmpNum\"", "\"SignedEmpName\"" };

            try
            {
                _logger.LogInformation("Calling GetFlowStatus: {Url}",
                    _request.anonymizationLogBody(url, noLogReq, "******"));

                var data = await _request.GetJSON<FlowStatusRoot>(url, headers, noLogReq, noLogRes);
                return data ?? new FlowStatusRoot
                {
                    IsSuccess = false,
                    FlowStatus = new FlowStatusData(),
                    ValidationMsg = new() { "Null response" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFlowStatus failed. signId={signId}", signId);
                return new FlowStatusRoot
                {
                    IsSuccess = false,
                    FlowStatus = new FlowStatusData(),
                    ValidationMsg = new() { ex.Message }
                };
            }
        }
    }
}
