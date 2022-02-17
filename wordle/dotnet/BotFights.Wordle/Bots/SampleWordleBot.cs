using BotFights.Wordle.Models;

namespace BotFights.Wordle.Bots;

class SampleWordleBot : IWordleBot
{
    public Task<string> GetNextGuess(List<Try> tries, List<string> allWords)
    {
        var allPositions = tries.Where(x => x.Positions != null).SelectMany(x => x.Positions).ToList();
        var correctPositions = GetCorrectChars(allPositions);
        var words = GetRemainingWords(allWords, correctPositions);

        var triedWords = tries.ToDictionary(x => x.TryString);
        return Task.FromResult(words.FirstOrDefault(x => !triedWords.ContainsKey(x)));
    }

    private static IEnumerable<string> GetRemainingWords(IEnumerable<string> allWords, List<Position> correctPositions)
    {
        var enumerable = allWords;
        if (!correctPositions.Any())
        {
            return enumerable.OrderBy(x => x).ToList();
        }

        foreach (var position in correctPositions)
        {
            enumerable = enumerable.Where(x => x[position.Index] == position.Char);
        }

        return enumerable.OrderBy(x => x).ToList();
    }

    private static List<Position> GetCorrectChars(List<Position> positions)
    {
        return positions
            .Where(x => x.Result is PositionResult.Correct)
            .GroupBy(x => x.Index)
            .Select(x => x.First())
            .ToList();
    }
}