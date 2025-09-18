using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CDD.Api.Converts
{
    public class ReplaceSpecialCharConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override bool CanRead { get { return true; } }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            String text = Convert.ToString(reader.Value) ?? String.Empty;
            if (!String.IsNullOrWhiteSpace(text))
            {
                text = String.Join("", text.Where(c => !char.IsWhiteSpace(c)));
                return Regex.Replace(text, @"[\<\>\~\`\!\#\$\%\^\*\=\<\>\?\|\\\;\'\u0022]", String.Empty);
            }
            return String.Empty;

        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

    }
}
