using System.Text;
using Microsoft.Extensions.Logging;

namespace Wow.Blizzard.Client;

internal class BlizzardTokenClient : IBlizzardTokenClient
{
    private readonly ILogger<BlizzardTokenClient> _logger;
    private readonly IBlizzardTokenApi            _tokenApi;

    public BlizzardTokenClient(IBlizzardTokenApi tokenApi, ILogger<BlizzardTokenClient> logger)
    {
        _tokenApi = tokenApi;
        _logger   = logger;
    }

    public async Task<TokenResponse> GetTokenAsync(string clientId, string clientSecret)
    {
        _tokenApi.Authorization =
            "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var response = await _tokenApi.GetAccessTokenAsync(new TokenParameters());

        if (!response.ResponseMessage.IsSuccessStatusCode)
        {
            var content    = response.StringContent;
            var reason     = response.ResponseMessage.ReasonPhrase;
            var statusCode = response.ResponseMessage.StatusCode;
            _logger.LogError("Failed to get token. Reason: {Reason}, Status Code: {StatusCode}, Content: {Content}",
                             reason, statusCode, content);
        }

        var data         = response.GetContent();
        var responseDate = response.ResponseMessage.Headers.Date;
        return new TokenResponse
        {
            AccessToken = data.AccessToken, ExpiresIn = data.ExpiresIn, TokenType = data.TokenType, Scope = data.Scope,
            CreatedAt   = responseDate.GetValueOrDefault()
        };
    }
}