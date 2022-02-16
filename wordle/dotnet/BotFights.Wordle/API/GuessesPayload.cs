using System.Text.Json.Serialization;

namespace BotFights.Wordle.API;

public class GuessesPayload
{
    [JsonConverter(typeof(PropertyArrayOfStringsConverter))]
    public List<string> Guesses { get; set; } = new List<string>();
}