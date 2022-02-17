using System.Text.Json;
using BotFights.Core.API;
using BotFights.Core.API.HttpMessageHandlers;
using BotFights.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Refit;
using Serilog;

namespace BotFights.Core;

public static class Extensions
{
    public static IServiceCollection AddBotFights(this IServiceCollection builder, IConfiguration configuration)
    {
        builder.Configure<BotFightsConfiguration>(configuration.GetSection("BotFights"));
        builder.AddTransient<IBotFightsConfiguration>(provider => provider.GetService<IOptions<BotFightsConfiguration>>()?.Value);
        builder.AddBotFightsClient<IBotFightsAPI>();
        var registry = builder.AddPolicyRegistry();

        registry.Add("retry", GetRetryPolicy());
        registry.Add("circuit", GetCircuitBreakerPolicy());

        return builder;
    }

    public static IServiceCollection AddBotFightsClient<T>(this IServiceCollection services) where T : class
    {
        services.AddTransient<HttpAuthMessageHandler>();
        services.AddTransient<HttpLoggingMessageHandler>();
        services
            .AddRefitClient<T>()
            .AddPollyContextLoggingNoOpPolicy<T>()
            .AddPolicyHandlerFromRegistry("circuit")
            .AddPolicyHandlerFromRegistry("retry")
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<BotFightsConfiguration>>().Value;
                client.BaseAddress = new Uri(config.API);
            })
            .AddHttpMessageHandler<HttpAuthMessageHandler>()
            .AddHttpMessageHandler<HttpLoggingMessageHandler>()
            ;
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
                onBreak: (result, state, duration, context) =>
                {
                    context.GetLogger().LogWarning("CircuitBreaker PreviousState:{PreviousState} State:{State} Duration {Duration:#}",
                        state, CircuitState.Open, duration.TotalSeconds);
                },
                onReset: context => { context.GetLogger().LogWarning("CircuitBreaker State:{State}", CircuitState.Closed); },
                () => { });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 1000),
                onRetry: (exception, duration, retryCount, context) =>
                {
                    context.GetLogger()
                        .LogWarning("Retry Number: {RetryCount}  Waiting: {Duration:#}ms, due to: {Message}",
                            retryCount,
                            duration.TotalMilliseconds,
                            exception.Exception?.Message ?? exception.Result.ToString());
                });
    }
}