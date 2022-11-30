using System.Collections.Immutable;
using Discord.Interactions;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds.Events;
using GrainInterfaces.Discord.Guilds.MessageTriggers;
using Microsoft.Extensions.Caching.Memory;
using Orleans.Streams;

namespace Discord.Commands.Discord;

public class GuildTriggersAutocompleteHandler : AutocompleteHandler, IAsyncDisposable
{
    private readonly IClusterClient _clusterClient;
    private readonly List<ulong>    _guilds = new();
    private readonly IMemoryCache   _memoryCache;

    public GuildTriggersAutocompleteHandler(IClusterClient clusterClient, IMemoryCache memoryCache)
    {
        _clusterClient = clusterClient;
        _memoryCache   = memoryCache;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var guild in _guilds)
        {
            var streamKey = StreamCacheKey(guild);
            var handleKey = StreamHandleCacheKey(guild);
            var rulesKey  = RulesCacheKey(guild);

            var stream = _memoryCache.Get<IAsyncStream<TriggerEvent>>(streamKey);

            if (stream is not null)
            {
                var handles = await stream.GetAllSubscriptionHandles();

                foreach (var h in handles) await h.UnsubscribeAsync();
            }

            _memoryCache.Remove(streamKey);
            _memoryCache.Remove(rulesKey);
            _memoryCache.Remove(handleKey);
        }
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo      parameter, IServiceProvider         services)
    {
        var filter  = autocompleteInteraction.Data.Current.Value as string;
        var guildId = context.Guild.Id;
        var stream = Cached(StreamCacheKey(guildId), entry =>
        {
            _guilds.Add(guildId);
            return _clusterClient.GetTriggersStream(guildId);
        });

        var handles = await stream.GetAllSubscriptionHandles();

        if (!handles.Any()) await stream.SubscribeAsync(CreateOnNextAsync(guildId));

        IEnumerable<MessageTrigger> rules = await CachedRulesAsync(guildId);

        if (filter is not null or "")
            rules = rules.Where(r => r.Trigger.Contains(filter, StringComparison.OrdinalIgnoreCase)
                                  || r.Response.Contains(filter, StringComparison.OrdinalIgnoreCase)
                                  || r.Name.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase));

        var options = rules.Take(25).Select(x => new AutocompleteResult(x.ToString(), x.Name));

        return AutocompletionResult.FromSuccess(options);
    }

    private Func<TriggerEvent, StreamSequenceToken, Task> CreateOnNextAsync(ulong guildId)
    {
        return (e, token) => OnNextAsync(guildId, e, token);
    }

    private async Task OnNextAsync(ulong guildId, TriggerEvent te, StreamSequenceToken sst)
    {
        switch (te)
        {
            case TriggerUpdated update:
            {
                await HandleUpdateAsync(guildId, update);
                break;
            }
            case TriggerCreated created:
            {
                await HandleCreatedAsync(guildId, created);
                break;
            }
            case TriggerDeleted deleted:
            {
                await HandleDeletedAsync(guildId, deleted);
                break;
            }
        }
    }

    private async Task HandleDeletedAsync(ulong guildId, TriggerDeleted deleted)
    {
        var rules = await CachedRulesAsync(guildId);

        var existing = rules.FirstOrDefault(x => x.Name == deleted.Name);

        if (existing is not null)
            rules = rules.Remove(existing);
        else
            return;

        _memoryCache.Set(RulesCacheKey(guildId), rules);
    }

    private async Task HandleCreatedAsync(ulong guildId, TriggerCreated created)
    {
        var rules = await CachedRulesAsync(guildId);

        if (rules.Any(x => x.Name == created.Trigger.Name)) return;

        rules = rules.Add(created.Trigger);

        _memoryCache.Set(RulesCacheKey(guildId), rules);
    }

    private async Task HandleUpdateAsync(ulong guildId, TriggerUpdated update)
    {
        var rules = await CachedRulesAsync(guildId);

        var existing = rules.FirstOrDefault(x => x.Name == update.Trigger.Name);

        rules = existing is not null
                    ? rules.Replace(existing, update.Trigger)
                    : rules.Add(update.Trigger);

        _memoryCache.Set(RulesCacheKey(guildId), rules);
    }


    private Task<ImmutableList<MessageTrigger>> CachedRulesAsync(ulong guildId)
    {
        return CachedAsync(RulesCacheKey(guildId),
                           async entry =>
                               await _clusterClient.GetGuildTriggersMessageWatcherGrain(guildId).GetAllAsync());
    }

    private Task<T> CachedAsync<T>(object key, Func<ICacheEntry, Task<T>> func)
    {
        return _memoryCache.GetOrCreateAsync(key, func)!;
    }

    private T Cached<T>(object key, Func<ICacheEntry, T> func)
    {
        return _memoryCache.GetOrCreate(key, func)!;
    }

    private string StreamCacheKey(ulong guildId)
    {
        return $"guild:{guildId}:reactions";
    }

    private string RulesCacheKey(ulong guildId)
    {
        return $"guild:{guildId}:reactions:rules";
    }

    private string StreamHandleCacheKey(ulong guildId)
    {
        return $"guild:{guildId}:reactions:handle";
    }
}