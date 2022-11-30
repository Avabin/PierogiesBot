using System.Collections.Immutable;
using GrainInterfaces;
using GrainInterfaces.Wow.Blizzard;
using Microsoft.Extensions.Logging;

namespace Wow.Blizzard.Client;

internal class BlizzardRealmsClient : IBlizzardRealmsClient
{
    private readonly IBlizzardApi                  _blizzardApi;
    private readonly ILogger<BlizzardRealmsClient> _logger;
    private readonly Lazy<IBlizzardTokenGrain>     _tokenGrain;


    public BlizzardRealmsClient(IBlizzardApi                  blizzardApi, IClusterClient clusterClient,
                                ILogger<BlizzardRealmsClient> logger)
    {
        _blizzardApi = blizzardApi;
        _logger      = logger;

        _tokenGrain = new Lazy<IBlizzardTokenGrain>(clusterClient.GetBlizzardTokenGrain);
    }

    private IBlizzardTokenGrain TokenGrain => _tokenGrain.Value;

    public async Task<ImmutableList<BlizzardRealm>> GetRealmsAsync(string region = "eu")
    {
        var token    = await TokenGrain.GetTokenAsync();
        var response = await _blizzardApi.GetRealmsAsync(region, token);

        if (!response.ResponseMessage.IsSuccessStatusCode)
        {
            var reason     = response.ResponseMessage.ReasonPhrase;
            var statusCode = response.ResponseMessage.StatusCode;
            var content    = await response.ResponseMessage.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get realms from Blizzard API. Reason: {Reason}, Status Code: {StatusCode}, Content: {Content}",
                             reason, statusCode, content);

            return ImmutableList<BlizzardRealm>.Empty;
        }

        var blizzardRealms = response.GetContent();

        return blizzardRealms.Realms.ToImmutableList();
    }

    public async Task<BlizzardRealm?> GetRealmAsync(string slug)
    {
        var token = await TokenGrain.GetTokenAsync();

        var realms = await Task.WhenAll(GetRealmAsync("eu", slug, token), GetRealmAsync("us", slug, token));

        return realms.FirstOrDefault(r => r != null);
    }

    private async Task<BlizzardRealm?> GetRealmAsync(string region, string slug, string token)
    {
        var response = await _blizzardApi.GetRealmAsync(slug, token, region);

        if (!response.ResponseMessage.IsSuccessStatusCode)
        {
            var reason     = response.ResponseMessage.ReasonPhrase;
            var statusCode = response.ResponseMessage.StatusCode;
            var content    = await response.ResponseMessage.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get realm from Blizzard API. Reason: {Reason}, Status Code: {StatusCode}, Content: {Content}",
                             reason, statusCode, content);

            return null;
        }

        var blizzardRealm = response.GetContent();

        return new BlizzardRealm(blizzardRealm.Id, blizzardRealm.Name, blizzardRealm.Slug, blizzardRealm.Region.Name);
    }
}