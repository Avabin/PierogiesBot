using Shared.ScheduledMessages;

namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable]
[GenerateSerializer]
public record ScheduledMessageEvent;

[Immutable]
[GenerateSerializer]
public record MessageScheduled
    ([property: Id(0)] ScheduledMessage Message, [property: Id(1)] ulong GuildId) : ScheduledMessageEvent;

// MessageUnregistered
[Immutable]
[GenerateSerializer]
public record MessageUnregistered
    ([property: Id(0)] string ScheduledMessageName, [property: Id(1)] ulong GuildId) : ScheduledMessageEvent;

// ScheduledMessageExecuted
[Immutable]
[GenerateSerializer]
public record ScheduledMessageExecuted([property: Id(0)] string ScheduledMessageName, [property: Id(1)] ulong GuildId,
    [property: Id(2)] ulong ChannelId) : ScheduledMessageEvent;