using GrainInterfaces;
using GrainInterfaces.Discord;
using GrainInterfaces.Discord.Guilds;
using Microsoft.Extensions.Logging;
using Shared.MessageTriggers;

namespace ConsoleClient.Commands;

[Command("guild")]
public class GuildsCommands : ConsoleAppBase
{
    private readonly IClusterClient _clusterClient;

    public GuildsCommands(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [Command(new[] { "list", "ls" }, "List all guilds")]
    public async Task List()
    {
        var guilds = await _clusterClient.GetDiscordGuildsGrain().GetGuildsAsync();
        Console.WriteLine($"Found {guilds.Count} guilds");
        // covert guilds list to string table
        var table = new ConsoleTable("Id", "Name");

        foreach (var guild in guilds) table.AddRow(guild.Id.ToString(), guild.Name);

        Console.WriteLine(table.ToString());
    }

    [Command("get", "Get guild by id")]
    public async Task Get([Option(0, "Guild id")] ulong id)
    {
        var guild = await _clusterClient.GetDiscordGuildGrain(id).GetViewAsync();
        Console.WriteLine($"Found guild {guild.Name}");
        // covert guilds list to string table
        var table = new ConsoleTable("Id", "Name", "Timezone");
        table.AddRow(guild.Id.ToString(), guild.Name, guild.Timezone);
        Console.WriteLine(table.ToString());
    }


    [Command("trigger")]
    public class TriggerCommands : ConsoleAppBase
    {
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<TriggerCommands> _logger;

        public TriggerCommands(IClusterClient clusterClient, ILogger<TriggerCommands> logger)
        {
            _clusterClient = clusterClient;
            _logger = logger;
        }

        [Command("list", "Lists all triggers")]
        public async Task List([Option(0, "Guild snowflake ID")] ulong guildId)
        {
            var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(guildId);
            var triggers = await grain.GetAllAsync();

            foreach (var trigger in triggers) _logger.LogInformation("Trigger: {@Trigger}", trigger);
        }

        [Command("simple-response", "Adds a simple match response trigger")]
        public async Task SimpleResponse([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Trigger")] string trigger,
            [Option(2, "Response")] string response,
            [Option("-n", "Name")] string? name = null)
        {
            name ??= trigger;
            await Grain(guildId).AddAsync(name, new SimpleResponseMessageTrigger(trigger, name, response));

            _logger.LogInformation("Added simple trigger {Name} with trigger {Trigger} and response {Response}", name,
                trigger, response);
        }

        [Command("regex-response", "Adds a regex match response trigger")]
        public async Task RegexResponse([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Trigger")] string trigger,
            [Option(2, "Response")] string response,
            [Option("-n", "Name")] string? name = null)
        {
            name ??= trigger;
            await Grain(guildId).AddAsync(name, new RegexResponseMessageTrigger(trigger, name, response));

            _logger.LogInformation("Added regex trigger {Name} with trigger {Trigger} and response {Response}", name,
                trigger, response);
        }

        [Command("remove", "Removes a trigger")]
        public async Task Remove([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Trigger name")] string name)
        {
            await Grain(guildId).RemoveAsync(name);

            _logger.LogInformation("Removed trigger {Name}", name);
        }

        [Command("simple-reaction", "Adds a simple match reaction trigger")]
        public async Task SimpleReaction([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Trigger")] string trigger,
            [Option(2, "Reaction")] string reaction,
            [Option("-n", "Name")] string? name = null)
        {
            name ??= trigger;
            await Grain(guildId).AddAsync(name, new SimpleReactionMessageTrigger(trigger, name, reaction));

            _logger.LogInformation("Added simple trigger {Name} with trigger {Trigger} and reaction {Reaction}", name,
                trigger, reaction);
        }

        [Command("regex-reaction", "Adds a regex match reaction trigger")]
        public async Task RegexReaction([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Trigger")] string trigger,
            [Option(2, "Reaction")] string reaction,
            [Option("-n", "Name")] string? name = null)
        {
            name ??= trigger;
            await Grain(guildId).AddAsync(name, new RegexReactionMessageTrigger(trigger, name, reaction));

            _logger.LogInformation("Added regex trigger {Name} with trigger {Trigger} and reaction {Reaction}", name,
                trigger, reaction);
        }

        private IGuildTriggersMessageWatcherGrain Grain(ulong guildId)
        {
            var grain = _clusterClient.GetGuildTriggersMessageWatcherGrain(guildId);
            return grain;
        }
    }

    [Command("set")]
    public class Set : ConsoleAppBase
    {
        private readonly IClusterClient _clusterClient;

        public Set(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [Command("timezone", "Set guild timezone")]
        public async Task Timezone([Option(0, "Guild snowflake ID")] ulong guildId,
            [Option(1, "Timezone")] string timezone)
        {
            var stream = _clusterClient.GetDiscordGuildStream(guildId);

            await stream.OnNextAsync(new SetGuildTimezone(timezone));

            Console.WriteLine($"Set timezone to {timezone}");
        }
    }
}