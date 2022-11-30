using GrainInterfaces.Discord.Guilds.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable] [GenerateSerializer] public record TriggerUpdated([property: Id(0)] MessageTrigger Trigger) : TriggerEvent;