namespace GrainInterfaces.Wow.Blizzard;

public interface IBlizzardTokenGrain : IGrainWithStringKey
{
    Task<string> GetTokenAsync();
}