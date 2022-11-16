using GrainInterfaces.Discord.Guilds.Grains;
using GrainInterfaces.Discord.Guilds.MessageTriggers;
using Microsoft.Extensions.Logging;

namespace ConsoleClient.Commands;

[Command("trigger")]
public class TriggerCommands : ConsoleAppBase
{
    private readonly IClusterClient           _clusterClient;
    private readonly ILogger<TriggerCommands> _logger;

    public TriggerCommands(IClusterClient clusterClient, ILogger<TriggerCommands> logger)
    {
        _clusterClient = clusterClient;
        _logger        = logger;
    }

    [Command("list", "Lists all triggers")]
    public async Task List([Option(0, "Guild snowflake ID")] ulong guildId)
    {
        var grain    = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        var triggers = await grain.GetAllAsync();

        foreach (var trigger in triggers) _logger.LogInformation("Trigger: {@Trigger}", trigger);
    }

    [Command("simple-response", "Adds a simple match response trigger")]
    public async Task SimpleResponse([Option(0,    "Guild snowflake ID")] ulong   guildId,
                                     [Option(1,    "Trigger")]            string  trigger,
                                     [Option(2,    "Response")]           string  response,
                                     [Option("-n", "Name")]               string? name = null)
    {
        name ??= trigger;
        var grain = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        await grain.AddAsync(name, new SimpleResponseMessageTrigger(trigger, name, response));

        _logger.LogInformation("Added simple trigger {Name} with trigger {Trigger} and response {Response}", name,
                               trigger, response);
    }

    [Command("regex-response", "Adds a regex match response trigger")]
    public async Task RegexResponse([Option(0, "Guild snowflake ID")] ulong guildId,
                                    [Option(1, "Trigger")] string trigger,
                                    [Option(2, "Response")] string response, [Option("-n", "Name")] string? name = null)
    {
        name ??= trigger;
        var grain = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        await grain.AddAsync(name, new RegexResponseMessageTrigger(trigger, name, response));

        _logger.LogInformation("Added regex trigger {Name} with trigger {Trigger} and response {Response}", name,
                               trigger, response);
    }

    [Command("remove", "Removes a trigger")]
    public async Task Remove([Option(0, "Guild snowflake ID")] ulong guildId, [Option(1, "Trigger name")] string name)
    {
        var grain = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        await grain.RemoveAsync(name);

        _logger.LogInformation("Removed trigger {Name}", name);
    }

    [Command("simple-reaction", "Adds a simple match reaction trigger")]
    public async Task SimpleReaction([Option(0,    "Guild snowflake ID")] ulong   guildId,
                                     [Option(1,    "Trigger")]            string  trigger,
                                     [Option(2,    "Reaction")]           string  reaction,
                                     [Option("-n", "Name")]               string? name = null)
    {
        name ??= trigger;
        var grain = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        await grain.AddAsync(name, new SimpleReactionMessageTrigger(trigger, name, reaction));

        _logger.LogInformation("Added simple trigger {Name} with trigger {Trigger} and reaction {Reaction}", name,
                               trigger, reaction);
    }

    [Command("regex-reaction", "Adds a regex match reaction trigger")]
    public async Task RegexReaction([Option(0, "Guild snowflake ID")] ulong guildId,
                                    [Option(1, "Trigger")] string trigger,
                                    [Option(2, "Reaction")] string reaction, [Option("-n", "Name")] string? name = null)
    {
        name ??= trigger;
        var grain = _clusterClient.GetGrain<IGuildTriggersMessageWatcherGrain>(guildId.ToString());
        await grain.AddAsync(name, new RegexReactionMessageTrigger(trigger, name, reaction));

        _logger.LogInformation("Added regex trigger {Name} with trigger {Trigger} and reaction {Reaction}", name,
                               trigger, reaction);
    }
}