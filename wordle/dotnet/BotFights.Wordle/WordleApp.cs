using BotFights.Core;
using BotFights.Core.API;
using BotFights.Wordle.API;
using BotFights.Wordle.Bots;
using BotFights.Wordle.Models;
using BotFights.Wordle.Services;
using BotFights.Wordle.Services.WordList;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BotFights.Wordle;

public class WordleApp
{
    public static void Run(Action<IServiceCollection> configureServices = null)
    {
        var app = BuildApp(configureServices);
        AddBotFightsCommand(app);
        AddUserCommand(app);
        AddBotTestCommand(app);
        app.Run();
    }

    private static CoconaApp BuildApp(Action<IServiceCollection> configureServices)
    {
        var builder = CoconaApp.CreateBuilder();
        builder.Services.AddBotFights(builder.Configuration);
        builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
        builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
        builder.Services.AddTransient<IWordleFightService, WordleFightService>();
        builder.Services.AddSingleton<IWordListProvider, FileWordListProvider>();
        builder.Services.AddSingleton<IWordleBotPlayerService, WordleBotPlayerService>();
        builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

        configureServices?.Invoke(builder.Services);

        builder.Services.Scan(scan => scan.FromCallingAssembly().AddClasses(c => c.AssignableTo<IWordleBot>()).As<IWordleBot>());

        var app = builder.Build();
        return app;
    }

    private static void AddUserCommand(CoconaApp app)
    {
        app.AddCommand("user",
            async ([Option('u')] string userId, [FromService] IBotFightsAPI botFightsAPI, [FromService] ILogger<Program> logger) =>
            {
                var user = await botFightsAPI.GetUser(userId);
                logger.LogInformation("User: {@User}", user);

                return 0;
            });
    }

    private static void AddBotFightsCommand(CoconaApp app)
    {
        app.AddCommand("botfights",
            async ([FromService] IEnumerable<IWordleBot> bots,
                [FromService] IWordleFightService service,
                [FromService] IWordListProvider wordListProvider,
                [FromService] ILogger<Program> logger) =>
            {
                var botList = bots.ToList();
                if (!botList.Any())
                {
                    logger.LogError("No bots found. Create a class that Inherits the IWordleBot interface to run a bot");
                    return 1;
                }

                if (botList.Count > 1)
                {
                    logger.LogError(
                        "More than one bot found. Either delete or comment out a bot to remove it from this list. Bots: {@Bots}",
                        botList.Select(x => x.GetType().Name));
                    return 1;
                }

                var bot = botList.First();

                var words = await wordListProvider.GetWordList("wordlist.txt");
                var fight = await service.CreateFight("test");
                logger.LogInformation("Created Fight: {FightId}", fight.Id);
                await Task.Delay(1000);

                while (fight.Games.Any(x => !x.Solved))
                {
                    var guesses = new List<Guess>();
                    foreach (var unSolvedGame in fight.Games.Where(x => !x.Solved))
                    {
                        var guessStr = await bot.GetNextGuess(unSolvedGame.Tries, words);
                        if (guessStr == null)
                        {
                            continue;
                        }

                        guesses.Add(new Guess
                        {
                            GameNumber = unSolvedGame.Number,
                            GuessString = guessStr?.ToLower()
                        });
                    }

                    fight = await service.TryGuesses(fight, guesses);
                    await Task.Delay(1000);
                    logger.LogInformation("Games remaining: {Games}", fight.Games.Count(x => !x.Solved));
                }

                foreach (var x in fight.Games)
                {
                    logger.LogInformation("Game Result: {@Fight}", new
                    {
                        Number = x.Number,
                        Solution = x.Tries.LastOrDefault()?.TryString,
                        NumberOfTries = x.Tries.Count,
                    });
                }

                return 0;
            });
    }

    private static void AddBotTestCommand(CoconaApp app)
    {
        app.AddCommand("test", async (int count, bool showGuesses, bool hideGameResults, [FromService] IWordleBotPlayerService service,
            [FromService] IWordListProvider wordListProvider,
            [FromService] ILogger<Program> logger) =>
        {
            var results = await service.RunBots(count);

            foreach (var botGamesPair in results)
            {
                LogBotResults(hideGameResults, botGamesPair, showGuesses, logger);
            }

            return 0;
        });
    }

    private static void LogBotResults(bool hideGameResults,
        KeyValuePair<string, List<Game>> valuePair,
        bool showGuesses,
        ILogger<Program> logger)
    {
        if (hideGameResults)
        {
            LogGameStats(valuePair, logger);
            return;
        }

        foreach (var game in valuePair.Value)
        {
            var logGame = new
            {
                Number = game.Number,
                Solved = game.Solved,
                Solution = game.Tries.LastOrDefault()?.TryString,
                NumberOfTries = game.Tries.Count,
                Guesses = showGuesses ? game.Tries.Select(x => x.TryString) : new[] { "Omitted" }
            };
            if (game.Solved)
            {
                logger.LogInformation("Bot: {Bot} Game Result: {@Game}", valuePair.Key, logGame);
            }
            else
            {
                logger.LogWarning("Bot: {Bot} Game Result: {@Game}", valuePair.Key, logGame);
            }
        }

        LogGameStats(valuePair, logger);
    }

    private static void LogGameStats(KeyValuePair<string, List<Game>> valuePair, ILogger<Program> logger)
    {
        logger.LogInformation("Game Stats: {@Stats}", new
        {
            Bot = valuePair.Key,
            Tries = new
            {
                Min = valuePair.Value.Min(x => x.Tries.Count),
                Average = valuePair.Value.Average(x => x.Tries.Count),
                Mix = valuePair.Value.Max(x => x.Tries.Count),
            }
        });
    }
}