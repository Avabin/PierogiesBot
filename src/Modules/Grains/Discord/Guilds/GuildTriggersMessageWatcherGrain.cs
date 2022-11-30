using System.Collections.Immutable;
using Core;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds;
using GrainInterfaces.Discord.Guilds.Events;
using Grains.Core;
using Orleans.Concurrency;
using Shared.MessageTriggers;

namespace Grains.Discord.Guilds;

[RegexImplicitStreamSubscription(StreamNamespaces.MessagesWatcher)]
public class GuildTriggersMessageWatcherGrain : EventSubscriberGrain<GuildReactionsState, MessageReceived>,
    IGuildTriggersMessageWatcherGrain
{
    private ulong _guildId;

    public async Task AddAsync(string name, MessageTrigger messageTrigger)
    {
        State = State.Add(name, messageTrigger);
        await WriteStateAsync();
        await this.GetTriggersStream(_guildId).OnNextAsync(new TriggerCreated(messageTrigger));
    }

    public async Task RemoveAsync(string name)
    {
        State = State.Remove(name);
        await WriteStateAsync();
        await this.GetTriggersStream(_guildId).OnNextAsync(new TriggerDeleted(name));
    }

    public async Task MuteChannelAsync(ulong channelId)
    {
        State = State.MuteChannel(channelId);
        await WriteStateAsync();
    }

    public async Task UnmuteChannelAsync(ulong channelId)
    {
        State = State.UnmuteChannel(channelId);
        await WriteStateAsync();
    }

    public Task<ImmutableList<IMessageTrigger>> GetAllAsync(int limit = 0)
    {
        var messageTriggers = State.Triggers.Value.Values.AsEnumerable();
        if (limit > 0) messageTriggers = messageTriggers.Take(limit);
        return Task.FromResult(messageTriggers.Cast<IMessageTrigger>().ToImmutableList());
    }

    public Task<bool> ContainsTriggerAsync(string name)
    {
        return Task.FromResult(State.Triggers.Value.ContainsKey(name));
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var id = this.GetPrimaryKeyString();
        _guildId = ulong.Parse(id);
        await base.OnActivateAsync(cancellationToken);
    }

    protected override async Task RaiseAsync(MessageReceived @event)
    {
        var matched = State.Match(@event.Content);

        if (!matched.Any()) return;

        var message = new ExecuteTriggers(matched.Select(x => x.Value with { Name = x.Key }).ToImmutableList(),
            @event.ChannelId,
            @event.MessageId);

        await this.GetTriggersStream(_guildId).OnNextAsync(message);
    }
}

[Immutable]
[GenerateSerializer]
public record GuildReactionsState([property: Id(0)] Immutable<Dictionary<string, MessageTrigger>> Triggers,
    [property: Id(1)] ImmutableList<ulong> MutedChannels)
{
    public GuildReactionsState() :
        this(new Immutable<Dictionary<string, MessageTrigger>>(new Dictionary<string, MessageTrigger>()),
            ImmutableList<ulong>.Empty)
    {
    }

    public GuildReactionsState Add(string name, MessageTrigger messageTrigger)
    {
        var newTriggers = new Dictionary<string, MessageTrigger>(Triggers.Value) { [name] = messageTrigger };
        return this with { Triggers = newTriggers.AsImmutable() };
    }

    public GuildReactionsState Remove(string name)
    {
        var newTriggers = new Dictionary<string, MessageTrigger>(Triggers.Value);
        newTriggers.Remove(name);
        return this with { Triggers = newTriggers.AsImmutable() };
    }

    public GuildReactionsState MuteChannel(ulong channelId)
    {
        if (MutedChannels.Contains(channelId)) return this;

        return this with { MutedChannels = MutedChannels.Add(channelId) };
    }

    public GuildReactionsState UnmuteChannel(ulong channelId)
    {
        if (!MutedChannels.Contains(channelId)) return this;

        return this with { MutedChannels = MutedChannels.Remove(channelId) };
    }

    public ImmutableList<KeyValuePair<string, MessageTrigger>> Match(string eventContent)
    {
        return Triggers.Value.Where(x => x.Value.IsMatch(eventContent)).ToImmutableList();
    }
}