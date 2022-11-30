namespace Grains.Wow;

[GenerateSerializer]
public class WowCharacterState
{
    [Id(0)] public int Level { get; set; } = 0;

    [Id(1)] public DateTimeOffset LastRefresh { get; set; } = DateTimeOffset.MinValue;

    [Id(2)] public string Realm { get; set; } = string.Empty;

    [Id(3)] public string Name { get; set; } = string.Empty;
}