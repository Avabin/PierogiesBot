﻿using System.Collections.Immutable;
using Shared.ScheduledMessages;

namespace GrainInterfaces.Discord.Guilds;

public interface IGuildScheduledMessagesWatcherGrain : IGrainWithStringKey
{
    Task AddAsync(ScheduledMessage message);
    Task RemoveAsync(string name);
    Task<bool> ContainsAsync(string name);
    Task<ImmutableList<IScheduledMessage>> GetAllAsync();
}