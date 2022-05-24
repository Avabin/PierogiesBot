using System.Collections.Immutable;
using Core.Actors.Interfaces;
using Dapr.Actors.Runtime;

namespace Core.Actors.Implementations;

public class EventStore : Actor, IEventStore
{
    protected EventStore(ActorHost host) : base(host)
    {
    }

    public async Task SaveAsync(Event delivery)
    {
        var state = await StateManager.GetOrAddStateAsync(Id.GetId(), EventStoreState.Empty);

        var newState = new EventStoreState(Version: state.Version + 1, Events: state.Events.Add(delivery));

        await StateManager.SetStateAsync(Id.GetId(), newState);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        var state = await StateManager.TryGetStateAsync<EventStoreState>(Id.GetId());

        return state.HasValue ? state.Value.Events : ImmutableList<Event>.Empty;
    }
    
    public async Task<ulong> GetVersion() => (await StateManager.GetOrAddStateAsync(Id.GetId(), EventStoreState.Empty)).Version;
}