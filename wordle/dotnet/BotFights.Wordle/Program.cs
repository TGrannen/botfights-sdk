using BotFights.Core;
using BotFights.Wordle;
using BotFights.Wordle.API;
using BotFights.Wordle.Models;
using BotFights.Wordle.Services;
using BotFights.Wordle.Services.WordList;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddBotFights(typeof(Program));
builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
builder.Services.AddTransient<IWordleFightService, WordleFightService>();
builder.Services.AddSingleton<IWordListProvider, FileWordListProvider>();
builder.Services.AddTransient<IWordleBot, SampleWordleBot>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// var config = app.Services.GetRequiredService<IBotFightsConfiguration>();
// var botFightsAPI = app.Services.GetRequiredService<IBotFightsAPI>();
// var user = await botFightsAPI.GetUser(config.UserId);
// logger.LogInformation("User: {@User}", user);

var service = app.Services.GetRequiredService<IWordleFightService>();
var bot = app.Services.GetRequiredService<IWordleBot>();

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