namespace Sample.Api.Models.Response
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeneralResp<T> where T : new()
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; } = false;

        /// <summary>
        /// 0;-1: No permission 0: Successful 1: No data
        /// </summary>
        public int Code { get; set; } = -1;

        /// <summary>
        /// null
        /// </summary>
        public string? Exception { get; set; } = String.Empty;

        /// <summary>
        /// null
        /// </summary>
        public string? Message { get; set; } = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        public T? Result { get; set; }

    }
}
