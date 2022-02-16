using System.Net.Http.Headers;
using System.Text;
using BotFights.Core.Configuration;
using Microsoft.Extensions.Options;

namespace BotFights.Core.API.HttpMessageHandlers;

public class HttpAuthMessageHandler : DelegatingHandler
{
    private readonly IBotFightsConfiguration _configuration;

    public HttpAuthMessageHandler(IBotFightsConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authenticationString = $"{_configuration.UserId}:{_configuration.Password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return response;
    }
}