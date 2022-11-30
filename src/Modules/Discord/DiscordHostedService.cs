using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Discord;

public class DiscordHostedService : IHostedService
{
    private readonly IClusterClient            _clusterClient;
    private readonly IOptions<DiscordSettings> _options;
    private readonly IDiscordService           _service;

    public DiscordHostedService(IDiscordService service, IOptions<DiscordSettings> options,
                                IClusterClient  clusterClient)
    {
        _service       = service;
        _options       = options;
        _clusterClient = clusterClient;
    }

    private DiscordSettings Options => _options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _service.StartAsync();

        if (Options.CommandsEnabled) await _service.InstallInteractionsAsync();

        await _clusterClient.GetDiscordMessagesWatcherGrain().StartWatchingAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _clusterClient.GetDiscordMessagesWatcherGrain().StopWatchingAsync();
        await _service.StopAsync();
    }
}