using Newtonsoft.Json;

namespace CDD.Web.Libs
{
    public struct MyExceptionMessage
    {
        public string Code { get; }

        public string Title { get; }

        public string Message { get; }

        public object? Debug { get; }

        public MyExceptionMessage(int code, string title, string message, object? debug)
        {
            int errorDigitsNums = 4;
            this.Code = "ER" + code.ToString($"D{errorDigitsNums}");
            this.Title = title;
            this.Message = message;
            this.Debug = debug;
        }

        public MyException GetException()
        {
            return new MyException(this.Code, this.Title, this.Message, this.Debug);
        }

        public string GetError()
        {
            return JsonConvert.SerializeObject(new { Code = this.Code, Title = this.Title, Message = this.Message, Debug = this.Debug });
        }
    }
}
