using Newtonsoft.Json;

namespace CDD.Web.Libs
{
    public class MyException : Exception
    {
        /// <summary>
        /// 自定義 錯誤 Code 
        /// </summary>
        public virtual string Code { get; }

        /// <summary>
        /// 顯示錯誤標題
        /// </summary>
        public virtual string Title { get; }

        /// <summary>
        /// Message
        /// </summary>
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public object? DebugObj { get; }


        public MyException(string code, string title, string message, object? debugObj) : base(message)
        {
            this.Code = code;
            this.Title = title;
            this.DebugObj = debugObj;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
