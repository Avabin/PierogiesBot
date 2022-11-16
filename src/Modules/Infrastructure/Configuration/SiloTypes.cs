using System.Collections.Immutable;
using Grains.Discord;
using Grains.Discord.Guilds;
using Grains.Wow;
using Grains.Wow.Blizzard;

namespace Infrastructure.Configuration;

public static class SiloTypes
{
    public static readonly SiloType All = new(ImmutableList<Type>.Empty);

    public static readonly SiloType Discord =
        new(ImmutableList.Create(typeof(WowCharacterGrain), typeof(BlizzardRealmGrain), typeof(BlizzardTokenGrain)));

    public static readonly SiloType Wow = new(ImmutableList.Create(typeof(DiscordUserGrain), typeof(DiscordGuildGrain),
                                                                   typeof(GuildTriggersMessageWatcherGrain),
                                                                   typeof(GuildRuleExecutorGrain),
                                                                   typeof(DiscordMessagesWatcherGrain)));

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