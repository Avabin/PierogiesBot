using GrainInterfaces.Wow;

namespace Wow;

internal class WowService : IWowService
{
    private readonly Dictionary<string, IWowDataSource> _dataSources;

    public WowService(IEnumerable<IWowDataSource> dataSources)
    {
        _dataSources = dataSources.ToDictionary(x => x.Server, x => x);
    }

    public async Task<WowCharacterView?> FetchCharacterAsync(string server, string realm, string name)
    {
        if (!_dataSources.TryGetValue(server, out var dataSource)) return null;

        var character = await dataSource.GetCharacterAsync(realm, name);

        return character is null
                   ? null
                   : new WowCharacterView(server, character.Realm, character.Name, character.Level);
    }
}