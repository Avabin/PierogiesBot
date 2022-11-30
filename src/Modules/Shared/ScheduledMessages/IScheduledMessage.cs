namespace Shared.ScheduledMessages;

public interface IScheduledMessage
{
    DateTimeOffset At { get; init; }
    string Content { get; init; }
    string Name { get; init; }
    ulong ChannelId { get; init; }
}