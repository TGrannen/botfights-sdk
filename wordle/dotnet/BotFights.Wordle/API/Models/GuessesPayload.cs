using System.Text.Json.Serialization;
using BotFights.Wordle.Services;

namespace BotFights.Wordle.API.Models;

public class GuessesPayload
{
    [JsonConverter(typeof(GuessPropertyArrayOfStringsConverter))]
    public List<Guess> Guesses { get; init; } = new();
}