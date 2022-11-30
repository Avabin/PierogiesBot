using System.Collections.Immutable;
using Shared.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds;

public interface IGuildTriggersMessageWatcherGrain : IGrainWithStringKey
{
    Task AddAsync(string name, MessageTrigger messageTrigger);
    Task RemoveAsync(string name);

    Task MuteChannelAsync(ulong channelId);
    Task UnmuteChannelAsync(ulong channelId);
    Task<ImmutableList<IMessageTrigger>> GetAllAsync(int limit = 0);
    Task<bool> ContainsTriggerAsync(string name);
}