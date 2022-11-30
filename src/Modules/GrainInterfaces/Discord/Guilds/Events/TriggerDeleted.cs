namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable] [GenerateSerializer] public record TriggerDeleted([property: Id(0)] string Name) : TriggerEvent;