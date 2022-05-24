namespace Guilds.DataStore.Actors.Interfaces;

public interface IGuildStateReader
{
    Task<string> GetNameAsync();
}