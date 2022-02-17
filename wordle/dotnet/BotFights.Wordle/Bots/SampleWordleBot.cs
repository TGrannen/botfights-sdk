using BotFights.Wordle.Models;
using BotFights.Wordle.Services.WordList;

namespace BotFights.Wordle.Bots;

class SampleWordleBot : IWordleBot
{
    private readonly IWordListProvider _listProvider;

    public SampleWordleBot(IWordListProvider listProvider)
    {
        _listProvider = listProvider;
    }

    public async Task<string> GetNextGuess(List<Try> tries)
    {
        var allPositions = tries.Where(x => x.Positions != null).SelectMany(x => x.Positions).ToList();
        var correctPositions = GetCorrectChars(allPositions);
        var allWords = await _listProvider.GetWordList();
        var words = GetRemainingWords(allWords, correctPositions);

        var triedWords = tries.ToDictionary(x => x.TryString);
        return words.FirstOrDefault(x => !triedWords.ContainsKey(x));
    }

    private IEnumerable<string> GetRemainingWords(IEnumerable<string> allWords, List<Position> correctPositions)
    {
        var enumerable = allWords;
        if (correctPositions.Any())
        {
            foreach (var position in correctPositions)
            {
                enumerable = enumerable.Where(x => x[position.Index] == position.Char);
            }
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