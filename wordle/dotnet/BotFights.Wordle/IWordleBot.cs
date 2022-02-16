using BotFights.Wordle.Services;

namespace BotFights.Wordle;

public interface IWordleBot
{
    Task<string> GetNextGuess(Game game);
}