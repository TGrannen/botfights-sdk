using BotFights.Wordle.Services;

namespace BotFights.Wordle;

class SampleWordleBot : IWordleBot
{
    public Task<string> GetNextGuess(Game game)
    {
        throw new NotImplementedException();
    }
}