using System.Collections.Immutable;

namespace GrainInterfaces.Discord.Guilds;

public interface IDiscordGuildsGrain : IGrainWithStringKey
{
    public const string                        Id = "Guilds";
    Task<ImmutableList<BasicDiscordGuildInfo>> GetGuildsAsync();
}

[Immutable]
[GenerateSerializer]
public record BasicDiscordGuildInfo([property: Id(0)] ulong Id, [property: Id(1)] string Name);