using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Sample.Api.Attributes;
using Sample.Api.Extensions;
using Sample.Api.Helpers;
using Sample.Api.Libs;
using Sample.Api.Models.DTO;
using Sample.Api.Repositories;
using Sample.Api.Repositories.DTO;

namespace Sample.Api.Middlewares
{
    /// <summary>
    /// api key 驗證
    /// </summary>
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;

        private readonly ILogger<ApiKeyAuthMiddleware> _logger;

        private readonly IMemoryCacheHelper _memoryCacheHelper;

        private readonly ConfigurationSection _WebSetting;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="env"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="memoryCacheHelper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiKeyAuthMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<ApiKeyAuthMiddleware> logger,
            IMemoryCacheHelper memoryCacheHelper
        )
        {
            _next = next;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException(nameof(_WebSetting));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
        }

        /// <summary>
        /// Invoke Life Cycle
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ipHelper"></param>
        /// <param name="systemRepository"></param>
        /// <param name="memoryCacheHelper"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context,
            IIPHelper ipHelper,
            SystemRepository systemRepository,
            IMemoryCacheHelper memoryCacheHelper)
        {
            string EndpointName = context.GetEndpoint()?.DisplayName ?? String.Empty;
            bool UseApiKeyAuthorize = _WebSetting.GetValue("UseApiKeyAuthorize", true);
            // 系統預設全部都驗證ApiKey
            // 是否驗證 ApiKey
            if (UseApiKeyAuthorize && IsDoApiKeyAuth(context.GetEndpoint()))
            {
                string system;
                string source;
                // (in header) x-system = {System}
                string? systemIdFromHeader = context.Request.Headers[HttpContextHeaderKey.System].FirstOrDefault();
                // (in header) x-source = AES-256{"source_id":"System", time: "{yyyy/MM/dd HH:mm:ss}", "apiKey": "ApiKey" }
                string? sourceFromHeader = context.Request.Headers[HttpContextHeaderKey.Source].FirstOrDefault();
                string ip = ipHelper.GetUserIP();
                if (String.IsNullOrEmpty(systemIdFromHeader) == false && String.IsNullOrEmpty(sourceFromHeader) == false)
                {
                    system = systemIdFromHeader;
                    source = sourceFromHeader;
                }
                else
                {
                    Tuple<string, string>? fromPostBody = await GetSystemAndSourceFromPostBody(context);
                    if (fromPostBody == null)
                    {
                        throw MyExceptionList.Unauthorized($"System And Source is requierd in postbody ;IP:{ip}").GetException();
                    }
                    system = fromPostBody.Item1;
                    source = fromPostBody.Item2;
                }

                // 1. get all system info from DB firsttime then from cache
                List<Repositories.DTO.SystemDTO> systemInfos = await GetExternalSystemList(systemRepository);

                // AES-256{"source_id":"System", time: "{yyyy/MM/dd HH:mm:ss}", "apiKey": "ApiKey" }
                Repositories.DTO.SystemDTO systemInfo = systemInfos.Where(x => x.System.Equals(system)).FirstOrDefault()
                         ?? throw MyExceptionList.Unauthorized($"System:{system} is not found in systemInfos;IP:{ip}").GetException();
                // 解密 source,valid time
                ParseEncryptedApiKey(systemInfo, source, out Tuple<Guid, DateTime> apiKeyTuple);

                // 驗證 api key
                CheckIfApiKeyExist(system, apiKeyTuple.Item1, systemInfo, EndpointName, ip);

                // Wirte systemInfo to httpcontext.item
                context.Items.Add(HttpContextItemKey.VerifiedSystemInfo, systemInfo);

                // log
                _logger.LogInformation($"System : {system}; IP: {ip} 通過驗證");
            }

            await _next(context);
        }

        /// <summary>
        /// 串接就有文件規格 system、source(apikey) 從post body 拿
        /// 處理大文件：如果請求正文很大，將它全部讀取到內存中可能會有性能問題。對於大文件，考慮使用流式處理。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<Tuple<string, string>?> GetSystemAndSourceFromPostBody(HttpContext context)
        {
            // 確保請求正文是可以讀取的
            context.Request.EnableBuffering();
            string? source = null;
            string? system = null;
            try
            {
                // 使用 JsonDocument 來流式解析 JSON
                using (var jsonDoc = await JsonDocument.ParseAsync(context.Request.Body))
                {
                    if (jsonDoc.RootElement.TryGetProperty(HttpContextPostBodyKey.Source.ToCamelCase(true), out JsonElement sourceValue))
                    {
                        source = sourceValue.GetString();
                    }
                    else if (jsonDoc.RootElement.TryGetProperty(HttpContextPostBodyKey.Source, out JsonElement sourceValue2))
                    {
                        source = sourceValue2.GetString();
                    }


                    if (jsonDoc.RootElement.TryGetProperty(HttpContextPostBodyKey.System.ToCamelCase(true), out JsonElement systemValue))
                    {
                        system = systemValue.GetString();
                    }
                    else if (jsonDoc.RootElement.TryGetProperty(HttpContextPostBodyKey.System, out JsonElement systemValue2))
                    {
                        system = systemValue2.GetString();
                    }

                    if (String.IsNullOrEmpty(source) == false && String.IsNullOrEmpty(system) == false)
                    {
                        return new Tuple<string, string>(system, source);
                    }
                }
            }
            catch (Exception ex)
            {
                throw MyExceptionList.Unauthorized($"{ex.Message + ex.StackTrace}").GetException();
            }
            finally
            {
                // 請求正文的多次讀取：在 ASP.NET Core 中，HttpRequest.Body 是一個只能讀取一次的流。如果你需要多次讀取它，你需要在讀取後重置流的位置，如 context.Request.Body.Position = 0;
                context.Request.Body.Position = 0; // 重置流的位置，以便後續使用
            }
            return null;

        }

        /// <summary>
        /// 是否驗證 ApiKey
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private static bool IsDoApiKeyAuth(Endpoint? endpoint)
        {
            ApiKeyAuthenticationAttribute? isMethodUseApiKeyAuth = endpoint?.Metadata.GetMetadata<ApiKeyAuthenticationAttribute>();
            AllowAnonymousAttribute? allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
            // 是否驗證 ApiKey
            bool DoApiKeyAuth = false;
            if (endpoint != null)
            {
                ControllerActionDescriptor? controllerActionDescriptor = endpoint.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();

                if (controllerActionDescriptor != null)
                {
                    var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
                    ApiKeyAuthenticationAttribute? isControllerUseApiKeyAuth = controllerTypeInfo.GetCustomAttributes<ApiKeyAuthenticationAttribute>().FirstOrDefault();
                    if (allowAnonymous == null)
                    {
                        DoApiKeyAuth = (isControllerUseApiKeyAuth != null || isMethodUseApiKeyAuth != null);
                    }
                }
            }
            return DoApiKeyAuth;
        }
        /// <summary>
        /// 檢查api key 格式是否正確
        /// api key 以AES加密
        /// api key format : [Guid,DateTime]
        /// </summary>
        private void ParseEncryptedApiKey(Repositories.DTO.SystemDTO systemInfo, string encryptedApiKeyData, out Tuple<Guid, DateTime> apiKeyTuple)
        {
            string rawSourceStr;
            rawSourceStr = EncryptionHelper.DecryptString(encryptedApiKeyData, systemInfo.HashKey, systemInfo.IVKey);

            // api key 到期時間
            int requestExpirationSeconds = _WebSetting.GetValue("ApiRequestExpirationSeconds", 60);
            try
            {
                Source source = JsonConvert.DeserializeObject<Source>(rawSourceStr) ?? throw MyExceptionList.IsInValidApiKeyPattern($"{rawSourceStr} : Is Null").GetException();
                DateTime requestSendTime = DateTime.ParseExact(source.time ?? throw new ArgumentNullException(nameof(source.time)), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                apiKeyTuple = new Tuple<Guid, DateTime>((Guid)(source.AES_key ?? throw new ArgumentNullException(nameof(source.AES_key))), DateTime.Now);

                // -1 永不到期
                if (requestExpirationSeconds != -1)
                {
                    DateTime requestExpiration = requestSendTime.AddSeconds(requestExpirationSeconds);
                    if (requestExpiration < DateTime.Now)
                    {
                        throw MyExceptionList.Unauthorized($"DateTime.Now : {DateTime.Now}; Api Expiration: {requestExpiration} ;RequestExpired").GetException();
                    }
                }

            }
            catch (Exception ex)
            {
                throw MyExceptionList.IsInValidApiKeyPattern($"{rawSourceStr} : Is InValid Pattern").GetException();
            }
        }

        /// <summary>
        /// 檢查api key是否正確
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="apiKey"></param>
        /// <param name="externalSystems"></param>
        /// <param name="EndpointName"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool CheckIfApiKeyExist(string systemID, Guid apiKey, List<Repositories.DTO.SystemDTO> externalSystems, string EndpointName, string ip)
        {
            bool isAllow = externalSystems.Where(x => systemID.Equals(x.System) && apiKey.Equals(x.ApiKey)).Any();
            if (isAllow == false)
            {
                throw MyExceptionList.Unauthorized($"Api Key not found in externalSystems; Api Key : {apiKey};  API endpoint :{EndpointName};IP:{ip} 執行").GetException();
            }
            return isAllow;
        }

        private bool CheckIfApiKeyExist(string systemID, Guid apiKey, Repositories.DTO.SystemDTO externalSystems, string EndpointName, string ip)
        {
            bool isAllow = externalSystems.System.Equals(systemID) && externalSystems.ApiKey.Equals(apiKey);
            if (isAllow == false)
            {
                throw MyExceptionList.Unauthorized($"Api Key not found in externalSystems; Api Key : {apiKey};  API endpoint :{EndpointName};IP:{ip} 執行").GetException();
            }
            return isAllow;
        }

        /// <summary>
        /// 取得 外部系統驗證資訊
        /// </summary>
        /// <param name="systemRepository"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<List<Repositories.DTO.SystemDTO>> GetExternalSystemList(SystemRepository systemRepository)
        {
            List<Repositories.DTO.SystemDTO>? externalSystems = await _memoryCacheHelper.GetValueAsync<List<Repositories.DTO.SystemDTO>?>(MemoryCacheKey.AllExternalSystem);
            if (externalSystems == null || externalSystems.Any() == false)
            {
                // bind From Appsetting
                externalSystems = new List<Repositories.DTO.SystemDTO> { };
                _config.GetSection("ExternalSystemList")?.Bind(externalSystems);
                // cache 初始化
                List<Repositories.DTO.SystemDTO>? externalSystemsFromDB = systemRepository.GetSystemList()?.ToList() ?? throw new Exception("GetExternalSystemList Fail No Data Return");
                externalSystems.AddRange(externalSystemsFromDB);
                await UpdateExternalSystemListStorage(externalSystems);
            }
            return externalSystems;
        }

        /// <summary>
        /// 將系統資訊寫入快取
        /// </summary>
        /// <param name="externalSystem"></param>
        /// <returns></returns>
        private async Task UpdateExternalSystemListStorage(List<Repositories.DTO.SystemDTO> externalSystem)
        {
            await _memoryCacheHelper.AddAsync(MemoryCacheKey.AllExternalSystem, externalSystem);
        }

        /// <summary>
        /// 取得 已通過apikey驗證系統資訊
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static SystemDTO? TryGetVerifiedSystemInfo(HttpContext httpContext)
        {
            object? systemInfo;
            bool res = httpContext.Items.TryGetValue(HttpContextItemKey.VerifiedSystemInfo, out systemInfo);
            if (res && systemInfo != null)
            {
                return (SystemDTO)systemInfo;
            }
            return null;
        }
    }

    // AES-256{"source_id":"System", time: "{yyyy/MM/dd HH:mm:ss}", "AES_key": "ApiKey" }
    /// <summary>
    /// 
    /// </summary>
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

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiKeyAuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthorizeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthMiddleware>();
        }
    }
}
