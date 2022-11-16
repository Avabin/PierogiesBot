using System.Collections.Immutable;
using Core;
using GrainInterfaces.Discord.Guilds.Events;
using GrainInterfaces.Discord.Guilds.Grains;
using GrainInterfaces.Discord.Guilds.MessageTriggers;
using Grains.Core;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace Grains.Discord.Guilds;

[ImplicitStreamSubscription(StreamNamespaces.MessagesWatcher)]
public class GuildTriggersMessageWatcherGrain : EventingGrain<GuildReactionsState, MessageReceived>,
                                                IGuildTriggersMessageWatcherGrain
{
    public async Task AddAsync(string name, MessageTrigger messageTrigger)
    {
        State = State.Add(name, messageTrigger);
        await WriteStateAsync();
    }

    public async Task RemoveAsync(string name)
    {
        State = State.Remove(name);
        await WriteStateAsync();
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

    public Task<ImmutableList<MessageTrigger>> GetAllAsync()
    {
        return Task.FromResult(State.Triggers.Value.Select(x => x.Value).ToImmutableList());
    }

    protected override async Task RaiseAsync(MessageReceived @event)
    {
        var matched = State.Triggers.Value.Where(x => x.Value.IsMatch(@event.Content)).ToList();

        if (!matched.Any()) return;

        var message = new ExecuteTriggers(matched.Select(x => x.Value with { Name = x.Key }).ToImmutableList(),
                                          @event.ChannelId,
                                          @event.MessageId);

        await this.GetStreamProvider(StreamProviders.RabbitMQ)
                  .GetStream<RuleEvent>(StreamId.Create(StreamNamespaces.RuleHits, this.GetPrimaryKeyString()))
                  .OnNextAsync(message);
    }
}

[Immutable]
[GenerateSerializer]
public record GuildReactionsState([property: Id(0)] Immutable<Dictionary<string, MessageTrigger>> Triggers,
                                  [property: Id(1)] ImmutableList<ulong>                          MutedChannels)
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
}