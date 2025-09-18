namespace CDD.Web.Models.DTO
{
    public class HttpContextHeaderKey
    {
        /// <summary>
        /// Log Request track id
        /// </summary>
        public static string SystemID { get; } = "x-system";

        /// <summary>
        /// Log Request track id
        /// </summary>
        public static string ApiKey { get; } = "x-source";
    }
}
