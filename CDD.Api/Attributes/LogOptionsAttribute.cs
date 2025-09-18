namespace Sample.Api.Attributes
{
    /// <summary>
    /// 停用logging Filter
    /// </summary>
    public class IgnoreLogging : Attribute
    {

    }

    public class LogOptions : Attribute
    {
        #region Log Request Options
        /// <summary>
        /// 是否紀錄 IncomeRequest BASIC INFO (ID;IP;URL;HttpMethod;DateTime)
        /// </summary>
        public bool IsRecord_RequestInfo { get; set; } = true;

        /// <summary>
        /// 是否紀錄 IncomeRequest Model
        /// </summary>
        public bool IsRecord_RequestModel { get; set; } = false;

        /// <summary>
        /// 去識別化 regex rules
        /// </summary>
        public string[]? NoLogReqPatterns { get; set; } = null;
        #endregion


        #region Log Response Options
        /// <summary>
        /// 是否紀錄 回應格式
        /// </summary>
        public bool IsRecord_ResponseInfo { get; set; } = true;

        /// <summary>
        /// 回應格式 是否包含Request Model + Header
        /// </summary>
        public bool IsRecord_ResponseFullInfo { get; set; } = false;

        /// <summary>
        /// 去識別化 regex rules
        /// </summary>
        public string[]? NoLogRespPatterns { get; set; } = null;
        #endregion

        /// <summary>
        /// 欲取代字串
        /// </summary>
        public string? ReplaceStr = null;
    }
}
