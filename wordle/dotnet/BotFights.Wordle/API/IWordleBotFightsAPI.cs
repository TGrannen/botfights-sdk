using Refit;

namespace BotFights.Wordle.API;

public interface IWordleBotFightsAPI
{
    [Put("/game/wordle")]
    Task<WordleFight> CreateFight();

    [Patch("/game/wordle/{fightId}")]
    Task<WordleFight> UpdateGuesses(string fightId, [Body] GuessesPayload payload);
}