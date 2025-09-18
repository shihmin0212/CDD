using Newtonsoft.Json;

namespace Sample.Api.Libs
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
            Code = "ER" + code.ToString($"D{errorDigitsNums}");
            Title = title;
            Message = message;
            Debug = debug;
        }

        public MyException GetException()
        {
            return new MyException(Code, Title, Message, Debug);
        }

        public string GetError()
        {
            return JsonConvert.SerializeObject(new { Code = Code, Title = Title, Message = Message, Debug = Debug });
        }
    }
}
