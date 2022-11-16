using GrainInterfaces.Wow;
using Microsoft.Extensions.Logging;
using Wow;

namespace Grains.Wow;

public class WowCharacterGrain : Grain<WowCharacterState>, IWowCharacterGrain
{
    private static   TimeSpan                   _updateInterval = TimeSpan.FromMinutes(5);
    private readonly ILogger<WowCharacterGrain> _logger;
    private readonly IWowService                _service;

    public WowCharacterGrain(IWowService service, ILogger<WowCharacterGrain> logger)
    {
        _service = service;
        _logger  = logger;
    }

    public async Task RefreshAsync()
    {
        _logger.LogInformation("Refreshing character");
        var (server, realm, name) = WowCharacterKey.Parse(this.GetPrimaryKeyString());
        var characterView = await _service.FetchCharacterAsync(server, realm, name);

        if (characterView is not null)
        {
            State.Name  = characterView.Name;
            State.Realm = characterView.Realm;
            State.Level = characterView.Level;
            await WriteStateAsync();
            _logger.LogInformation("Character refreshed");
        }
        else
        {
            _logger.LogWarning("Character {Name} not found on {Server}/{Realm}", name, server, realm);
        }
    }

    public Task<string> GetServerAsync()
    {
        return Task.FromResult(Server());
    }

    public Task<string> GetRealmAsync()
    {
        return Task.FromResult(Realm());
    }

    public Task<string> GetNameAsync()
    {
        return Task.FromResult(Name());
    }

    public Task<CharacterView> GetViewAsync()
    {
        var (server, realm, name) = WowCharacterKey.Parse(this.GetPrimaryKeyString());

        return Task.FromResult(new CharacterView(server, State.Realm, State.Name, State.Level));
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        _logger.LogInformation("OnActivateAsync");

        if (State.LastRefresh < DateTimeOffset.UtcNow - _updateInterval)
            await RefreshAsync();

        RegisterTimer(OnTimerAsync, null, _updateInterval, _updateInterval);
    }

    private Task OnTimerAsync(object arg)
    {
        return RefreshAsync();
    }

    private string Server()
    {
        return WowCharacterKey.Parse(this.GetPrimaryKeyString()).Server;
    }

    private string Realm()
    {
        return State.Realm;
    }

    private string Name()
    {
        return State.Name;
    }
}