using Orleans;

namespace Discord.Shared;

[Immutable]
[GenerateSerializer]
public record DiscordGuildView([property: Id(0)] ulong Id, [property: Id(1)] string Name);