namespace GrainInterfaces.Wow.Blizzard;

public interface IBlizzardRealmGrain : IGrainWithStringKey
{
    Task<string> GetNameAsync();
    Task<string> GetRegionAsync();
}