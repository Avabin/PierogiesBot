using Dapr.Actors;

namespace Guilds.DataStore.Actors.Interfaces;

public interface IGuild : IActor
{
    Task ChangeNameAsync(string name);
}