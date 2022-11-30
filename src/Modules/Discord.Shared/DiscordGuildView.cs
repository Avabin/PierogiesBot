namespace Discord.Shared;

[Immutable]
[GenerateSerializer]
public record DiscordGuildView([property: Id(0)] ulong  Id, [property: Id(1)] string Name,
                               [property: Id(2)] string Timezone)
{
}