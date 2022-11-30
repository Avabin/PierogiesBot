using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds.Events;
using Microsoft.Extensions.Caching.Memory;
using Orleans.Streams;

namespace Discord;

public class DiscordMessagesWatcherService : IDiscordMessagesWatcherService
{
    private readonly IClusterClient  _client;
    private readonly IDiscordService _discordService;
    private readonly IMemoryCache    _memoryCache;
    private          IDisposable?    _subscription;

    public DiscordMessagesWatcherService(IClusterClient client, IDiscordService discordService,
                                         IMemoryCache   memoryCache)
    {
        _client         = client;
        _discordService = discordService;
        _memoryCache    = memoryCache;
    }

    public async Task StartAsync()
    {
        if (_subscription is not null)
            return;
        await _discordService.StartAsync();
        _subscription = _discordService.MessagesObservable
                                       .Where(x => x.Channel is IGuildChannel && x is { Author.IsBot: false })
                                       .Do(x =>
                                        {
                                            var channel = (IGuildChannel)x.Channel;
                                            var message = new MessageReceived(x.Channel.Id, x.Id, x.Content);
                                            var guildId = channel.Guild.Id;
                                            var observer =
                                                _memoryCache.GetOrCreate(guildId, _ => Stream(guildId).ToObserver());
                                            observer!.OnNext(message);
                                        }).Subscribe();
    }

    public async Task StopAsync()
    {
        if (_subscription is null)
            return;
        _subscription?.Dispose();

        await _discordService.StopAsync();
    }

    private IAsyncStream<MessagesWatcherEvent> Stream(ulong id)
    {
        return _client.GetMessagesWatcherStream(id);
    }
}

public static class StreamExtensions
{
    public static IObservable<T> ToObservable<T>(this IAsyncStream<T> stream)
    {
        return Observable.Create<T>(async observer =>
        {
            var handle =
                await stream.SubscribeAsync(async (message, _) => await Task.Run(() => observer.OnNext(message)));
            var disposable = Disposable.Create(stream, s => handle.UnsubscribeAsync().Wait());

            return disposable;
        });
    }

    public static IObserver<T> ToObserver<T>(this IAsyncStream<T> stream)
    {
        return Observer.Create<T>(message => stream.OnNextAsync(message));
    }
}