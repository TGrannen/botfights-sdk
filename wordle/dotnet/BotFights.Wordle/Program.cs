using BotFights.Core;
using BotFights.Core.API;
using BotFights.Core.Configuration;
using BotFights.Wordle.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddBotFights(typeof(Program));
builder.Services.AddBotFightsClient<IWordleBotFightsAPI>();
var app = builder.Build();

var api = app.Services.GetRequiredService<IBotFightsAPI>();
var config = app.Services.GetRequiredService<IBotFightsConfiguration>();
var user = await api.GetUser(config.UserId);
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("User: {@User}", user);

