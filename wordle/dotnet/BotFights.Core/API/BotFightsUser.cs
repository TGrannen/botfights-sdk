using System.Text.Json.Serialization;

namespace BotFights.Core.API;

public class BotFightsUser
{
    [JsonPropertyName("user_id")] public string Id { get; set; }
    [JsonPropertyName("github")] public string GitHub { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("link")] public string Link { get; set; }
    [JsonPropertyName("btc_address")] public string BTCAddress { get; set; }
}