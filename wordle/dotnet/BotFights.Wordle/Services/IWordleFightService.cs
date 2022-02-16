namespace BotFights.Wordle.Services;

public interface IWordleFightService
{
    Task<WordleFight> CreateFight(string @event);
    Task<WordleFight> TryGuesses(WordleFight fight, List<Guess> guesses);
}