namespace Sample.Api.Models.DTO
{
    public class HttpContextItemKey
    {
        /// <summary>
        /// Log Request track id
        /// </summary>
        public static string ActionId { get; } = "ActionId";

        /// <summary>
        /// Log Call Api Helper ActionId
        /// </summary>
        public static string CallApiActionId { get; } = "CallApiActionId";


        public const string VerifiedSystemInfo = "VerifiedSystemInfo";
    }
}
