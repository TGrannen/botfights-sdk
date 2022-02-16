namespace BotFights.Core.Configuration;

class BotFightsConfiguration : IBotFightsConfiguration
{
    public string API { get; set; }
    public string UserId { get; set; }
    public string Password { get; set; }
}