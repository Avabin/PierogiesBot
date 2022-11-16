namespace Grains.Wow;

public class WowCharacterState
{
    public int            Level       { get; set; } = 0;
    public DateTimeOffset LastRefresh { get; set; } = DateTimeOffset.MinValue;
    public string         Realm       { get; set; } = string.Empty;
    public string         Name        { get; set; } = string.Empty;
}