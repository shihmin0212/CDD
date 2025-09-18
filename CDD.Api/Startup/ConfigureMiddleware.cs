using Microsoft.AspNetCore.HttpOverrides;

namespace CDD.Api.Startup
{
    public static class ConfigureMiddleware
    {
        public static WebApplication ConfigureForceHttps(this WebApplication app, ConfigurationManager configuration)
        {
            // 在某些情況下，可能無法將轉送的標頭新增至透過 Proxy 傳送給應用程式 
            if (Convert.ToBoolean(configuration.GetSection("AppSetting")["ForceHttps"]))
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next(context);
                });
            }
            return app;
        }

        /// <summary>
        /// Get Right Headers From Proxy / For Get User IP
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureForwardedHeaders(this WebApplication app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });
            return app;
        }

        public static WebApplication ConfigureHstsAndHttpsRedirection(this WebApplication app, ConfigurationManager configuration)
        {
            if (Convert.ToBoolean(configuration.GetSection("AppSetting")["UseHsts"]))
            {
                app.UseHsts();
            }

            if (Convert.ToBoolean(configuration.GetSection("AppSetting")["UseHttpsRedirection"]))
            {
                app.UseHttpsRedirection();
            }
            return app;
        }

        /// <summary>
        /// Swagger Config
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureSwagger(this WebApplication app, ConfigurationManager configuration)
        {
            if (Convert.ToBoolean(configuration.GetSection("WebSetting")["UseSwaggerGUI"]))
            {
                // Swagger 
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }

    }
}
