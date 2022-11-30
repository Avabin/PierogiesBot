using System.Collections.Immutable;
using Shared;

namespace GrainInterfaces.Discord.Guilds;

public interface IDiscordGuildsGrain : IGrainWithStringKey
{
    public const string Id = "Guilds";
    Task<ImmutableList<BasicDiscordGuildInfo>> GetGuildsAsync();
}