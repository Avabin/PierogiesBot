using Core;
using GrainInterfaces.Discord;
using GrainInterfaces.Discord.Guilds;
using GrainInterfaces.Discord.Guilds.Events;
using GrainInterfaces.Discord.Users;
using GrainInterfaces.Wow;
using GrainInterfaces.Wow.Blizzard;
using Orleans.Runtime;
using Orleans.Streams;

namespace GrainInterfaces;

public static class GrainFactoryExtensions
{
    public static IWowCharacterGrain GetWowCharacterGrain(this IGrainFactory grainFactory, string server, string realm,
                                                          string             name)
    {
        var key = new WowCharacterKey(server, realm, name);
        return grainFactory.GetGrain<IWowCharacterGrain>(key);
    }

    public static IDiscordMessagesWatcherGrain GetDiscordMessagesWatcherGrain(this IGrainFactory grainFactory)
    {
        return grainFactory.GetGrain<IDiscordMessagesWatcherGrain>(IDiscordMessagesWatcherGrain.Id);
    }

    public static IDiscordUserGrain GetDiscordUserGrain(this IGrainFactory grainFactory, ulong id)
    {
        return grainFactory.GetGrain<IDiscordUserGrain>(id.ToString("D"));
    }

    public static IBlizzardRealmGrain GetBlizzardRealmGrain(this IGrainFactory grainFactory, string slug)
    {
        return grainFactory.GetGrain<IBlizzardRealmGrain>(slug);
    }

    public static IBlizzardTokenGrain GetBlizzardTokenGrain(this IGrainFactory grainFactory)
    {
        return grainFactory.GetGrain<IBlizzardTokenGrain>(IBlizzardTokenGrain.Id);
    }

    public static IGuildTriggersMessageWatcherGrain GetGuildTriggersMessageWatcherGrain(
        this IGrainFactory grainFactory, ulong guildId)
    {
        return grainFactory.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString("D"));
    }

    public static IDiscordGuildsGrain GetDiscordGuildsGrain(this IGrainFactory grainFactory)
    {
        return grainFactory.GetGrain<IDiscordGuildsGrain>(IDiscordGuildsGrain.Id);
    }

    public static IDiscordGuildGrain GetDiscordGuildGrain(this IGrainFactory grainFactory, ulong guildId)
    {
        return grainFactory.GetGrain<IDiscordGuildGrain>(guildId.ToString("D"));
    }

    public static IGuildScheduledMessagesWatcherGrain GetGuildScheduledMessagesWatcherGrain(
        this IGrainFactory grainFactory, ulong guildId)
    {
        return grainFactory.GetGrain<IGuildScheduledMessagesWatcherGrain>(guildId.ToString("D"));
    }

    public static IAsyncStream<DiscordUserEvent> GetDiscordUserStream(this IClusterClient client, ulong userId,
                                                                      string?             streamProvider = null)
    {
        return client.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                     .GetStream<DiscordUserEvent>(StreamId.Create(StreamNamespaces.ForDiscordUser(userId),
                                                                  userId.ToString("d")));
    }

    public static IAsyncStream<DiscordGuildEvent> GetDiscordGuildStream(this IClusterClient client, ulong guildId,
                                                                        string?             streamProvider = null)
    {
        return client.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                     .GetStream<DiscordGuildEvent>(StreamId.Create(StreamNamespaces.ForDiscordGuild(guildId),
                                                                   guildId.ToString("d")));
    }

    public static IAsyncStream<MessagesWatcherEvent> GetMessagesWatcherStream(
        this IClusterClient grain, ulong guildId, string? streamProvider = null)
    {
        return grain.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                    .GetStream<MessagesWatcherEvent>(StreamId.Create(StreamNamespaces.ForMessagesWatcher(guildId),
                                                                     guildId.ToString("D")));
    }

    public static IAsyncStream<TriggerEvent> GetTriggersStream(this IClusterClient client, ulong guildId,
                                                               string?             streamProvider = null)
    {
        return client.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                     .GetStream<TriggerEvent>(StreamId.Create(StreamNamespaces.ForTriggers(guildId),
                                                              guildId.ToString("d")));
    }


    public static IAsyncStream<DiscordUserEvent> GetDiscordUserStream(this Grain grain, ulong userId,
                                                                      string?    streamProvider = null)
    {
        return grain.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                    .GetStream<DiscordUserEvent>(StreamId.Create(StreamNamespaces.ForDiscordUser(userId),
                                                                 userId.ToString("d")));
    }

    public static IAsyncStream<DiscordGuildEvent> GetDiscordGuildStream(this Grain grain, ulong guildId,
                                                                        string?    streamProvider = null)
    {
        return grain.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                    .GetStream<DiscordGuildEvent>(StreamId.Create(StreamNamespaces.ForDiscordGuild(guildId),
                                                                  guildId.ToString("d")));
    }

    public static IAsyncStream<MessagesWatcherEvent> GetMessagesWatcherStream(
        this Grain grain, ulong guildId, string? streamProvider = null)
    {
        return grain.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                    .GetStream<MessagesWatcherEvent>(StreamId.Create(StreamNamespaces.ForMessagesWatcher(guildId),
                                                                     guildId.ToString("D")));
    }

    public static IAsyncStream<TriggerEvent> GetTriggersStream(this Grain grain, ulong guildId,
                                                               string?    streamProvider = null)
    {
        return grain.GetStreamProvider(streamProvider ?? StreamProviders.Default)
                    .GetStream<TriggerEvent>(StreamId.Create(StreamNamespaces.ForTriggers(guildId),
                                                             guildId.ToString("d")));
    }
}