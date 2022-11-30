using GrainInterfaces.Discord.Guilds.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable]
[GenerateSerializer]
public record ExecuteTriggers([property: Id(0)] IReadOnlyList<MessageTrigger> Triggers,
                              [property: Id(1)] ulong                         ChannelId,
                              [property: Id(2)] ulong                         MessageId) : TriggerEvent;