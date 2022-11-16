namespace GrainInterfaces.Wow;

[Immutable]
[GenerateSerializer]
public record CharacterView([property: Id(0)] string Server, [property: Id(1)] string Realm,
                            [property: Id(2)] string Name,   [property: Id(3)] int    Level)
{
}