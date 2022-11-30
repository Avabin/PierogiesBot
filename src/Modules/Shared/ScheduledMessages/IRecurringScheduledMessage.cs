namespace Shared.ScheduledMessages;

public interface IRecurringScheduledMessage
{
    TimeSpan Interval { get; }
}