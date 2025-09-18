using CDD.Api.Libs;

namespace CDD.Api.Helpers
{
    public class APIHelper
    {
        private readonly IConfiguration _config;
        private readonly ILogger<APIHelper> _logger;
        private readonly IRequest _request;

        private readonly ConfigurationSection _URL;

        public APIHelper(
            ILogger<APIHelper> logger,
            IConfiguration config,
            IRequest request
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _URL = (ConfigurationSection)config.GetSection("URL");
        }

        /*public async Task<API_GetCloseRateResp> GetCloseRate(DateTime dateTime, int currency)
        {
            API_GetCloseRateResp resp = new API_GetCloseRateResp() { Status = false };
            // DateTime=2022-06-01&Currency=2
            string url = $"{_URL["GetCloseRate"]}?DateTime={dateTime.ToString("yyyy-MM-dd")}&Currency={currency}";
            try
            {
                #region API Response Json Example
                //{
                //    "status": true,
                //    "message": "",
                //    "dateTime": "20220531",
                //    "currency": "JPY",
                //    "rate": 0.2275
                //}
                #endregion
                resp = await _request.GetJSON<API_GetCloseRateResp>(url);
                if (resp.Status)
                {
                    return resp;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HttpRequestException:{ex.Message}; {ex.StackTrace}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"TaskCanceledException:{ex.Message}; {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception:{ex.Message}; {ex.StackTrace}");
            }
            return resp;
        }
        */
    }
}
