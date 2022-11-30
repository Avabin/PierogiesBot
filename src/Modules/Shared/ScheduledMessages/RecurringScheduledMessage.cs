namespace Shared.ScheduledMessages;

[Immutable]
[GenerateSerializer]
public record RecurringScheduledTextMessage(DateTimeOffset At, string Content, string Name, ulong ChannelId,
    [property: Id(4)] TimeSpan Interval) :
    ScheduledTextMessage(At, Content, Name, ChannelId), IRecurringScheduledMessage;

[Immutable]
[GenerateSerializer]
public record RecurringScheduledEmojiMessage(DateTimeOffset At, string Content, string Name, ulong ChannelId,
    [property: Id(4)] TimeSpan Interval) :
    ScheduledEmojiMessage(At, Content, Name, ChannelId), IRecurringScheduledMessage;