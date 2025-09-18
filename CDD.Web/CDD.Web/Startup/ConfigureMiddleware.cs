using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using CDD.Web.Libs;

namespace CDD.Web.Startup
{
    /// <summary>
    /// Clean up program.cs
    /// </summary>
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
        /// wwwroot security headers
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static WebApplication ConfigureStaticFiles(this WebApplication app, ConfigurationManager configuration)
        {
            ConfigurationSection SecurityHeadersEnableState = (ConfigurationSection)configuration.GetSection("SecurityHeadersEnableState");
            // wwwroot security headers
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (SecurityHeadersEnableState.GetValue("Enable", false) && SecurityHeadersEnableState.GetValue("CSP", false))
                    {
                        string csp = string.Join(" ", configuration.GetSection("SecurityHeaders:CSP").Get<string[]>());

                        if (ctx.File.Name.ToLower().EndsWith(".html"))
                        {
                            ctx.Context.Response.Headers.Append("Cache-Control", "no-store, max-age=0");
                        }
                        /* 2023-03 弱掃存有stored xss 風險 header 將於 DMZ IIS 加上
                        string csp = string.Join(" ", configuration.GetSection("SecurityHeaders:CSP").Get<string[]>());
                        ctx.Context.Response.Headers.Append("Content-Security-Policy", csp);
                        ctx.Context.Response.Headers.Append("X-Content-Security-Policy", csp);
                        ctx.Context.Response.Headers.Append("X-WebKit-CSP", csp);*/
                    }
                }
            });
            return app;
        }

        /// <summary>
        ///  Handle 404 erros /OOA/SourceNotFoundError
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureHandle404Error(this WebApplication app)
        {
            app.Use(async (ctx, next) =>
            {
                await next();
                if ((ctx.Response.StatusCode == 404 || ctx.Response.StatusCode == 405) && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    throw MyExceptionList.SourceNotFoundError().GetException();
                }
            });
            return app;
        }

        /// <summary>
        ///  define cookie policy
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureCookiePolicy(this WebApplication app, ConfigurationManager configuration)
        {
            app.UseCookiePolicy(new CookiePolicyOptions         // define cookie policy
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = Convert.ToBoolean(configuration.GetSection("CookieConfig")["SecureCookie"]) ?
                    CookieSecurePolicy.Always : CookieSecurePolicy.None,
                MinimumSameSitePolicy = SameSiteMode.None
            });
            return app;
        }

        /// <summary>
        /// Swagger Config
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                // Swagger 
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }

        /// <summary>
        /// Endpoints
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureMapEndpoints(this WebApplication app)
        {
            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });*/
            app.MapDefaultControllerRoute();
            return app;
        }
    }
}
