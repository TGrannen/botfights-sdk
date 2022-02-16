using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotFights.Wordle.API;

class PropertyArrayOfStringsConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var message = new List<string>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");

            reader.Read();
            var str = reader.GetString() ?? string.Empty;
            message.Add(str);
        }

        return message;
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        int i = 0;
        foreach (var s in value)
        {
            writer.WritePropertyName(i.ToString());
            writer.WriteStringValue(s);
            i++;
        }

        writer.WriteEndObject();
    }
}