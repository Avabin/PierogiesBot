namespace Wow.Blizzard;

internal interface IRegionalWowDataSource : IWowDataSource
{
    Task<Character?> GetCharacterAsync(string realm, string name, string region);
}