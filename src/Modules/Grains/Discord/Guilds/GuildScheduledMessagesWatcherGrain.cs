using System.Collections.Immutable;
using Core;
using Discord;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds;
using GrainInterfaces.Discord.Guilds.Events;
using Grains.Core;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;
using Shared.ScheduledMessages;
using TimeZoneConverter;

namespace Grains.Discord.Guilds;

public class GuildScheduledMessagesWatcherGrain :
    EventEmitterGrain<GuildScheduledMessagesWatcherGrainState, ScheduledMessageEvent>,
    IGuildScheduledMessagesWatcherGrain, IRemindable
{
    private readonly IDiscordService _discordService;
    private readonly ILogger<GuildScheduledMessagesWatcherGrain> _logger;
    private ulong _guildId;

    public GuildScheduledMessagesWatcherGrain(IDiscordService discordService,
        ILogger<GuildScheduledMessagesWatcherGrain> logger)
    {
        _discordService = discordService;
        _logger = logger;
    }

    public async Task AddAsync(IScheduledMessage message)
    {
        _logger.LogInformation("Adding scheduled message '{Name}' to guild {GuildId}", message.Name, _guildId);
        State = State.Add(message);
        await WriteStateAsync();
        await ScheduleAsync(message);
        await NotifyAsync(StreamNamespaces.ForScheduledMessages(_guildId), this.GetPrimaryKeyString(),
            new MessageScheduled(message, _guildId));
    }

    public async Task RemoveAsync(string name)
    {
        _logger.LogInformation("Removing scheduled message '{Name}' from guild {GuildId}", name, _guildId);
        await UnregisterAsync(name);
    }

    public Task<bool> ContainsAsync(string name)
    {
        return Task.FromResult(State.Contains(name));
    }

    public Task<ImmutableList<IScheduledMessage>> GetAllAsync()
    {
        return Task.FromResult(State.ScheduledMessages.Value.Values.Cast<IScheduledMessage>().ToImmutableList());
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        _logger.LogDebug("Received reminder '{ReminderName}' for guild {GuildId}", reminderName, _guildId);
        var scheduledMessage = State.GetByName(reminderName);
        await (scheduledMessage switch
        {
            ScheduledEmojiMessage scheduledEmojiMessage => HandleEmojiAsync(scheduledEmojiMessage),
            ScheduledTextMessage scheduledTextMessage => HandleTextAsync(scheduledTextMessage),
            _ => Task.CompletedTask
        });

        if (scheduledMessage is null)
        {
            _logger.LogWarning(
                "Received reminder '{ReminderName}' for guild {GuildId} but no scheduled message was found",
                reminderName, _guildId);

            return;
        }

        if (scheduledMessage is not IRecurringScheduledMessage)
        {
            _logger.LogInformation("Message '{ReminderName}' is not recurring, removing", reminderName);
            await UnregisterAsync(reminderName);

            State = State.Remove(reminderName);
            await WriteStateAsync();
        }

        await NotifyAsync(StreamNamespaces.ForScheduledMessages(_guildId), this.GetPrimaryKeyString(),
            new ScheduledMessageExecuted(scheduledMessage.Name, _guildId, scheduledMessage.ChannelId));
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var key = this.GetPrimaryKeyString();
        _guildId = ulong.Parse(key);
        await base.OnActivateAsync(cancellationToken);
    }

    private async Task ScheduleAsync(IScheduledMessage message)
    {
        _logger.LogDebug("Scheduling message '{@Message}' for guild {GuildId}", message, _guildId);
        var guildGrain = GrainFactory.GetDiscordGuildGrain(_guildId);
        var timezoneId = await guildGrain.GetTimezoneAsync();

        var timezone = TZConvert.GetTimeZoneInfo(timezoneId);

        var now = DateTimeOffset.Now;
        var convertedNow = TimeZoneInfo.ConvertTime(now, timezone);

        var dueTime = message.At.Subtract(convertedNow);

        _logger.LogDebug("Scheduling message '{Name}' for guild {GuildId} in {DueTime}", message.Name, _guildId,
            dueTime);

        var period = TimeSpan.FromMinutes(1);

        if (message is IRecurringScheduledMessage recurring)
        {
            _logger.LogDebug("Message is recurring, scheduling for every {Period:g}", period);
            period = recurring.Interval;
        }

        _logger.LogDebug("Registering reminder for {DueTime:g}", dueTime);
        var reminder = await this.RegisterOrUpdateReminder(message.Name, dueTime, period);
        _logger.LogInformation("Registered reminder {@Reminder} for guild {GuildId}", reminder, _guildId);
    }

    private async Task UnregisterAsync(string name)
    {
        _logger.LogDebug("Unregistering reminder for message '{Name}' for guild {GuildId}", name, _guildId);
        var reminder = await this.GetReminder(name);
        await this.UnregisterReminder(reminder);
        State = State.Remove(name);
        await WriteStateAsync();

        await NotifyAsync(StreamNamespaces.ForScheduledMessages(_guildId), this.GetPrimaryKeyString(),
            new MessageUnregistered(name, _guildId));
    }

    private async Task HandleTextAsync(ScheduledTextMessage scheduledTextMessage)
    {
        _logger.LogDebug("Handling text message '{Name}' for guild {GuildId}", scheduledTextMessage.Name, _guildId);
        await _discordService.SendMessageAsync(scheduledTextMessage.ChannelId, scheduledTextMessage.Content);
    }

    private async Task HandleEmojiAsync(ScheduledEmojiMessage scheduledEmojiMessage)
    {
        _logger.LogDebug("Handling emoji message '{Name}' for guild {GuildId}", scheduledEmojiMessage.Name, _guildId);
        await _discordService.SendEmoteAsync(scheduledEmojiMessage.ChannelId, scheduledEmojiMessage.Content);
    }
}

[Immutable]
[GenerateSerializer]
public record GuildScheduledMessagesWatcherGrainState(
    [property: Id(0)] Immutable<Dictionary<string, IScheduledMessage>> ScheduledMessages
)
{
    public GuildScheduledMessagesWatcherGrainState() : this(new Dictionary<string, IScheduledMessage>().AsImmutable())
    {
    }

    public GuildScheduledMessagesWatcherGrainState Add(IScheduledMessage message)
    {
        var dict = new Dictionary<string, IScheduledMessage>(ScheduledMessages.Value);
        dict[message.Name] = message;
        return this with { ScheduledMessages = dict.AsImmutable() };
    }

    public GuildScheduledMessagesWatcherGrainState Remove(string name)
    {
        var dict = new Dictionary<string, IScheduledMessage>(ScheduledMessages.Value);
        dict.Remove(name);
        return this with { ScheduledMessages = dict.AsImmutable() };
    }

    public IScheduledMessage? GetByName(string reminderName)
    {
        return ScheduledMessages.Value.TryGetValue(reminderName, out var message) ? message : null;
    }

    public bool Contains(string name)
    {
        return ScheduledMessages.Value.ContainsKey(name);
    }
}