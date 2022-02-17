using BotFights.Wordle.Bots;
using BotFights.Wordle.Models;
using BotFights.Wordle.Services.WordList;
using Microsoft.Extensions.Logging;

namespace BotFights.Wordle.Services;

class WordleBotPlayerService : IWordleBotPlayerService
{
    private readonly IWordListProvider _listProvider;
    private readonly IEnumerable<IWordleBot> _bots;
    private readonly ILogger<WordleBotPlayerService> _logger;
    private readonly Random _random;

    public WordleBotPlayerService(IWordListProvider listProvider, IEnumerable<IWordleBot> bots, ILogger<WordleBotPlayerService> logger)
    {
        _listProvider = listProvider;
        _bots = bots;
        _logger = logger;
        _random = new Random();
    }

    public async Task<Dictionary<string, List<Game>>> RunBots(int numberOfGames)
    {
        var secrets = await _listProvider.GetWordList("secrets.txt");
        var words = await _listProvider.GetWordList("wordlist.txt");

        Dictionary<string, List<Game>> botGameDict = new();

        foreach (var bot in _bots)
        {
            _logger.LogInformation("Running {Num} Games for Bot {Bot}", numberOfGames, bot.GetType().Name);
            List<Game> games = new List<Game>();
            for (var i = 0; i < numberOfGames; i++)
            {
                var randomIndex = _random.Next(secrets.Count);
                var word = secrets[randomIndex];

                var game = await RunBot(bot, word, words);
                game.Number = i;
                games.Add(game);
            }

            botGameDict.Add(bot.GetType().Name, games);
        }

        return botGameDict;
    }

    private static async Task<Game> RunBot(IWordleBot bot, string secret, List<string> words)
    {
        var game = new Game { Tries = new List<Try>() };
        while (true)
        {
            var guess = await bot.GetNextGuess(game.Tries);

            if (string.IsNullOrEmpty(guess))
            {
                return game;
            }
            
            var @try = GetTryResult(guess, secret);
            game.Tries.Add(@try);

            if (@try.Positions.Any(x => x.Result != PositionResult.Correct))
            {
                continue;
            }

            game.Solved = true;
            return game;
        }
    }

    private static Try GetTryResult(string guess, string secret)
    {
        var result = new Try
        {
            TryString = guess,
            Positions = new List<Position>()
        };
        var index = 0;
        foreach (var c in guess)
        {
            PositionResult r = secret.Contains(c)
                ? secret.IndexOf(c) == index ? PositionResult.Correct : PositionResult.Close
                : PositionResult.Miss;
            result.Positions.Add(new Position
            {
                Index = index,
                Char = c,
                Result = r
            });
            index++;
        }

        return result;
    }
}