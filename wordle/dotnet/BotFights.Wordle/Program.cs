using BotFights.Core;
using BotFights.Core.API;
using BotFights.Core.Configuration;
using BotFights.Wordle.API;
using BotFights.Wordle.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddBotFights(typeof(Program));
builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
builder.Services.AddTransient<IWordleFightService, WordleFightService>();
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

var config = app.Services.GetRequiredService<IBotFightsConfiguration>();
var botFightsAPI = app.Services.GetRequiredService<IBotFightsAPI>();
var user = await botFightsAPI.GetUser(config.UserId);
logger.LogInformation("User: {@User}", user);

var service = app.Services.GetRequiredService<IWordleFightService>();

var fight = await service.CreateFight("test");
logger.LogInformation("Created Fight: {@Fight}", fight);

fight = await service.TryGuesses(fight, new List<Guess>
{
    new() { GameNumber = 0, GuessString = "irate" }
});

logger.LogInformation("Updated Fight: {@Fight}", fight);