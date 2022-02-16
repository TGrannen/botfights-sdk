using BotFights.Wordle.Models;

namespace BotFights.Wordle;

public interface IWordleBot
{
    Task<string> GetNextGuess(List<Try> tries);
}