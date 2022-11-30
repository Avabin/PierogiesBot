using Discord.Interactions;
using Discord.WebSocket;
using GrainInterfaces;
using Shared.MessageTriggers;

namespace Discord.Commands.Discord;

public class DiscordTriggersInteractionModule : GrainedInteractionModuleBase
{
    public DiscordTriggersInteractionModule(IClusterClient clusterClient) : base(clusterClient)
    {
    }

    [SlashCommand("add_response", "Add an automatic response to a message")]
    public async Task AddResponseTriggerAsync(string trigger, string response, bool regex = false)
    {
        var grain = Client.GetGuildTriggersMessageWatcherGrain(Context.Guild.Id);

        var name = Context.User.Id.ToString("D");

        if (await grain.ContainsTriggerAsync(name))
        {
            var user = (SocketGuildUser)Context.User;

            if (!user.Roles.Any(x => x.Permissions.Administrator))
                await
                    RespondAsync(
                        "You already have a trigger set up. Only administrators can have more than one trigger set up.",
                        ephemeral: true);

            name = $"{name}_{Guid.NewGuid().ToString("N")[..8]}";
        }

        MessageTrigger reaction = regex
            ? new RegexResponseMessageTrigger(trigger, name, response)
            : new SimpleResponseMessageTrigger(trigger, name, response);

        await grain.AddAsync(name, reaction);

        await RespondAsync($"Response {name} added", ephemeral: true);
    }

    [SlashCommand("add_reaction", "Add an automatic reaction")]
    public async Task AddSimpleReactionAsync(string trigger,
        [Autocomplete(typeof(EmojisAutocompleteHandler))]
        string emojiName,
        bool regex = false)
    {
        var grain = Client.GetGuildTriggersMessageWatcherGrain(Context.Guild.Id);

        var name = Context.User.Id.ToString("D");

        if (await grain.ContainsTriggerAsync(name))
        {
            var user = (SocketGuildUser)Context.User;

            if (!user.GuildPermissions.Administrator)
            {
                await
                    RespondAsync(
                        "You already have a trigger set up. Only administrators can have more than one trigger set up.",
                        ephemeral: true);
                return;
            }

            name = $"{name}_{Guid.NewGuid().ToString("N")[..8]}";
        }

        MessageTrigger reaction = regex
            ? new RegexReactionMessageTrigger(trigger, emojiName, name)
            : new SimpleReactionMessageTrigger(trigger, emojiName, name);

        await grain.AddAsync(name, reaction);

        await RespondAsync($"Trigger {name} added", ephemeral: true);
    }

    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("remove_trigger", "Remove an automatic trigger")]
    public async Task RemoveResponseTriggerAsync([Autocomplete(typeof(GuildTriggersAutocompleteHandler))] string name)
    {
        if (name is null or "")
        {
            await RespondAsync("You must specify a name", ephemeral: true);
            return;
        }

        var grain = Client.GetGuildTriggersMessageWatcherGrain(Context.Guild.Id);

        await grain.RemoveAsync(name);

        await RespondAsync($"Trigger {name} removed");
    }

    [SlashCommand("list_triggers", "List all automatic responses and reactions")]
    public async Task ListResponseTriggersAsync()
    {
        var grain = Client.GetGuildTriggersMessageWatcherGrain(Context.Guild.Id);

        var triggers = await grain.GetAllAsync();

        if (triggers.IsEmpty)
        {
            await RespondAsync("No triggers found");
            return;
        }

        var builder = new EmbedBuilder();

        foreach (var trigger in triggers) builder.AddField(trigger.Name, $"{trigger.Trigger} -> {trigger.Response}");

        await RespondAsync(embed: builder.Build());
    }
}