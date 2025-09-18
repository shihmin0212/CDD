using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using CDD.Api.Attributes;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.DTO;
using CDD.Api.Repositories;
using CDD.Api.Repositories.DTO;

namespace CDD.Api.Middlewares
{
    /// <summary>
    /// admin Token 驗證
    /// </summary>
    public class AdminTokenAuthMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;

        private readonly ILogger<AdminTokenAuthMiddleware> _logger;

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
        public AdminTokenAuthMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            IConfiguration config,
            ILogger<AdminTokenAuthMiddleware> logger,
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
            if (UseApiKeyAuthorize && IsDoApiTokenAuth(context.GetEndpoint()))
            {
                string ip = ipHelper.GetUserIP();
                SystemDTO? systemInfo = ApiKeyAuthMiddleware.TryGetVerifiedSystemInfo(context);
                if (systemInfo == null)
                {
                    throw MyExceptionList.Unauthorized($"(Admin) System And Source is requierd ;IP:{ip}").GetException();
                }

                if (systemInfo.IsAdmin == false)
                {
                    throw MyExceptionList.Unauthorized($"(Admin) System ${systemInfo.System} is not Admin Role ;IP:{ip}").GetException();
                }

                // log
                _logger.LogInformation($"Admin System : {systemInfo?.System}; IP: {ip} 通過驗證");
            }

            await _next(context);
        }

        /// <summary>
        /// 是否驗證 AdminToken ApiKey
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private static bool IsDoApiTokenAuth(Endpoint? endpoint)
        {

            AdminTokenAuthAttribute? isMethodUseApiKeyAuth = endpoint?.Metadata.GetMetadata<AdminTokenAuthAttribute>();
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
                    AdminTokenAuthAttribute? isControllerUseApiKeyAuth = controllerTypeInfo.GetCustomAttributes<AdminTokenAuthAttribute>().FirstOrDefault();
                    if (allowAnonymous == null)
                    {
                        DoApiKeyAuth = (isControllerUseApiKeyAuth != null || isMethodUseApiKeyAuth != null);
                    }
                }
            }
            return DoApiKeyAuth;
        }


        /// <summary>
        /// 取得 已通過apikey驗證系統資訊
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static SystemDTO? GetVerifiedAdminSystemInfo(HttpContext httpContext)
        {
            object? _systemInfo;
            bool res = httpContext.Items.TryGetValue(HttpContextItemKey.VerifiedSystemInfo, out _systemInfo);
            if (res && _systemInfo != null)
            {
                SystemDTO systemInfo = (SystemDTO)_systemInfo;
                if (systemInfo != null && !string.IsNullOrEmpty(systemInfo.System) && systemInfo.ApiKey != Guid.Empty && systemInfo.IsAdmin == true)
                {
                    return systemInfo;
                }
            }
            return null;
        }
    }

    // 
    /// <summary>
    /// 
    /// </summary>

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AdminTokenAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminTokenAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminTokenAuthMiddleware>();
        }
    }
}
