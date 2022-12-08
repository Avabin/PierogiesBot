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

    [HttpGet("{id:long}/messageTriggers/{triggerName}")]
    public async Task<IMessageTrigger> GetTriggerAsync(ulong id, string triggerName)
    {
        _logger.LogInformation("Trigger");
        var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(id);

        var trigger = await grain.GetMessageTriggerAsync(triggerName);

        return trigger;
    }

    [HttpPost("{id:long}/messageTriggers")]
    public async Task AddTriggerAsync(ulong id, [FromBody] IMessageTrigger trigger)
    {
        _logger.LogInformation("Add trigger");
        var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(id);

        await grain.AddAsync(trigger.Name, trigger);
    }

    [HttpDelete("{id:long}/messageTriggers/{triggerName}")]
    public async Task RemoveTriggerAsync(ulong id, string triggerName)
    {
        _logger.LogInformation("Remove trigger");
        var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(id);

        await grain.RemoveAsync(triggerName);
    }

    [HttpGet("{id:long}/scheduledMessages")]
    public async Task<IEnumerable<IScheduledMessage>> GetScheduledMessagesAsync(ulong id)
    {
        _logger.LogInformation("Scheduled Messages");
        var grain = _clusterClient.GetGuildScheduledMessagesWatcherGrain(id);

        var messages = await grain.GetAllAsync();

        return messages;
    }

    [HttpPost("{id:long}/scheduledMessages")]
    public async Task AddScheduledMessageAsync(ulong id, [FromBody] IScheduledMessage message)
    {
        _logger.LogInformation("Add scheduled message");
        var grain = _clusterClient.GetGuildScheduledMessagesWatcherGrain(id);

        await grain.AddAsync(message);
    }

    [HttpDelete("{id:long}/scheduledMessages/{messageName}")]
    public async Task RemoveScheduledMessageAsync(ulong id, string messageName)
    {
        _logger.LogInformation("Remove scheduled message");
        var grain = _clusterClient.GetGuildScheduledMessagesWatcherGrain(id);

        await grain.RemoveAsync(messageName);
    }
}