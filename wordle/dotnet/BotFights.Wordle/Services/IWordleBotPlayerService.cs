using BotFights.Wordle.Models;

namespace BotFights.Wordle.Services;

internal interface IWordleBotPlayerService
{
    Task<Dictionary<string, List<Game>>> RunBots(int numberOfGames);
}