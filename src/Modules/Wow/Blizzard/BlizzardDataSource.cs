using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Wow.Blizzard.Client;
using Wow.Shared;

namespace Wow.Blizzard;

internal class BlizzardDataSource : IWowDataSource
{
    private readonly IBlizzardApi                _blizzardApi;
    private readonly IClusterClient              _clusterClient;
    private readonly ILogger<BlizzardDataSource> _logger;

    public BlizzardDataSource(IBlizzardApi                blizzardApi, IClusterClient clusterClient,
                              ILogger<BlizzardDataSource> logger)
    {
        _blizzardApi   = blizzardApi;
        _clusterClient = clusterClient;
        _logger        = logger;
    }

    public string Server { get; } = Servers.Blizzard;

    public async Task<Character?> GetCharacterAsync(string realm, string name)
    {
        var tokenGrain = _clusterClient.GetBlizzardTokenGrain();
        var token      = await tokenGrain.GetTokenAsync();

        var realmGrain = _clusterClient.GetBlizzardRealmGrain(realm);

        var region = await realmGrain.GetRegionAsync();

        var character = await _blizzardApi.GetCharacterAsync(realm, name, token, region);

        if (!character.ResponseMessage.IsSuccessStatusCode)
        {
            var statusCode = character.ResponseMessage.StatusCode;
            var content    = character.StringContent;
            var reason     = character.ResponseMessage.ReasonPhrase;
            _logger.LogError("Error getting character from Blizzard API. Status code: {StatusCode}, reason: {Reason}, content: {Content}",
                             statusCode, reason, content);
            return null;
        }

        var characterData = character.GetContent();

        return new Character(characterData.Name, characterData.Realm.Name, characterData.Level);
    }
}