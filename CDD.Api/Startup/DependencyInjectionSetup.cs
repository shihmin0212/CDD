using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Quartz;
using Quartz.Impl;
using Quartz.Plugin.Interrupt;
using Quartz.Spi;
using Quartz.Util;
using CDD.Api.ActionFilters;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.DTO;
using CDD.Api.Repositories;
using CDD.Api.Services;

namespace CDD.Api.Startup
{
    public static class DependencyInjectionSetup
    {
        /// <summary>
        /// Add env appsettings 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureAppSetting(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                // 清除所有預設的配置來源
                config.Sources.Clear();
                config.SetBasePath(System.AppContext.BaseDirectory);
                // 僅使用指定的 appsettings 檔案
                if (hostingContext.HostingEnvironment.IsProduction())
                {
                    config.AddJsonFile($"appsettings.json",
                                   optional: false,
                                   reloadOnChange: true);
                }
                else
                {
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                   optional: false,
                                   reloadOnChange: true);
                }

                // 如果有需要，也可以從環境變數或命令列參數載入（選擇性）
                // 4.載入環境變數(如需使用 Docker 或其他容器配置時非常有用)
                // config.AddEnvironmentVariables(); 

                config.Build();
            });
            return host;
        }

        /// <summary>
        /// 讀取命令列參數的輔助方法
        /// </summary>
        /// <param name="args"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string? GetCommandLineArgument(string[] args, string key)
        {
            string prefix = $"--{key}=";
            string? arg = args.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            return arg != null ? arg.Substring(prefix.Length) : null;
        }

        public static ILoggingBuilder NLogSetup(this ILoggingBuilder logging)
        {
            // NLog: Setup NLog for Dependency injection
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

            return logging;
        }


        public static IServiceCollection RegisterServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            #region Stop auto response 400
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            #endregion

            #region Add Cus Filter Action to Controllers 
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>(1);  // 自訂 ModelStateFilter 錯訊
                options.Filters.Add<AutoLogAttribute>(2);        // Auto Log Request And Response
            })
            .AddNewtonsoftJson(jsonSettings =>
            {
                jsonSettings.SerializerSettings.Formatting = Formatting.None;
                // Parse dates as DateTimeOffset values by default. You should prefer using DateTimeOffset over
                // DateTime everywhere. Not doing so can cause problems with time-zones.
                jsonSettings.SerializerSettings.DateParseHandling =
                  DateParseHandling.DateTimeOffset;

                jsonSettings.SerializerSettings.ReferenceLoopHandling =
                  ReferenceLoopHandling.Ignore;

                // Output enumeration values as strings in JSON.
                //jsonSettings.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            #endregion

            services.AddResponseCompression();

            #region 正確接收傳遞 header From Proxy
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = null;        // 設置非 null 值是一種預防措施 設定為 null 可停用限制 預設值為 1。
                options.KnownNetworks.Clear();      // 可以清除KnownNetworks和KnownProxies列表來接受任何轉發的請求
                options.KnownProxies.Clear();
            });
            #endregion

            #region Swagger DI

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CDD Api Service",
                    Contact = new OpenApiContact { Name = "陳先生", Email = "nautilustube@gmail.com" },
                    Version = "v1",
                    Description =
                    $"串接系統請提供以下基本資訊用於（Api Key）驗證,並於每次請求夾帶</br>" +
                    $"方式1.在Http Header中夾帶請求授權驗證系統（Api Key）</br>" +
                    "System:系統代碼;  time:時間;  ApiKey:系統註冊後提供系統專屬apiKey</br></br>" +
                    "1.1 Name : ”x-system” : System 系統代碼</br>" +
                    "1.2 Name : ”x-source” : AES-256 ( {\"source_id\":\"System\", time: \"{yyyy/MM/dd HH:mm:ss}\", \"apiKey\": \"ApiKey\" })以AES加密(請使用註冊後提供的系統專屬KEY 和IV値)，加密完成後，轉換成Base64字串</br></br>" +
                    "</br></br>" +
                    $"方式2.在Http PostBody 中夾帶請求授權驗證系統（Api Key）</br>" +
                    "System:系統代碼;  time:時間;  ApiKey:系統註冊後提供系統專屬apiKey</br></br>" +
                    "2.1 Name : ”System” : System </br>" +
                    "2.2 Name : ”Source” : AES-256 ( {\"source_id\":\"System\", time: \"{yyyy/MM/dd HH:mm:ss}\", \"apiKey\": \"ApiKey\" }以AES加密(請使用註冊後提供的系統專屬KEY 和IV値)，加密完成後，轉換成Base64字串</br></br>"
                });
                c.AddSecurityDefinition("System", new OpenApiSecurityScheme()
                {
                    Description = "System must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = HttpContextHeaderKey.System, //header with x-system-id
                    Scheme = "SystemIDScheme"
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "System"
                    },
                    In = ParameterLocation.Header
                };
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Description = "ApiKey must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = HttpContextHeaderKey.Source, //header with api key
                    Scheme = "ApiKeyScheme"
                });
                var key1 = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() },
                             { key1, new List<string>() }
                    };
                c.AddSecurityRequirement(requirement);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Cus Services DI
            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddHttpContextAccessor();
            services.AddTransient<IIPHelper, IPHelper>();           // Get IP
            services.AddScoped<AutoLogAttribute>();
            services.AddScoped<ValidateModelAttribute>();



            #region HttpClientFactory Retry polly
            IConfigurationSection WebSetting = configuration.GetSection("WebSetting");
            bool useApiRetry = WebSetting.GetValue("UseApiRetry", false);
            int individualTimeoutSec = WebSetting.GetValue("ApiRequestTimeoutSeconds", 30);

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();        //NoOp 策略只是“按原樣”執行底層調用，沒有任何額外的策略行為
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(individualTimeoutSec); // Timeout for an individual try

            WebProxy? webProxy = null;
            bool useApiProxy = WebSetting.GetValue("UseApiProxy", true);
            if (useApiProxy)
            {
                String proxyURL = "http://proxy.esunsec.com.tw:8080";
                webProxy = new WebProxy()
                {
                    Address = new Uri($"{proxyURL}"),
                    BypassProxyOnLocal = true,
                    BypassList = new string[] {
                    "https://backstageapi.testesunsec.com.tw",
                }
                };
            }

            services.AddHttpClient<IRequest, Request>("HttpClient", client => { client.Timeout = TimeSpan.FromSeconds(individualTimeoutSec); })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // C#的程式裡面發出request和內部或者外部的服務溝通，如果內部或者外部的服務只允許https連線
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseCookies = false,
            })
            .AddPolicyHandler(
                (services, request) =>
                {
                    if (useApiRetry == false)
                    {
                        return noOpPolicy;
                    }
                    else
                    {
                        return
                            HttpPolicyExtensions
                            .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                                                        //.OrResult(msg => !msg.IsSuccessStatusCode && msg.StatusCode!= HttpStatusCode.TooManyRequests) // ensure 200 - 299 exclude 429
                            .WaitAndRetryAsync(new[]
                            {
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(6)
                            },
                            onRetry: (outcome, timespan, retryAttempt, context) =>
                            {
                                services.GetService<ILogger>()?
                                    .LogWarning($"Is Not Success StatusCode :{outcome.Result.StatusCode},Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}.");
                            });
                    }

                }
            ).AddPolicyHandler(timeoutPolicy); // We place the timeoutPolicy inside the retryPolicy, to make it time out each try.;
            #endregion
            services.AddTransient<IRequest, Request>();             // Get Request Data Methods 
            services.AddTransient<APIHelper>();                     // Get Request Data Methods 
            services.AddScoped<IDapperHelper, DapperHelper>();      // DB Query Helper
            services.AddScoped<BaseRepository<object>>();
            services.AddScoped<UserRepository>();
            services.AddScoped<SystemRepository>();
            services.AddScoped<AdminSystemRepository>();
            // TODO：再確認參考其他專案的 IRequest 實作
            services.AddScoped<IRequest, Request>();

            services.AddScoped<IFlowStageService, FlowStageService>();
            services.AddMemoryCache();
            services.AddSingleton<IMemoryCacheHelper, MemoryCacheHelper>();

            #endregion

            return services;
        }

        /// <summary>
        /// 註冊發信服務
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddFluentEmail(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Add Email Service
            services.AddTransient<IEmailService, EmailService>();
            IConfigurationSection emailSettings = configuration.GetSection("EmailSetting");
            string? defaultFromEmail = emailSettings.GetValue("DefaultFromEmail", String.Empty);
            string host = emailSettings["SmtpHost"] ?? throw new ArgumentException("SmtpHost is missing in appsettting");
            int port = emailSettings.GetValue("SmtpPort", 25);
            string? userName = emailSettings["SmtpUserName"];
            string? password = emailSettings["smtpPassword"];

            if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(password))
            {
                services
                .AddFluentEmail(defaultFromEmail)
                .AddRazorRenderer()
                .AddSmtpSender(host, port, userName, password);
            }
            else
            {
                services
                    .AddFluentEmail(defaultFromEmail)
                    .AddRazorRenderer()
                    .AddSmtpSender(host, port);
            }

            return services;
        }

    }
}
