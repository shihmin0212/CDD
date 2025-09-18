namespace CDD.Web.Middlewares
{
    /// <summary>
    /// 預設轉導畫面
    /// </summary>
    public class RedirectToIndex
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

        public RedirectToIndex(RequestDelegate next, ILogger<RedirectToIndex> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string path = httpContext.Request.Path;
            if (path.Equals(String.Empty) || path.Equals("/"))
            {
                string queryString = httpContext.Request.QueryString.ToString();
                httpContext.Response.Redirect("/Login" + queryString);
                return;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RedirectToIndexExtensions
    {
        public static IApplicationBuilder UseRedirectToIndex(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectToIndex>();
        }
    }
}
