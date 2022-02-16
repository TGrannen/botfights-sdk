using BotFights.Core.API;
using BotFights.Core.API.HttpMessageHandlers;
using BotFights.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Serilog;

namespace BotFights.Core;

public static class Extensions
{
    public static WebApplicationBuilder AddBotFights(this WebApplicationBuilder builder, Type type)
    {
        builder.Configuration.AddUserSecrets(type.Assembly);
        builder.Services.Configure<BotFightsConfiguration>(builder.Configuration.GetSection("BotFights"));
        builder.Services.AddTransient<IBotFightsConfiguration>(provider => provider.GetService<IOptions<BotFightsConfiguration>>()?.Value);
        builder.Services.AddBotFightsClient<IBotFightsAPI>();
        builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

        return builder;
    }

    public static IServiceCollection AddBotFightsClient<T>(this IServiceCollection services) where T : class
    {
        services.AddTransient<HttpAuthMessageHandler>();
        services.AddTransient<HttpLoggingMessageHandler>();
        services
            .AddRefitClient<T>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<BotFightsConfiguration>>().Value;
                client.BaseAddress = new Uri(config.API);
            })
            .AddHttpMessageHandler<HttpAuthMessageHandler>()
            .AddHttpMessageHandler<HttpLoggingMessageHandler>();
        return services;
    }
}