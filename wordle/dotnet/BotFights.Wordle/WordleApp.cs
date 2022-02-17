﻿using BotFights.Core;
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
        builder.Services.AddTransient<IWordleBot, SampleWordleBot>();
        builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

        configureServices?.Invoke(builder.Services);

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
            async ([FromService] IWordleBot bot,
                [FromService] IWordleFightService service,
                [FromService] IWordListProvider wordListProvider,
                [FromService] ILogger<Program> logger) =>
            {
                var words = await wordListProvider.GetWordList();
                var fight = await service.CreateFight("test");
                logger.LogInformation("Created Fight: {FightId}", fight.Id);
                await Task.Delay(1000);

                while (fight.Games.Any(x => !x.Solved))
                {
                    var guesses = new List<Guess>();
                    foreach (var unSolvedGame in fight.Games.Where(x => !x.Solved))
                    {
                        var guessStr = await bot.GetNextGuess(unSolvedGame.Tries);
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
                }

                foreach (var x in fight.Games)
                {
                    logger.LogInformation("Game Result: {@Fight}", new
                    {
                        Number = x.Number,
                        Solution = x.Tries.LastOrDefault()?.TryString,
                        NumberOfTries = x.Tries.Count
                    });
                }

                return 0;
            });
    }
}