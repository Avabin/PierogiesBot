namespace GrainInterfaces.Discord.Guilds.ScheduledMessages;

[Immutable]
[GenerateSerializer]
public abstract record ScheduledMessage([property: Id(0)] DateTimeOffset At,   [property: Id(1)] string Content,
                                        [property: Id(2)] string         Name, [property: Id(3)] ulong  ChannelId);

[Immutable]
[GenerateSerializer]
public record ScheduledTextMessage
    (DateTimeOffset At, string Content, string Name, ulong ChannelId) : ScheduledMessage(At, Content, Name, ChannelId);

[Immutable]
[GenerateSerializer]
public record ScheduledEmojiMessage
    (DateTimeOffset At, string Content, string Name, ulong ChannelId) : ScheduledMessage(At, Content, Name, ChannelId);