using System.Text.Json;
using System.Text.Json.Serialization;

namespace AngularToDo.Server
{
    // Allows JSON booleans to be written as true/false or as numbers (0 = false, 1 = true).
    public class JsonBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number => reader.GetInt32() != 0,
                JsonTokenType.String when bool.TryParse(reader.GetString(), out var value) => value,
                _ => throw new JsonException($"Cannot convert {reader.TokenType} to Boolean.")
            };
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
