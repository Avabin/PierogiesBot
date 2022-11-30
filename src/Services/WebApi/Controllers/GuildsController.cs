using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.MessageTriggers;
using Shared.ScheduledMessages;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GuildsController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IClusterClient _clusterClient;

    private readonly ILogger<GuildsController> _logger;

    public GuildsController(ILogger<GuildsController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    [HttpGet]
    public async Task<IEnumerable<BasicDiscordGuildInfo>> Get()
    {
        var grain = _clusterClient.GetDiscordGuildsGrain();

        return await grain.GetGuildsAsync();
    }

    [HttpGet("{id:long}/messageTriggers")]
    public async Task<IEnumerable<IMessageTrigger>> GetTriggersAsync(ulong id)
    {
        _logger.LogInformation("Triggers");
        var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(id);

        var triggers = await grain.GetAllAsync();

        return triggers;
    }

    [HttpGet("{id:long}/scheduledMessages")]
    public async Task<IEnumerable<IScheduledMessage>> GetScheduledMessagesAsync(ulong id)
    {
        _logger.LogInformation("Scheduled Messages");
        var grain = _clusterClient.GetGuildScheduledMessagesWatcherGrain(id);

        var messages = await grain.GetAllAsync();

        return messages;
    }
}