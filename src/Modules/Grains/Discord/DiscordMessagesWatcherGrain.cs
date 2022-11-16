using Discord;
using GrainInterfaces.Discord;
using Microsoft.Extensions.Logging;

namespace Grains.Discord;

public class DiscordMessagesWatcherGrain : Grain, IDiscordMessagesWatcherGrain
{
    private readonly ILogger<DiscordMessagesWatcherGrain> _logger;
    private readonly IDiscordMessagesWatcherService       _service;

    public DiscordMessagesWatcherGrain(ILogger<DiscordMessagesWatcherGrain> logger,
                                       IDiscordMessagesWatcherService       service)
    {
        _logger  = logger;
        _service = service;
    }

    public async Task StartWatchingAsync()
    {
        _logger.LogInformation("Starting watching for messages");
        await _service.StartAsync();
    }

    public async Task StopWatchingAsync()
    {
        _logger.LogInformation("Stopping watching for messages");
        await _service.StopAsync();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!this.GetPrimaryKeyString().Equals(IDiscordMessagesWatcherGrain.Id))
            throw new
                InvalidOperationException("Invalid primary key, this grain is a singleton. Use IDiscordMessagesWatcherGrain.Id as primary key.");
        await base.OnActivateAsync(cancellationToken);
    }
}