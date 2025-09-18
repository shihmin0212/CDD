using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using Sample.Api.Middlewares;
using Sample.Api.Models.DTO;
using Sample.Api.Startup;

#region Nlog Init
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
#endregion
try
{
    // https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding?view=net-6.0
    // dotnet core default not support big-5
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    // ASPNETCORE_ENVIRONMENT From IIS 
    string? aspnetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    logger.Info("ASPNETCORE_ENVIRONMENT:" + aspnetEnvironment ?? "Not Found");
    // 從命令列參數中取得 environment
    string? environmentFromArgs = DependencyInjectionSetup.GetCommandLineArgument(args, "environment");
    logger.Info("environmentFromArgs:" + environmentFromArgs ?? "Not Found");

    string environment = (environmentFromArgs ?? aspnetEnvironment ?? "Production");

    //WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
    // sc create SchedulerWorker binpath="D:\SchedulerWorker\SchedulerWorker.exe"
    WebApplicationOptions webApplicationOptions = new WebApplicationOptions
    {
        EnvironmentName = environment,
        ContentRootPath = AppContext.BaseDirectory,
        Args = args,
    };

    var builder = WebApplication.CreateBuilder(webApplicationOptions);
    builder.Configuration.AddCommandLine(args);
    builder.Host.UseWindowsService();
    builder.Host.UseSystemd();



    // allows both to access and to set up the config
    ConfigurationManager configuration = builder.Configuration;
    #region NLog: Setup NLog for Dependency injection+
    builder.Logging.NLogSetup();
    builder.Host.UseNLog();
    #endregion

    #region Config AppSetting.Json
    builder.Host.ConfigureAppSetting();
    #endregion

    IConfiguration webSetting = configuration.GetSection("WebSetting");
    bool UseKestrelWebHost = webSetting.GetValue("UseKestrelWebHost", true);
    if (UseKestrelWebHost)
    {
        builder.WebHost.UseKestrel();
    }

    #region 設定上傳大小
    // application running on IIS:
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = int.MaxValue;
    });
    builder.Services.Configure<FormOptions>(options =>
    {
        //options.ValueLengthLimit = int.MaxValue;        
        options.MultipartBodyLengthLimit = 128 * 1024 * 1024; // default value is: 128 MB
        //options.MultipartHeadersLengthLimit = int.MaxValue;
    });
    #endregion

    #region Add services to the container. 
    builder.Services.AddDistributedMemoryCache();               // use an in-memory cache
    builder.Services.RegisterServices(configuration); 
    builder.Services.AddFluentEmail(configuration); // 寄送信件服務註冊
    #endregion

    var app = builder.Build();
    logger.Info("ENV:" + app.Environment.EnvironmentName);

    #region Middleware Pipeline
    app.ConfigureForceHttps(configuration);     // 在某些情況下，可能無法將轉送的標頭新增至透過 Proxy 傳送給應用程式 騙過它
    app.ConfigureForwardedHeaders();            // IIS 轉送標頭中介 至 Kestrel
    app.UseResponseCompression();

    //  Exception Handler
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseMyExceptionHandler();

    app.ConfigureHstsAndHttpsRedirection(configuration);    // app.UseHsts(); + app.UseHttpsRedirection();
    // Set Auto Log Uid
    app.Use(async (ctx, next) =>
    {
        // 0.http action id create 
        ctx.Items[HttpContextItemKey.ActionId] = Guid.NewGuid().ToString();
        await next();
    });
    app.UseRouting();
    app.UseApiKeyAuthorizeMiddleware();     // api key 驗證
    app.UseAdminTokenAuthMiddleware();      // admin jwt token 驗證(api key system資訊依賴 ApiKeyAuthorizeMiddleware)
    app.ConfigureSwagger(configuration);
    #endregion

    #region Map methods 
    app.MapControllers();
    #endregion

    app.Run();

}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, $"Stopped program because of exception:{JsonConvert.SerializeObject(ex)}");
}
finally
{
    logger.Error("Application Shutdown");
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}


public partial class Program { }
