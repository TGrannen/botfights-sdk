using System.Text.Json.Serialization;
using BotFights.Wordle.Services;

namespace BotFights.Wordle.API.Models;

public class WordleFightPayload
{
    [JsonPropertyName("fight_id")] public string Id { get; set; }
    [JsonPropertyName("n")] public int Number { get; set; }
    [JsonPropertyName("guess_cnt")] public int GuessCount { get; set; }
    [JsonPropertyName("round_num")] public int RoundNumber { get; set; }
    [JsonPropertyName("wordlist")] public string Wordlist { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("feedback")] 
    [JsonConverter(typeof(GuessPropertyArrayOfStringsConverter))]
    public List<Guess> Results { get; set; }
}