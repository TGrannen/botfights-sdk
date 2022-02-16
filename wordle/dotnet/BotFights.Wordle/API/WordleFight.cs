using System.Text.Json.Serialization;

namespace BotFights.Wordle.API;

public class WordleFight
{
    [JsonPropertyName("fight_id")] public string? Id { get; set; }
    [JsonPropertyName("n")] public int? Number { get; set; }
    [JsonPropertyName("guess_cnt")] public int? GuessCount { get; set; }
    [JsonPropertyName("round_num")] public int? RoundNumber { get; set; }
    [JsonPropertyName("wordlist")] public string? Wordlist { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("feedback")] 
    [JsonConverter(typeof(PropertyArrayOfStringsConverter))]
    public List<string>? Feedback { get; set; }
}