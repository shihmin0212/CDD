using Newtonsoft.Json;

namespace CDD.Api.Converts
{
    public class TrimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override bool CanRead { get { return true; } }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {

            String text = Convert.ToString(reader.Value) ?? String.Empty;
            return !String.IsNullOrEmpty(text) ? text.Trim() : text;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
