using BotFights.Wordle.Models;

namespace BotFights.Wordle.Bots;

public interface IWordleBot
{
    Task<string> GetNextGuess(List<Try> tries, List<string> words);
}