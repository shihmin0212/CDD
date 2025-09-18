using CommonUtilities.Helpers;
using Newtonsoft.Json;
using CDD.Web.Controllers;
using CDD.Web.Models.DTO;

namespace CDD.Web.Serives
{
    public class ApiKeyService
    {
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<LoginController> _logger;

        private readonly IConfiguration _config;

        #region appsetting.json 設定值

        private readonly ConfigurationSection _WebSetting;

        private readonly ExternalSystem _systemInfoForApiService;
        #endregion
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiKeyService(IWebHostEnvironment env,
            ILogger<LoginController> logger,
            IConfiguration config)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // _Web Setting 
            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
            // Init SystemInfoForApiService
            _systemInfoForApiService = new ExternalSystem();
            InitSystemInfoForApiService();
        }
        private void InitSystemInfoForApiService()
        {
            _config.GetSection("SystemInfoForApiService")?.Bind(this._systemInfoForApiService);
            if (String.IsNullOrEmpty(this._systemInfoForApiService.SystemID)) { throw new ArgumentNullException("SystemInfoForApiService SystemID IS Null"); }
            if (String.IsNullOrEmpty(this._systemInfoForApiService.ApiKey.ToString())) { throw new ArgumentNullException("SystemInfoForApiService ApiKey IS Null"); }
            if (String.IsNullOrEmpty(this._systemInfoForApiService.HashKey)) { throw new ArgumentNullException("SystemInfoForApiService HashKey IS Null"); }
            if (String.IsNullOrEmpty(this._systemInfoForApiService.IVKey)) { throw new ArgumentNullException("SystemInfoForApiService HashKey IS Null"); }
        }


        /// <summary>
        /// 取得Api Service 用之系統資訊
        /// </summary>
        /// <returns></returns>
        public ExternalSystem GetSystemInfoForApiService()
        {
            return this._systemInfoForApiService;
        }


        /// <summary>
        /// 取得Api Service 用之 api key
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetApiKeyHeaders()
        {
            Tuple<string, string> encryptedApiKey = GetSystemWithEncryptedApiKey(
                    _systemInfoForApiService.SystemID,
                    _systemInfoForApiService.ApiKey,
                    _systemInfoForApiService.HashKey,
                    _systemInfoForApiService.IVKey);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(HttpContextHeaderKey.SystemID, encryptedApiKey.Item1);
            headers.Add(HttpContextHeaderKey.ApiKey, encryptedApiKey.Item2);
            return headers;
        }

        /// <summary>
        /// OOA, (Encrypt) OOA + apiKey + time
        /// </summary>
        /// <param name="system"></param>
        /// <param name="apiKey"></param>
        /// <param name="hashKey"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetSystemWithEncryptedApiKey(string system, Guid apiKey, string hashKey, string iv)
        {
            return Tuple.Create(system, EncryptionHelper.EncryptString(
                        JsonConvert.SerializeObject(new Source
                        {
                            source_id = system,
                            AES_key = apiKey,
                            time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                        }),
                    hashKey,
                    iv));
        }
    }

    public class Source
    {
        /// <summary>
        /// OOA
        /// </summary>
        public string? source_id { get; set; }

        /// <summary>
        /// yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string? time { get; set; }

        /// <summary>
        /// UUID
        /// </summary>
        public Guid? AES_key { get; set; }
    }
}
