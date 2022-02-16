using Refit;

namespace BotFights.Wordle.API;

public interface IWordleBotFightsAPI
{
    [Put("/game/wordle")]
    Task<WordleGameResponse> GetResponse();
}