using BotFights.Wordle.API;
using BotFights.Wordle.API.Models;
using BotFights.Wordle.Models;

namespace BotFights.Wordle.Services;

class WordleFightService : IWordleFightService
{
    private readonly IWordleBotFightsAPI _api;

    public WordleFightService(IWordleBotFightsAPI api)
    {
        _api = api;
    }

    public async Task<WordleFight> CreateFight(string @event)
    {
        var response = await _api.CreateFight(new CreateFightPayload { Event = @event });
        var result = new WordleFight
        {
            Id = response.Id,
            GuessCount = response.GuessCount,
            Number = response.Number,
            RoundNumber = response.RoundNumber,
            Status = response.Status,
            Wordlist = response.Wordlist,
            Games = response.Results.Select((_, i) => new Game { Number = i, Tries = new List<Try>() }).ToList()
        };

        return result;
    }

    public async Task<WordleFight> TryGuesses(WordleFight fight, List<Guess> guesses)
    {
        var response = await _api.UpdateGuesses(fight.Id, new GuessesPayload
        {
            Guesses = guesses
        });
        var numbers = guesses.Select(x => x.GameNumber).ToList();

        foreach (var result in response.Results.Where(x => numbers.Contains(x.GameNumber)))
        {
            var game = fight.Games.First(x => x.Number == result.GameNumber);
            var guessString = guesses.FirstOrDefault(x => x.GameNumber == game.Number)?.GuessString;
            var @try = new Try
            {
                TryString = guessString,
                Positions = Parse(guessString, result.GuessString)
            };
            if (@try.Positions.All(x => x.Result == PositionResult.Correct))
            {
                game.Solved = true;
            }
            game.Tries.Add(@try);
        }

        return fight;
    }

    private List<Position> Parse(string guessString, string resultNumberString)
    {
        var positions = guessString.Zip(resultNumberString, (c, c1) => new Position
        {
            Char = c,
            Result = c1 switch
            {
                '1' => PositionResult.Miss,
                '2' => PositionResult.Close,
                '3' => PositionResult.Correct,
                _ => throw new ArgumentOutOfRangeException()
            }
        }).ToList();
        int i = 0;
        foreach (var position in positions)
        {
            position.Index = i;
            i++;
        }

        return positions;
    }
}