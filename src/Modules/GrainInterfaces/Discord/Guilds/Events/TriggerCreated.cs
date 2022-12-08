using Shared.MessageTriggers;

namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable]
[GenerateSerializer]
public record TriggerCreated([property: Id(0)] IMessageTrigger Trigger) : TriggerEvent;