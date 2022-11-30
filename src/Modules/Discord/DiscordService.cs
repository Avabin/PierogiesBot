using System.Collections.Immutable;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Core;
using Discord.Interactions;
using Discord.Shared;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord;

internal class DiscordService : IDiscordService
{
    private const string ActivityName = "Discord.Commands";

    private readonly AsyncLazy<DiscordSocketClient> _client;
    private readonly AsyncLazy<InteractionService>  _interactionService;
    private readonly ILogger<DiscordService>        _logger;


    private readonly ISubject<SocketMessage>   _messageSubject = new Subject<SocketMessage>();
    private readonly IOptions<DiscordSettings> _options;
    private readonly IServiceProvider          _serviceProvider;
    private          IDisposable?              _subscription;

    public DiscordService(IOptions<DiscordSettings> options, ILogger<DiscordService> logger,
                          IServiceProvider          serviceProvider)
    {
        _options         = options;
        _logger          = logger;
        _serviceProvider = serviceProvider;

        _client = new AsyncLazy<DiscordSocketClient>(async () =>
        {
            var client = new DiscordSocketClient();

            client.Log += message => Task.Run(() =>
            {
                var logLevel = message.Severity switch
                {
                    LogSeverity.Critical => LogLevel.Critical,
                    LogSeverity.Error    => LogLevel.Error,
                    LogSeverity.Warning  => LogLevel.Warning,
                    LogSeverity.Info     => LogLevel.Information,
                    LogSeverity.Verbose  => LogLevel.Trace,
                    LogSeverity.Debug    => LogLevel.Debug,
                    _                    => LogLevel.None
                };

                if (logLevel is LogLevel.Error || message.Exception is not null)
                    logger.LogError(message.Exception, message.Message);
                else
                    logger.Log(logLevel, message.Message);
            });
            _logger.LogInformation("Starting Discord");
            var token = Settings.Token;

            var tcs = new TaskCompletionSource<bool>(false);

            Task OnReady()
            {
                return Task.Run(() => tcs.SetResult(true));
            }

            client.Ready += OnReady;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await tcs.Task;
            client.Ready -= OnReady;

            client.MessageReceived += OnMessageReceivedAsync;
            _logger.LogInformation("Discord started");
            return client;
        });

        _interactionService = new AsyncLazy<InteractionService>(async () =>
        {
            var service = new InteractionService(await _client!.Value);
            service.AddTypeConverter<TimeZoneInfo>(new TimeZoneInfoTypeReader());
            service.AddTypeConverter<TimeOnly>(new TimeOnlyTypeConverter());

            return service;
        });
    }

    protected DiscordSettings Settings => _options.Value;


    private static ActivitySource ActivitySource { get; } = new("Discord.Commands");

