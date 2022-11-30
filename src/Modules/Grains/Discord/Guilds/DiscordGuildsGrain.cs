using System.Collections.Immutable;
using Discord;
using GrainInterfaces.Discord.Guilds;
using Shared;

namespace Grains.Discord.Guilds;

public class DiscordGuildsGrain : Grain, IDiscordGuildsGrain
{
    private readonly IDiscordService _service;

    public DiscordGuildsGrain(IDiscordService service)
    {
        _service = service;
    }

    public async Task<ImmutableList<BasicDiscordGuildInfo>> GetGuildsAsync()
    {
        var guilds = await _service.GetGuildsAsync();

        return guilds.Select(x => new BasicDiscordGuildInfo(x.id, x.name)).ToImmutableList();
    }
}