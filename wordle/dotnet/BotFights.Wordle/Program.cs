using BotFights.Core;
using BotFights.Core.API;
using BotFights.Core.Configuration;
using BotFights.Wordle.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddBotFights(typeof(Program));
builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

var config = app.Services.GetRequiredService<IBotFightsConfiguration>();
var botFightsAPI = app.Services.GetRequiredService<IBotFightsAPI>();
var user = await botFightsAPI.GetUser(config.UserId);
logger.LogInformation("User: {@User}", user);

var api = app.Services.GetRequiredService<IWordleBotFightsAPI>();
var fight = await api.UpdateGuesses("foh2cvva", new GuessesPayload
{
    Guesses = new List<string>
    {
        "irate",
        "angry",
        "largo",
        "margs",
        "dargs",
        "garbs"
    }
});
logger.LogInformation("Fight: {@Fight}", fight);