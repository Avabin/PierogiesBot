using GrainInterfaces.Discord.Guilds.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable] [GenerateSerializer] public record TriggerCreated([property: Id(0)] MessageTrigger Trigger) : TriggerEvent;