using System.Text.Json;
using System.Text.Json.Serialization;
using BotFights.Wordle.Models;

namespace BotFights.Wordle.API;

class GuessPropertyArrayOfStringsConverter : JsonConverter<List<Guess>>
{
    public override List<Guess> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guesses = new List<Guess>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");

            var propName = reader.GetString();
            reader.Read();
            var str = reader.GetString() ?? string.Empty;
            guesses.Add(new Guess
            {
                GameNumber = int.Parse(propName),
                GuessString = str
            });
        }

        return guesses;
    }

    public override void Write(Utf8JsonWriter writer, List<Guess> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var s in value)
        {
            writer.WritePropertyName(s.GameNumber.ToString());
            writer.WriteStringValue(s.GuessString);
        }

        writer.WriteEndObject();
    }
}