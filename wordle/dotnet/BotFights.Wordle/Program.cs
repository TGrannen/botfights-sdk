using BotFights.Core;
using BotFights.Core.API;
using BotFights.Wordle;
using BotFights.Wordle.API;
using BotFights.Wordle.Models;
using BotFights.Wordle.Services;
using BotFights.Wordle.Services.WordList;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddBotFights(builder.Configuration);
builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
builder.Services.AddTransient<IWordleFightService, WordleFightService>();
builder.Services.AddSingleton<IWordListProvider, FileWordListProvider>();
builder.Services.AddTransient<IWordleBot, SampleWordleBot>();
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();

app.AddCommand("botfights",
    async ([FromService] IWordleBot bot,
        [FromService] IWordleFightService service,
        [FromService] ILogger<Program> logger) =>
    {
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

app.AddCommand("user",
    async ([Option('u')] string userId, [FromService] IBotFightsAPI botFightsAPI, [FromService] ILogger<Program> logger) =>
    {
        var user = await botFightsAPI.GetUser(userId);
        logger.LogInformation("User: {@User}", user);

        return 0;
    });

app.Run();