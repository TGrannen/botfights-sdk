namespace BotFights.Core.Configuration;

public interface IBotFightsConfiguration
{
    string API { get;  }
    string UserId { get; }
    string Password { get;  }
}