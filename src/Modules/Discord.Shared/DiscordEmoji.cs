namespace Discord.Shared;

[Immutable]
[GenerateSerializer]
public record DiscordEmoji([property: Id(0)] string Name, [property: Id(1)] string Representation,
                           [property: Id(2)] ulong  Id);