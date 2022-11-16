using GrainInterfaces.Wow.Blizzard;
using Wow.Blizzard.Client;

namespace Grains.Wow.Blizzard;

public class BlizzardRealmGrain : Grain<BlizzardRealmState>, IBlizzardRealmGrain
{
    private readonly IBlizzardRealmsClient _realmsClient;

    public BlizzardRealmGrain(IBlizzardRealmsClient realmsClient)
    {
        _realmsClient = realmsClient;
    }

    public Task<string> GetNameAsync()
    {
        return Task.FromResult(State.Name);
    }

    public Task<string> GetRegionAsync()
    {
        return Task.FromResult(State.Region switch
        {
            "Europe"        => "eu",
            "North America" => "us",
            "Taiwan"        => "tw",
            "Korea"         => "kr",
            _               => throw new InvalidOperationException($"Unknown region {State.Region}")
        });
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        var key = this.GetPrimaryKeyString();

        State = State with { Slug = key };

        await RefreshStateAsync();
    }

    private async Task RefreshStateAsync()
    {
        if (State.UpdatedAt < DateTimeOffset.Now.AddDays(-7))
        {
            var realm = await _realmsClient.GetRealmAsync(State.Slug);
            if (realm is null) return;
            State = new BlizzardRealmState(realm.Id, realm.Slug, realm.Name, realm.Region, DateTimeOffset.Now);
            await WriteStateAsync();
        }
    }
}

[Immutable]
[GenerateSerializer]
public record BlizzardRealmState
(int                              Id, string Name, string Slug, string Region,
 [property: Id(3)] DateTimeOffset UpdatedAt) : BlizzardRealm(Id, Name, Slug, Region)
{
    public BlizzardRealmState() : this(0, string.Empty, string.Empty, "", DateTimeOffset.MinValue)
    {
    }
}