using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using CDD.Web.Libs;
using CDD.Web.Middlewares;
using CDD.Web.Startup;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
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

    // 使用 WebApplicationOptions 設定環境
    WebApplicationOptions options = new WebApplicationOptions
    {
        EnvironmentName = environment,
        Args = args
    };

    var builder = WebApplication.CreateBuilder(options);

    #region NLog: Setup 
    // NLog: Setup NLog for Dependency injection
    builder.Logging.NLogSetup();
    builder.Host.UseNLog();
    #endregion

    #region Config AppSetting.json && Kestrel
    builder.Host.ConfigureAppSetting();
    #endregion

    // HtmlEncoder unicode allow range
    builder.Services.ConfigureWebEncoder();

    // allows both to access and to set up the config
    ConfigurationManager configuration = builder.Configuration;
    builder.Services.RegisterBasicServices(configuration);
    #region Session Setting 
    builder.Services.RegisterSessionServices(configuration);
    #endregion

    #region 設定上傳大小
    int maxUploadSize = Convert.ToInt32(configuration.GetSection("WebSetting")["ImageMaxSize"] ?? throw new ArgumentNullException("WebSetting/ImageMaxSize is null"));
    maxUploadSize = maxUploadSize * 1024 * 1024;
    // application running on IIS:
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = maxUploadSize;
    });
    builder.Services.Configure<FormOptions>(options =>
    {
        //options.ValueLengthLimit = int.MaxValue;        
        options.MultipartBodyLengthLimit = maxUploadSize; // default value is: 128 MB
        //options.MultipartHeadersLengthLimit = int.MaxValue;
    });
    #endregion

    #region AntiForgeryToken Setting
    builder.Services.RegisterAntiforgeryServices(configuration);
    #endregion


    #region Cus Services DI 
    builder.Services.RegisterCusServices(configuration);
    #endregion

    builder.Services.RegisterHstsAndHttpsRedirectionServices(configuration);

    #region Swagger UI
    // .AddEndpointsApiExplorer() was created to support Minimal Api's.
    // builder.Services.AddEndpointsApiExplorer();
    builder.Services.SwaggerDI(configuration);
    #endregion

    var app = builder.Build();

    // 取得 EnvironmentName
    logger.Info("ENV:" + app.Environment.EnvironmentName);

    #region Middleware Pipeline 
    app.ConfigureForceHttps(configuration);                 // 在某些情況下，可能無法將轉送的標頭新增至透過 Proxy 傳送給應用程式 騙過它
    app.ConfigureForwardedHeaders();                        // 轉送的標頭中介
    app.UseResponseCompression();

    if (app.Environment.IsDevelopment())                // 處理UseMyExceptionHandler之前的 Exception 
    {
        // Swagger 
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/StateError");
    }
    app.UseSession();
    app.UseMyExceptionHandler();                        // 自定義 exception 處理

    app.ConfigureHstsAndHttpsRedirection(configuration);// HSTS HTTPS 轉導設定


    app.ConfigureStaticFiles(configuration);            // wwwroot static files configure

    app.ConfigureHandle404Error();                      // Handle 404 erros
    app.UseRedirectToIndex();                           // default Redirect to Index page 
    app.UseRouting();
    app.UseCors("CORSPolicy");                          // 跨域資源允許設定

    app.ConfigureCookiePolicy(configuration);           // define cookie policy 
    #endregion

    #region Endpoint Map
    app.ConfigureMapEndpoints();                        // endpoint mapping
    #endregion

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception" + JsonConvert.SerializeObject(exception));
}
finally
{
    logger.Info("Application Shutdown");
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
// for unit test
public partial class Program { }