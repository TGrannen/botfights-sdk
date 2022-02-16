using Refit;

namespace BotFights.Core.API;

public interface IBotFightsAPI
{
    [Get("/user/{userId}")]
    Task<BotFightsUser> GetUser(string userId);
}