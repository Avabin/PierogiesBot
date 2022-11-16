using System.Collections.Immutable;
using GrainInterfaces.Discord.Guilds.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds.Grains;

public interface IGuildTriggersMessageWatcherGrain : IGrainWithStringKey
{
    Task AddAsync(string    name, MessageTrigger messageTrigger);
    Task RemoveAsync(string name);

    Task                                MuteChannelAsync(ulong   channelId);
    Task                                UnmuteChannelAsync(ulong channelId);
    Task<ImmutableList<MessageTrigger>> GetAllAsync();
}