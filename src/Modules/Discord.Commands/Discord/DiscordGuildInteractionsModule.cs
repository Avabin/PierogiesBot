using Discord.Interactions;
using GrainInterfaces;
using GrainInterfaces.Discord;

namespace Discord.Commands.Discord;

[RequireUserPermission(GuildPermission.Administrator)]
public class DiscordGuildInteractionsModule : GrainedInteractionModuleBase
{
    public DiscordGuildInteractionsModule(IClusterClient clusterClient) : base(clusterClient)
    {
    }


    [SlashCommand("list", "Lists all guilds")]
    public async Task ListGuilds()
    {
        // return if not owner of the bot
        const ulong ownerId = 180791794638782465;
        if (Context.User.Id != ownerId)
        {
            await ReplyAsync("You are not my dad!");
            return;
        }

        var grain = Client.GetDiscordGuildsGrain();

        var guilds = await grain.GetGuildsAsync();

        var embed = new EmbedBuilder()
            .WithTitle("Guilds");

        foreach (var guild in guilds) embed.AddField(guild.Name, guild.Id.ToString(), true);

        await RespondAsync("Found guilds", embed: embed.Build());
    }

    [SlashCommand("set_timezone", "Sets the timezone for the guild")]
    public async Task SetTimezone(TimeZoneInfo timezone)
    {
        var stream = Client.GetDiscordGuildStream(Context.Guild.Id);

        var message = new SetGuildTimezone(timezone.Id);

        await stream.OnNextAsync(message);

        await RespondAsync("Timezone set");
    }


    [SlashCommand("get_timezone", "Gets the timezone for the guild")]
    public async Task GetTimezone()
    {
        var grain = Client.GetDiscordGuildGrain(Context.Guild.Id);

        var timezone = await grain.GetTimezoneAsync();

        await RespondAsync($"Timezone is {timezone}");
    }
}