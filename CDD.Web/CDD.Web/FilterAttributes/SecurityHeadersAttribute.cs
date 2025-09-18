using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CDD.Web.FilterAttributes
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        private readonly ConfigurationSection _SecurityHeadersEnableState;

        private readonly ConfigurationSection _securityHeaders;

        private readonly ConfigurationSection _cspHeaders;

        public SecurityHeadersAttribute(IConfiguration config)
        {
            _SecurityHeadersEnableState = (ConfigurationSection)config.GetSection("SecurityHeadersEnableState");
            _securityHeaders = (ConfigurationSection)config.GetSection("SecurityHeaders");
            _cspHeaders = (ConfigurationSection)config.GetSection("SecurityHeaders:CSP");
        }

        /// <summary>
        /// ViewResult 加上 Security Headers 
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            IActionResult result = context.Result;
            bool enableSecurityHeaders = _SecurityHeadersEnableState.GetValue("Enable", false);
            if (result is ViewResult && enableSecurityHeaders && !context.HttpContext.Response.HasStarted)
            {
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options") && _SecurityHeadersEnableState.GetValue("X-Content-Type-Options", false))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", _securityHeaders["X-Content-Type-Options"]);
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("Cache-Control") && _SecurityHeadersEnableState.GetValue("Cache-Control", false))
                {
                    context.HttpContext.Response.Headers.Add("Cache-Control", _securityHeaders["Cache-Control"]);
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("X-XSS-Protection") && _SecurityHeadersEnableState.GetValue("X-XSS-Protection", false))
                {
                    context.HttpContext.Response.Headers.Add("X-XSS-Protection", _securityHeaders["X-XSS-Protection"]);
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("X-FRAME-OPTIONS") && _SecurityHeadersEnableState.GetValue("X-FRAME-OPTIONS", false))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "DENY");
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy") && _SecurityHeadersEnableState.GetValue("Referrer-Policy", false))
                {
                    context.HttpContext.Response.Headers.Add("Referrer-Policy", _securityHeaders["Referrer-Policy"]);
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("Permissions-Policy") && _SecurityHeadersEnableState.GetValue("Permissions-Policy", false))
                {
                    context.HttpContext.Response.Headers.Add("Permissions-Policy", _securityHeaders["Permissions-Policy"]);
                }

                // Add 白名單
                if (_SecurityHeadersEnableState.GetValue("CSP", false))
                {
                    string csp = string.Join(" ", _cspHeaders.Get<string[]>());
                    if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                    {
                        context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                    }
                    if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                    {
                        context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                    }
                    if (!context.HttpContext.Response.Headers.ContainsKey("X-WebKit-CSP"))
                    {
                        context.HttpContext.Response.Headers.Add("X-WebKit-CSP", csp);
                    }
                }
            }
        }
    }
}

