namespace CDD.Web.Models.DTO
{
    public class XSRFKey
    {
        public static string CookieName { get; } = "XSRF-TOKEN";
        public static string HeaderName { get; } = "X-CSRF-TOKEN-HEADERNAME";
    }
}
