using GrainInterfaces.Discord.Guilds.ScheduledMessages;

namespace GrainInterfaces.Discord.Guilds;

public interface IGuildScheduledMessagesWatcherGrain : IGrainWithStringKey
{
    Task       AddAsync(ScheduledMessage message);
    Task       RemoveAsync(string        name);
    Task<bool> ContainsAsync(string      name);
}