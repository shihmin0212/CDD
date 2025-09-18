using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using CommonUtilities.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using CDD.Web.FilterAttributes;
using CDD.Web.Helpers;
using CDD.Web.Models.DTO;
using CDD.Web.Serives;
using CDD.Web.Services;
using StackExchange.Redis;

namespace CDD.Web.Startup
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

        public static IServiceCollection ConfigureWebEncoder(this IServiceCollection services)
        {
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(new[] {
                    UnicodeRanges.BasicLatin,
                    UnicodeRanges.HalfwidthandFullwidthForms,   // 全形英文或標點符號
                    UnicodeRanges.CjkUnifiedIdeographs,         // 中日韓統一表意文字列表
                    UnicodeRanges.CjkSymbolsandPunctuation,     // 中日韓標點符號
                });
            });
            return services;
        }

        public static IServiceCollection SwaggerDI(this IServiceCollection services, ConfigurationManager config)
        {
            #region Swagger DI
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion
            return services;
        }

        public static IServiceCollection RegisterBasicServices(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddResponseCompression();
            services.AddCors(o => o.AddPolicy("CORSPolicy", builder =>
            {
                builder//.WithOrigins(configuration.GetSection("CORS").GetChildren().Select(c => c.Value).ToArray())
                    .WithMethods(new string[] { "GET", "POST" })
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin => true); // allow any origin
            }));

            // Add services to the container.
            services.AddControllersWithViews(
                    (options) =>
                    {
                        if (config.GetValue<bool>("WebSetting:SecureXSRF"))
                        {
                            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        }
                        options.Filters.Add<LoginAuthenticationFilter>();

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

            // 正確接收傳遞header
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = null;        // 設置非 null 值是一種預防措施 設定為 null 可停用限制 預設值為 1。
                options.KnownNetworks.Clear();      // 可以清除KnownNetworks和KnownProxies列表來接受任何轉發的請求
                options.KnownProxies.Clear();
            });

            return services;
        }

        public static IServiceCollection RegisterSessionServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDistributedMemoryCache();

            // ACL SETUSER ETF >168@esun ~CDD.Web* +@all -@admin -@dangerous
            // ACL SETUSER ETF >168@esun ~* +@all
            // ACL SETUSER ETF ON
            // ACL USERS
            if (configuration.GetValue<bool>("WebSetting:IsUseRedisSessionStorage"))
            {
                string redisKeyPrefix = configuration.GetValue<string>("RedisConfig:KeyPrefix");
                string redisConnectStr = configuration.GetValue<string>("RedisConfig:ConnectionString");

                // Configuration options for the Redis connection
                ConfigurationOptions config = ConfigurationOptions.Parse(redisConnectStr);
                int databaseIndex = configuration.GetValue<int>("RedisConfig:SessionDBIndex");
                config.DefaultDatabase = databaseIndex; // Selecting the desired database index
                                                        //config.User = "ETF";
                                                        //config.Password = "168@esun";

                ConnectionMultiplexer redis = ConnectionMultiplexer
                    .Connect(config);
                services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "ERMS_DataProtectionKeys")
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(365))
                    .SetApplicationName("CDD.Web");

                services.AddStackExchangeRedisCache(option =>
                {
                    option.ConfigurationOptions = config;
                    option.InstanceName = redisKeyPrefix;
                });
            }

            // Session Setting 
            services.AddSession(options =>
            {
                options.Cookie.Name = SessionKey.SessionCookieName;
                options.IdleTimeout = TimeSpan.FromMinutes(
                   Int32.Parse(configuration.GetSection("CookieConfig")["IdleTimeoutMinutes"])
                );
                options.Cookie.MaxAge = TimeSpan.FromMinutes(
                    Int32.Parse(configuration.GetSection("CookieConfig")["CookieExpireMinutes"])
                );
                options.Cookie.IsEssential = true; // 防止在未經用戶明確許可的情況下將任何非必要的 cookie 發送到瀏覽器（無 Set-Cookie 標頭）
                options.Cookie.HttpOnly = true;
                // Add the SameSite attribute, this will emit the attribute with a value of none.
                // https://stackoverflow.com/questions/59990864/what-is-difference-between-samesite-lax-and-samesite-strict
                // Lax 允許在某些跨站點請求上發送 cookie，而 Strict 絕不允許在跨站點請求上發送 cookie。
                // Lax cookie可以跨站發送的情況必須滿足以下兩個條件
                // 1.URL 欄中顯示的 URL 發生變化，例如，用戶單擊鏈接以轉到另一個站點
                // 2.請求方法必須是安全的（例如 GET 或 HEAD，但不是 POST）。

                // 設定 為 Lax 從台網回來 Session 得以存續
                options.Cookie.SameSite = SameSiteMode.None;
                // Always : 安全一律標示為 true。 當您的登入頁面和所有後續頁面需要的所有驗證識別均為 HTTPS 時，請使用此值。 本機開發也會需要以 HTTPS URL 完成。
                // SameAsRequest : 如果提供 Cookie 的 URI 是 HTTPS，則 Cookie 只會在後續的 HTTPS 要求上傳回給伺服器。
                options.Cookie.SecurePolicy = Convert.ToBoolean(configuration.GetSection("CookieConfig")["SecureCookie"]) ?
                    CookieSecurePolicy.Always : CookieSecurePolicy.None;
            });
            return services;
        }

        public static IServiceCollection RegisterAntiforgeryServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;    // 絕不允許在跨站點請求上發送 cookie。 Dev 需打開 None
                options.Cookie.Name = XSRFKey.CookieName;
                options.HeaderName = XSRFKey.HeaderName;
                options.Cookie.SecurePolicy = Convert.ToBoolean(configuration.GetSection("CookieConfig")["SecureCookie"]) ?
                    CookieSecurePolicy.Always : CookieSecurePolicy.None;

            });
            return services;
        }

        public static IServiceCollection RegisterCusServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddMemoryCache();
            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            //services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //IHttpContextAccessor register
            services.AddHttpContextAccessor();
            services.AddScoped<SecurityHeadersAttribute>();
            //services.AddScoped<ServiceAlertMessageAttribute>();     // Add As Scope For Log Each Request Message
            services.AddScoped<ValidateModelAttribute>();

            #region HttpClientFactory Retry Polly
            IConfigurationSection WebSetting = configuration.GetSection("WebSetting");
            bool useApiRetry = WebSetting.GetValue("UseApiRetry", false);
            int individualTimeoutSec = WebSetting.GetValue("ApiRequestTimeoutSeconds", 30);

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();        //NoOp 策略只是“按原樣”執行底層調用，沒有任何額外的策略行為
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(individualTimeoutSec); // Timeout for an individual try

            WebProxy? webProxy = null;
            bool useApiProxy = WebSetting.GetValue("UseApiProxy", false);
            if (useApiProxy)
            {
                String proxyURL = WebSetting.GetValue("ProxyURL", "http://proxy.esunsec.com.tw:8080") ?? "http://proxy.esunsec.com.tw:8080";
                List<string> bypassList = new List<string>();
                WebSetting.GetSection("ProxyBypassList")?.Bind(bypassList);
                webProxy = new WebProxy()
                {
                    Address = new Uri($"{proxyURL}"),
                    BypassProxyOnLocal = true,
                    BypassList = (bypassList.Any()) ? bypassList.ToArray() : default
                };
            }

            services.AddHttpClient<Libs.IRequest, Libs.Request>("HttpClient", client => { client.Timeout = TimeSpan.FromSeconds(individualTimeoutSec); })
            .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    HttpClientHandler handler = new HttpClientHandler
                    {
                        // C#的程式裡面發出request和內部或者外部的服務溝通，如果內部或者外部的服務只允許https連線
                        ClientCertificateOptions = ClientCertificateOption.Manual,

                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                        UseCookies = false,
                        SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                        Proxy = webProxy
                    };
                    return handler;
                }
            )
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

            services.AddTransient<IIPHelper, IPHelper>();           // Get IP helper
            services.AddScoped<AutoLogAttribute>();                 // AutoLog Request & Response Filter
            services.AddTransient<APIHelper>();                     // API helper
            services.AddSingleton<EncryptionHelper>();              // 加解密 helper 
            services.AddTransient<ApiKeyService>();                 // Get ApiKey Hepler 
            services.Configure<RsaKeyOptions>(configuration.GetSection("Encryption"));
            services.AddTransient<UserService>();                  // 使用者/登入/登出/資訊 Service

            #region Inject ShareLibs And Common Libs
            // 驗證碼產生
            services.AddScoped<ICaptchaHepler, CaptchaHepler>(provider =>
            {
                // ILogger<CaptchaHepler> logger = provider.GetRequiredService<ILogger<CaptchaHepler>>();
                return new CaptchaHepler("Arial");
            });
            #endregion
            return services;
        }


        public static IServiceCollection RegisterHstsAndHttpsRedirectionServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            #region Add hsts
            if (Convert.ToBoolean(configuration.GetSection("AppSetting")["UseHsts"]))
            {
                services.AddHsts(options =>
                {
                    options.Preload = Convert.ToBoolean(configuration.GetSection("AppSetting")["Hsts_Preload"]);
                    options.IncludeSubDomains = Convert.ToBoolean(configuration.GetSection("AppSetting")["Hsts_IncludeSubDomains"]);
                    options.MaxAge = TimeSpan.FromDays(Int32.Parse(configuration.GetSection("AppSetting")["Hsts_MaxAgeDays"]));
                });
            }
            #endregion

            #region Add HttpsRedirection
            if (Convert.ToBoolean(configuration.GetSection("AppSetting")["UseHttpsRedirection"]))
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
                    options.HttpsPort = Int32.Parse(configuration.GetSection("AppSetting")["HttpsPort"]);
                });
            }
            #endregion
            return services;
        }
    }
}
