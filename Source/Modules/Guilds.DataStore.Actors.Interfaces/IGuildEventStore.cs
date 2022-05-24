using Core.Actors.Interfaces;
using Dapr.Actors;
using Guilds.Shared;

namespace Guilds.DataStore.Actors.Interfaces;

public interface IGuildEventStore : IActor
{
    Task SaveAsync(GuildCommand command);
    Task<IEnumerable<GuildCommand>> GetAllAsync();
}