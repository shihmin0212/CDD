namespace Sample.Api.Models.DTO
{
    public interface ILogBase
    {
        public int ResponseStatusCode { get; set; }

        public Guid ActionId { get; set; }

        public string? Method { get; set; }

        public string? Url { get; set; }

        public string? Action { get; set; }

        public object? RequestHeaders { get; set; }

        public object? RequestModel { get; set; }

        public object? ResponseData { get; set; }

        /// <summary>
        /// 觸發/完成時間
        /// </summary>
        public object? CreateTime { get; set; }

        /// <summary>
        /// 觸發/完成時間
        /// </summary>
        public object? LastUpdate { get; set; }
    }

    public class LogBase : ILogBase
    {
        public int ResponseStatusCode { get; set; }

        public Guid ActionId { get; set; }

        public string? Method { get; set; }

        public string? Url { get; set; }

        public string? Action { get; set; }

        public object? RequestHeaders { get; set; }

        public object? RequestModel { get; set; }

        public object? ResponseData { get; set; }

        /// <summary>
        /// 觸發/完成時間
        /// </summary>
        public object? CreateTime { get; set; }

        /// <summary>
        /// 觸發/完成時間
        /// </summary>
        public object? LastUpdate { get; set; }
    }

    public interface IWebClientLog : ILogBase
    {
        public string? TraceIdentifier { get; set; }

        public string? IP { get; set; }
        /// <summary>
        /// 客製化 內容
        /// </summary>
        public object? Content { get; set; }
    }

    public class WebClientLog : LogBase, IWebClientLog
    {
        public string? TraceIdentifier { get; set; }

        public string? IP { get; set; }
        /// <summary>
        /// 客製化 內容
        /// </summary>
        public object? Content { get; set; }
    }


    public interface IApiLog
    {
        public Guid? CallApiActionId { get; set; }
    }

    public class ApiLog : LogBase, IApiLog
    {
        public Guid? CallApiActionId { get; set; }

        public long? DurationMilliseconds { get; set; }
    }

    public interface IExceptionLog : ILogBase
    {
        public string? ErrorCode { get; set; }

        /// <summary>
        /// 錯誤訊息標題
        /// </summary>
        public string? ErrorTitle { get; set; }

        /// <summary>
        /// 錯誤訊息 內容
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Exception 內容
        /// </summary>
        public object? ExceptionObj { get; set; }
    }

    public class ExceptionLog : WebClientLog, IExceptionLog
    {
        public string? ErrorCode { get; set; }

        /// <summary>
        /// 錯誤訊息標題
        /// </summary>
        public string? ErrorTitle { get; set; }

        /// <summary>
        /// 錯誤訊息 內容
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Exception 內容
        /// </summary>
        public object? ExceptionObj { get; set; }
    }
}
