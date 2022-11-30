namespace GrainInterfaces.Wow.Blizzard;

public interface IBlizzardTokenGrain : IGrainWithStringKey
{
    public const string Id = "BlizzardToken";
    Task<string>        GetTokenAsync();
}