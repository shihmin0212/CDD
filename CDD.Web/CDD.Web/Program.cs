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
    // �q�R�O�C�ѼƤ����o environment
    string? environmentFromArgs = DependencyInjectionSetup.GetCommandLineArgument(args, "environment");
    logger.Info("environmentFromArgs:" + environmentFromArgs ?? "Not Found");

    string environment = (environmentFromArgs ?? aspnetEnvironment ?? "Production");

    // �ϥ� WebApplicationOptions �]�w����
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

    #region �]�w�W�Ǥj�p
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

    // ���o EnvironmentName
    logger.Info("ENV:" + app.Environment.EnvironmentName);

    #region Middleware Pipeline 
    app.ConfigureForceHttps(configuration);                 // �b�Y�Ǳ��p�U�A�i��L�k�N��e�����Y�s�W�ܳz�L Proxy �ǰe�����ε{�� �F�L��
    app.ConfigureForwardedHeaders();                        // ��e�����Y����
    app.UseResponseCompression();

    if (app.Environment.IsDevelopment())                // �B�zUseMyExceptionHandler���e�� Exception 
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
    app.UseMyExceptionHandler();                        // �۩w�q exception �B�z

    app.ConfigureHstsAndHttpsRedirection(configuration);// HSTS HTTPS ��ɳ]�w


    app.ConfigureStaticFiles(configuration);            // wwwroot static files configure

    app.ConfigureHandle404Error();                      // Handle 404 erros
    app.UseRedirectToIndex();                           // default Redirect to Index page 
    app.UseRouting();
    app.UseCors("CORSPolicy");                          // ���귽���\�]�w

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