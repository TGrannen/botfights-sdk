using BotFights.Wordle.API.Models;
using Refit;

namespace BotFights.Wordle.API;

public interface IWordleBotFightsAPI
{
    [Put("/game/wordle/")]
    Task<WordleFightPayload> CreateFight([Body] CreateFightPayload payload);

    [Patch("/game/wordle/{fightId}")]
    Task<WordleFightPayload> UpdateGuesses(string fightId, [Body] GuessesPayload payload);
}