namespace GrainInterfaces.Discord.Guilds.Events;

[Immutable] [GenerateSerializer] public abstract record TriggerEvent;

[Immutable]
[GenerateSerializer]
public record TriggersExecuted([property: Id(0)] IReadOnlyList<string> Triggers, [property: Id(1)] ulong GuildId,
                               [property: Id(2)] ulong ChannelId, [property: Id(3)] ulong MessageId) : TriggerEvent;