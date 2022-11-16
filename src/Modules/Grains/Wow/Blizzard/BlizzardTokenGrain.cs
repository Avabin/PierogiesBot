using GrainInterfaces.Wow.Blizzard;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wow.Blizzard;
using Wow.Blizzard.Client;

namespace Grains.Wow.Blizzard;

public class BlizzardTokenGrain : Grain<BlizzardTokenGrainState>, IBlizzardTokenGrain
{
    private readonly ILogger<BlizzardTokenGrain>  _logger;
    private readonly IOptions<WowBlizzardOptions> _options;
    private readonly IBlizzardTokenClient         _tokenClient;

    public BlizzardTokenGrain(IOptions<WowBlizzardOptions> options, IBlizzardTokenClient tokenClient,
                              ILogger<BlizzardTokenGrain>  logger)
    {
        _options     = options;
        _tokenClient = tokenClient;
        _logger      = logger;
    }

    private WowBlizzardOptions Options => _options.Value;

    public async Task<string> GetTokenAsync()
    {
        if (State.ExpiresAt > DateTimeOffset.Now.AddMinutes(-10))
            return State.Token; // Token is still valid for at least 10 minutes
        _logger.LogInformation("Refreshing token");

        var token = await _tokenClient.GetTokenAsync(Options.ClientId, Options.ClientSecret);
        State.Update(token);
        await WriteStateAsync();

        return State.Token;
    }
}