namespace GrainInterfaces.Wow.Blizzard;

[Immutable]
[GenerateSerializer]
public record BlizzardRealm(
    [property: Id(0)] int    Id, [property: Id(1)] string Name, [property: Id(2)] string Slug,
    [property: Id(3)] string Region)
{
    public BlizzardRealm() : this(0, string.Empty, string.Empty, string.Empty)
    {
    }
}