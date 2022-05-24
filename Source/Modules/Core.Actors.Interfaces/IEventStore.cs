using Dapr.Actors;

namespace Core.Actors.Interfaces;

public interface IEventStore : IActor
{
    Task SaveAsync(Event delivery);
    Task<IEnumerable<Event>> GetAllAsync();
}