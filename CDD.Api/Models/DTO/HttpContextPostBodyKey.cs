namespace CDD.Api.Models.DTO
{
    public class HttpContextPostBodyKey
    {
        /// <summary>
        /// {System}
        /// </summary>
        public const string System = "System";

        /// <summary>
        /// AES-256{"source_id":"System", time: "{yyyy/MM/dd HH:mm:ss}", "apiKey": "ApiKey" }
        /// </summary>
        public const string Source = "Source";
    }
}
