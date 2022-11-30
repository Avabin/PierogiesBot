namespace Shared;

[Immutable]
[GenerateSerializer]
public record BasicDiscordGuildInfo([property: Id(0)] ulong Id, [property: Id(1)] string Name);