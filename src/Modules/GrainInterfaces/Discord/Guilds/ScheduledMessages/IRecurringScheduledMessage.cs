namespace GrainInterfaces.Discord.Guilds.ScheduledMessages;

public interface IRecurringScheduledMessage
{
    TimeSpan Interval { get; }
}