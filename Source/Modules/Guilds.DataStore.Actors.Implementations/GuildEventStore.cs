using Core;
using Core.Actors.Implementations;
using Dapr.Actors.Runtime;
using Guilds.DataStore.Actors.Interfaces;
using Guilds.Shared;

namespace Guilds.DataStore.Actors.Implementations;

public class GuildEventStore : Actor, IGuildEventStore
{
    private ulong _guildId;
    public GuildEventStore(ActorHost host) : base(host)
    {
        
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        
        if (ulong.TryParse(Id.GetId(), out var guildId))
        {
            _guildId = guildId;
        }
    }

    public async Task SaveAsync(GuildCommand command)
    {
        var state     = await StateManager.GetOrAddStateAsync("guild", GuildEventStoreState.Empty);
        var newState  = state with { Events = state.Events.Add(command) };
        await StateManager.SetStateAsync("guild", newState);
    }

    public async Task<IEnumerable<GuildCommand>> GetAllAsync()
    {
        var state = await StateManager.GetOrAddStateAsync("guildState", GuildEventStoreState.Empty);
        return state.Events;
    }
}