    public async Task InstallInteractionsAsync(Assembly assembly)
    {
        var client             = await _client.Value;
        var interactionService = await _interactionService.Value;
        _logger.LogInformation("Installing interactions...");
        interactionService.Log += message =>
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error    => LogLevel.Error,
                LogSeverity.Warning  => LogLevel.Warning,
                LogSeverity.Info     => LogLevel.Information,
                LogSeverity.Verbose  => LogLevel.Debug,
                LogSeverity.Debug    => LogLevel.Trace,
                _                    => throw new ArgumentOutOfRangeException(nameof(message), "has wrong LogSeverity!")
            };
            if (message.Exception is not null) _logger.LogError(message.Exception, message.Message);
            else _logger.Log(logLevel, message.Message);
            return Task.CompletedTask;
        };

        await interactionService.AddModulesAsync(assembly, _serviceProvider);

        await RegisterInteractionsForGuilds();

        client.InteractionCreated += async interaction =>
        {
            using var activity = ActivitySource.StartActivity(ActivityName, ActivityKind.Server);
            var       tags     = GetTags(interaction);
            foreach (var (key, value) in tags) activity?.SetTag(key, value);
            var guildId       = interaction.GuildId!.Value;
            var interactionId = interaction.Id;
            _logger.LogTrace("Interaction created: {InteractionId}", interactionId);

            try
            {
                activity?.Start();
                var scope  = _serviceProvider.CreateScope();
                var ctx    = new SocketInteractionContext(client, interaction);
                var result = await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
                if (result.IsSuccess) activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception e)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                activity?.SetTag("error",      e.Message);
                activity?.SetTag("stacktrace", e.StackTrace);

                _logger.LogError(e, "Error while executing interaction {InteractionId} in guild {GuildId}",
                                 interactionId, guildId);
                throw;
            }

            activity?.Stop();
        };
    }

    public async Task InstallInteractionsAsync()
    {
        var assemblies = Settings.CommandsAssemblies.Select(x =>
        {
            try
            {
                return Assembly.Load(x);
            }
            catch (TypeLoadException e)
            {
                _logger.LogWarning(e, "Failed to load assembly {Assembly}", x);
                return null;
            }
        }).Where(x => x is not null).ToList();
        _logger.LogInformation("Loading {AssemblyCount} assemblies", assemblies.Count);

        foreach (var assembly in assemblies)
        {
            _logger.LogDebug("Loading assembly {AssemblyName}", assembly!.FullName);
            await InstallInteractionsAsync(assembly);
        }
    }

    public async Task MuteUserAsync(ulong userId, ulong guildId, TimeSpan? duration = null)
    {
        duration ??= TimeSpan.FromDays(1);
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var user   = guild.GetUser(userId);
        _logger.LogInformation("Timimg out {User} in {Guild} for {Duration}", user, guild, duration);
        await user.SetTimeOutAsync(duration.Value);
    }

    public ImmutableList<ulong> GetUserRoles(ulong guildId, ulong userId)
    {
        var client = _client.Value.GetAwaiter().GetResult();
        var guild  = client.GetGuild(guildId);
        var user   = guild.GetUser(userId);
        var roles  = user.Roles.Select(x => x.Id).ToImmutableList();

        return roles;
    }

    public async Task UnmuteUserAsync(ulong userId, ulong guildId)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var user   = guild.GetUser(userId);
        _logger.LogInformation("Clearing timeout for {User} in {Guild}", user, guild);
        await user.RemoveTimeOutAsync();
    }

    public async Task BanUserAsync(ulong userId, ulong guildId)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var user   = guild.GetUser(userId);
        _logger.LogInformation("Banning {User} in {Guild}", user, guild);
        await user.BanAsync();
    }

    public async Task UnbanUserAsync(ulong userId, ulong guildId)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        _logger.LogInformation("Unbanning {User} in {Guild}", userId, guild);
        await guild.RemoveBanAsync(userId);
    }

    public async Task KickUserAsync(ulong userId, ulong guildId)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var user   = guild.GetUser(userId);
        _logger.LogInformation("Kicking {User} in {Guild}", user, guild);
        await user.KickAsync();
    }

    public async Task<ulong> CreateRoleAsync(ulong guildId, string name, byte r, byte g, byte b)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);

        _logger.LogInformation("Creating role {Name} in {Guild}", name, guild);
        var role = await guild.CreateRoleAsync(name, GuildPermissions.None, new Color(r, g, b), false, false);

        return role.Id;
    }

    public async Task DeleteRoleAsync(ulong guildId, ulong id)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var role   = guild.GetRole(id);
        _logger.LogInformation("Deleting {Role} in {Guild}", role, guild);
        await role.DeleteAsync();
    }

    public async Task ArchiveThreadAsync(ulong guildId, ulong threadId)
    {
        var client = await _client.Value;
        var guild  = client.GetGuild(guildId);
        var thread = guild.GetThreadChannel(threadId);

        _logger.LogInformation("Archiving {Thread} in {Guild}", thread, guild);
        await thread.DeleteAsync();
    }

    public async Task<List<DiscordEmoji>> GetEmojisAsync()
    {
        var client = await _client.Value;
        var emojis = client.Guilds.SelectMany(x => x.Emotes).Select(x => new DiscordEmoji(x.Name, x.ToString(), x.Id))
                           .ToList();

        return emojis;
    }

    public IObservable<IMessage> MessagesObservable => _messageSubject.AsObservable();

    public async Task StartAsync()
    {
        _ = await _client.Value;
    }

    public async Task StopAsync()
    {
        if (_client.IsValueCreated)
        {
            var client = await _client.Value;
            if (client.ConnectionState is ConnectionState.Disconnected or ConnectionState.Disconnecting) return;
            _logger.LogInformation("Stopping Discord");
            client.MessageReceived -= OnMessageReceivedAsync;
            _messageSubject.OnCompleted();
            _subscription?.Dispose();
            _subscription = null;
            await client.LogoutAsync();
            await client.StopAsync();

            _logger.LogInformation("Discord stopped");
        }
    }

    public async Task AddReactionAsync(ulong channelId, ulong messageId, string emoteName)
    {
        var client = await _client.Value;

        if (client.GetChannel(channelId) is not ISocketMessageChannel channel)
        {
            _logger.LogError("AddReactionAsync: Channel {ChannelId} not found", channelId);
            throw new ArgumentException($"Channel with id {channelId} not found");
        }

        var message = await channel.GetMessageAsync(messageId);

        if (message is null)
        {
            _logger.LogError("AddReactionAsync: Message {MessageId} not found", messageId);
            throw new ArgumentException($"Message with id {messageId} not found");
        }

        var emote = client.Guilds.SelectMany(x => x.Emotes.Where(y => y.Name.Equals(emoteName))).FirstOrDefault();

        if (emote is null) // emote not found, try to use emoji
        {
            if (!Emoji.TryParse($":{emoteName}:", out var emoji))
            {
                _logger.LogError("AddReactionAsync: Emote/emoji {EmoteName} not found", emoteName);
                throw new ArgumentException($"Emote with name {emoteName} not found");
            }

            await message.AddReactionAsync(emoji);
            return;
        }

        await message.AddReactionAsync(emote);
    }

    public async Task<ulong> GetChannelGuildIdAsync(ulong channelId)
    {
        var client  = await _client.Value;
        var channel = await client.GetChannelAsync(channelId);
        if (channel is SocketGuildChannel guildChannel) return guildChannel.Guild.Id;

        _logger.LogError("GetChannelGuildIdAsync: Channel {ChannelId} os not a guild channel or is null", channelId);
        throw new InvalidOperationException("Channel is not a guild channel");
    }

    public async Task<IReadOnlyCollection<(ulong id, string name)>> GetGuildsAsync()
    {
        var client = await _client.Value;
        var guilds = client.Guilds;

        if (guilds.Count is 0)
        {
            _logger.LogError("GetGuildsAsync: No guilds found");
            throw new InvalidOperationException("No guilds found");
        }

        return guilds.Select(x => (x.Id, x.Name)).ToImmutableList();
    }

    public async Task SendMessageAsync(ulong channelId, string message)
    {
        var client = await _client.Value;

        if (client.GetChannel(channelId) is not ISocketMessageChannel channel)
        {
            _logger.LogError("SendMessageAsync: Channel {ChannelId} not found", channelId);
            throw new ArgumentException($"Channel with id {channelId} not found");
        }

        await channel.SendMessageAsync(message);
    }

    public async Task SendEmoteAsync(ulong channelId, string emoteName)
    {
        var client = await _client.Value;

        if (client.GetChannel(channelId) is not SocketTextChannel channel)
        {
            _logger.LogError("SendEmoteAsync: Channel {ChannelId} not found", channelId);
            throw new ArgumentException($"Channel with id {channelId} not found");
        }

        var emote = channel.Guild.Emotes.FirstOrDefault(x => x.Name == emoteName);

        if (emote is null)
        {
            _logger.LogError("SendEmoteAsync: Emote {EmoteName} not found", emoteName);
            throw new ArgumentException($"Emote with name {emoteName} not found");
        }

        await channel.SendMessageAsync(emote.ToString());
    }

    private async Task RegisterInteractionsForGuilds()
    {
        var client             = await _client.Value;
        var interactionService = await _interactionService.Value;

        await Parallel.ForEachAsync(client.Guilds,
                                    async (guild, _) =>
                                        await interactionService.RegisterCommandsToGuildAsync(guild.Id));
    }

    private Task OnMessageReceivedAsync(SocketMessage message)
    {
        _messageSubject.OnNext(message);
        return Task.CompletedTask;
    }

    public async Task<SocketTextChannel> GetChannelAsync(ulong channelId)
    {
        var client  = await _client.Value;
        var channel = await client.GetChannelAsync(channelId);
        if (channel is SocketTextChannel textChannel) return textChannel;

        _logger.LogError("GetChannelAsync: Channel {ChannelId} os not a text channel or is null", channelId);
        throw new InvalidOperationException("Channel is not a text channel");
    }

    public async Task<IEmote?> GetEmote(string emoteName)
    {
        var client = await _client.Value;
        var emote  = client.Guilds.SelectMany(x => x.Emotes).FirstOrDefault(x => x.Name == emoteName);

        return emote;
    }


    private IDictionary<string, object> GetTags(SocketInteraction interaction)
    {
        return new Dictionary<string, object>()
        {
            { "discord.guild_id", interaction.GuildId     ?? 0 },
            { "discord.channel_id", interaction.ChannelId ?? 0 },
            { "discord.interaction_id", interaction.Id },
            { "discord.user", interaction.User }
        };
    }
}