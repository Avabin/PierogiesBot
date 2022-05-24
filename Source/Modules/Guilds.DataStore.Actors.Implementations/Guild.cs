using Core;
using Core.Actors.Interfaces;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Guilds.DataStore.Actors.Interfaces;
using Guilds.Shared;
using Infrastructure;

namespace Guilds.DataStore.Actors.Implementations;

public class Guild : Actor, IGuild
{
    private readonly IClusterClient _clusterClient;
    private ulong _guildId;

    public Guild(ActorHost host, IClusterClient clusterClient) : base(host)
    {
        _clusterClient = clusterClient;
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        var id = Id.GetId();

        if (ulong.TryParse(id, out var guildId))
        {
            _guildId = guildId;
        }
        else
        {
            throw new InvalidOperationException($"Invalid guild id: {id}");
        }
    }

    public async Task ChangeNameAsync(string name)
    {
        var command         = new ChangeGuildNameCommand(name, _guildId);
        var guildEventStore = ProxyFactory.CreateActorProxy<IGuildEventStore>(new ActorId(Id.GetId()), "GuildEventStore");
        await guildEventStore.SaveAsync(command);
        await Apply(x => x with { Name = name });

        var notification = new GuildNameChanged(_guildId, name);
        await _clusterClient.PublishAsync(notification, Namespaces.Notifications);
    }

    private async Task Apply(Func<GuildState, GuildState> transform)
    {
        var state = await StateManager.GetOrAddStateAsync(Id.GetId(), GuildState.Empty);
        var newState = transform(state);
        await StateManager.SetStateAsync(Id.GetId(), newState);
    }
}