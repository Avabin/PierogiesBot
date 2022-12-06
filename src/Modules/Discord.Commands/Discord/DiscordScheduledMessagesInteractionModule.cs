using Discord.Interactions;
using Discord.WebSocket;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Shared.ScheduledMessages;
using TimeZoneConverter;

namespace Discord.Commands.Discord;

public class DiscordScheduledMessagesInteractionModule : GrainedInteractionModuleBase
{
    private readonly ILogger<DiscordScheduledMessagesInteractionModule> _logger;

    public DiscordScheduledMessagesInteractionModule(IClusterClient clusterClient,
        ILogger<DiscordScheduledMessagesInteractionModule> logger) :
        base(clusterClient)
    {
        _logger = logger;
    }

    [SlashCommand("schedule", "Schedules a message to be sent at a later time")]
    public async Task ScheduleMessageAsync(TimeOnly time, string message)
    {
        _logger.LogInformation("Scheduling message '{Message}' at {Time}", message, time);
        var user = (SocketGuildUser)Context.User;
        var guildId = Context.Guild.Id;
        var grain = Client.GetGuildScheduledMessagesWatcherGrain(guildId);
        var guildGrain = Client.GetDiscordGuildGrain(guildId);

        var timezoneId = await guildGrain.GetTimezoneAsync();

        if (!TZConvert.TryGetTimeZoneInfo(timezoneId, out var timezone))
        {
            _logger.LogWarning("Could not find timezone {TimezoneId} for guild {GuildId}", timezoneId, guildId);
            await RespondAsync("Could not find a valid timezone for this guild");
            return;
        }

        var now = DateTimeOffset.Now;
        var converted = TimeZoneInfo.ConvertTime(now, timezone!);

        _logger.LogDebug("Current time is {Now}, converted time is {Converted}", now, converted);

        if (time < TimeOnly.FromDateTime(converted.LocalDateTime))
        {
            converted = converted.AddDays(1);
        }

        var at = new DateTimeOffset(converted.Year, converted.Month, converted.Day, time.Hour, time.Minute, 0,
            converted.Offset);

        _logger.LogDebug("Scheduling message at {At}", at);
        var name = Context.User.Id.ToString("D");

        if (await grain.ContainsAsync(name))
        {
            if (!user.GuildPermissions.Administrator)
            {
                _logger.LogWarning("User {User} already has a scheduled message", name);
                await RespondAsync("You already have a scheduled message");
                return;
            }

            name = $"{name}_{Guid.NewGuid().ToString("N")[..8]}";
            _logger.LogTrace("User {User} is an admin, using name {Name}", user, name);
        }

        var scheduledMessage = new ScheduledTextMessage(at, message, name, Context.Channel.Id);

        _logger.LogDebug("Adding scheduled message {ScheduledMessage}", scheduledMessage);
        await grain.AddAsync(scheduledMessage);

        await RespondAsync($"Scheduled message for {at:HH:mm} ({timezoneId})");
    }

    [SlashCommand("schedule_emoji", "Schedules an emoji to be sent at a later time")]
    public async Task ScheduleEmojiAsync(TimeOnly time, string emoji)
    {
        _logger.LogInformation("Scheduling emoji '{Emoji}' at {Time}", emoji, time);
        var user = (SocketGuildUser)Context.User;
        var guildId = Context.Guild.Id;
        var grain = Client.GetGuildScheduledMessagesWatcherGrain(guildId);
        var guildGrain = Client.GetDiscordGuildGrain(guildId);

        var timezoneId = await guildGrain.GetTimezoneAsync();

        if (!TZConvert.TryGetTimeZoneInfo(timezoneId, out var timezone))
        {
            _logger.LogWarning("Could not find timezone {TimezoneId} for guild {GuildId}", timezoneId, guildId);
            await RespondAsync("Could not find a valid timezone for this guild");
            return;
        }

        var now = DateTimeOffset.Now;
        var converted = TimeZoneInfo.ConvertTime(now, timezone!);

        _logger.LogDebug("Current time is {Now}, converted time is {Converted}", now, converted);

        if (time < TimeOnly.FromDateTime(converted.LocalDateTime)) converted = converted.AddDays(1);

        var at = new DateTimeOffset(converted.Year, converted.Month, converted.Day, time.Hour, time.Minute, 0,
            converted.Offset);

        _logger.LogDebug("Scheduling emoji at {At}", at);
        var name = Context.User.Id.ToString("D");

        if (await grain.ContainsAsync(name))
        {
            if (!user.GuildPermissions.Administrator)
            {
                _logger.LogWarning("User {User} already has a scheduled message", name);
                await RespondAsync("You already have a scheduled message");
                return;
            }

            name = $"{name}_{Guid.NewGuid().ToString("N")[..8]}";
            _logger.LogTrace("User {User} is an admin, using name {Name}", user, name);
        }

        var scheduledMessage = new ScheduledEmojiMessage(at, emoji, name, Context.Channel.Id);

        _logger.LogDebug("Adding scheduled message {ScheduledMessage}", scheduledMessage);
        await grain.AddAsync(scheduledMessage);

        await RespondAsync($"Scheduled emoji for {at:HH:mm} ({timezoneId})");
    }

    [SlashCommand("delete_schedule", "Deletes a scheduled message")]
    public async Task DeleteScheduleAsync(string? name = null)
    {
        if (Context.User is not SocketGuildUser user)
        {
            _logger.LogWarning("User {User} is not a guild user", Context.User);
            await RespondAsync("You must be in a guild to use this command");
            return;
        }

        if (!user.GuildPermissions.Administrator) name = Context.User.Id.ToString("D");

        var grain = Client.GetGuildScheduledMessagesWatcherGrain(Context.Guild.Id);

        if (await grain.ContainsAsync(name))
        {
            await grain.RemoveAsync(name);
            await RespondAsync("Deleted scheduled message");
        }
        else
        {
            await RespondAsync($"Could not find a scheduled message with name {name}");
        }
    }
}