namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable]
[GenerateSerializer]
public record MessageReceived([property: Id(0)] ulong  ChannelId, [property: Id(1)] ulong MessageId,
                              [property: Id(2)] string Content) : MessagesWatcherEvent;