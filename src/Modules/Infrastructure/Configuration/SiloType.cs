using System.Collections.Immutable;
using Grains.Discord;
using Grains.Discord.Guilds;
using Grains.Discord.Users;
using Grains.Wow;
using Grains.Wow.Blizzard;

namespace Infrastructure.Configuration;

public record SiloType(ImmutableList<Type> ExcludedGrains)
{
    // All
    public static readonly SiloType All = new All();

    // Discord
    public static readonly SiloType Discord = new Discord();

    // Wow
    public static readonly SiloType Wow = new Wow();

    public static SiloType Parse(string value)
    {
        return value switch
        {
            "Discord" => Discord,
            "Wow"     => Wow,
            "All"     => All,
            _         => throw new ArgumentException($"Unknown silo type: {value}")
        };
    }
}

public record All() : SiloType(ImmutableList<Type>.Empty);

public record Discord() : SiloType(ImmutableList.Create(typeof(WowCharacterGrain), typeof(BlizzardRealmGrain),
                                                        typeof(BlizzardTokenGrain)));

public record Wow() : SiloType(ImmutableList.Create(typeof(DiscordUserGrain), typeof(DiscordGuildGrain),
                                                    typeof(GuildTriggersMessageWatcherGrain),
                                                    typeof(GuildMessageTriggersExecutorGrain),
                                                    typeof(DiscordMessagesWatcherGrain),
                                                    typeof(DiscordGuildsGrain)));