using BotFights.Wordle.Models;

namespace BotFights.Wordle;

class SampleWordleBot : IWordleBot
{
    public Task<string> GetNextGuess(List<Try> tries)
    {
        throw new NotImplementedException();
    }
}