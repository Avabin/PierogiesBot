using Core;
using Orleans.EventSourcing;
using Orleans.Streams;

namespace Grains.Core;

internal class NotifyingGrainInternal<TEventBase>
{
    private readonly IStreamProvider _streamProvider;

    public NotifyingGrainInternal(IStreamProvider streamProvider)
    {
        _streamProvider = streamProvider;
    }

    public async Task NotifyAsync(string @ns, string id, TEventBase body)
    {
        var stream = _streamProvider.GetStream<TEventBase>(@ns, id);

        await stream.OnNextAsync(body);
    }
}

public abstract class EventEmitterGrain<TEventBase> : Grain
{
    private NotifyingGrainInternal<TEventBase> _notifyingGrainInternal;

    protected EventEmitterGrain()
    {
        _notifyingGrainInternal =
            new NotifyingGrainInternal<TEventBase>(this.GetStreamProvider(StreamProviders.Default));
    }

    public virtual async Task NotifyAsync(string @ns, string id, TEventBase body)
    {
        await _notifyingGrainInternal.NotifyAsync(@ns, id, body);
    }
}

public abstract class EventEmitterGrain<TState, TEventBase> : Grain<TState>
{
    private NotifyingGrainInternal<TEventBase> _notifyingGrainInternal;

    protected EventEmitterGrain()
    {
        _notifyingGrainInternal =
            new NotifyingGrainInternal<TEventBase>(this.GetStreamProvider(StreamProviders.Default));
    }

    public virtual async Task NotifyAsync(string @ns, string id, TEventBase body)
    {
        await _notifyingGrainInternal.NotifyAsync(@ns, id, body);
    }
}

public abstract class EventEmitterJournaledGrain<TState, TEventBase> : JournaledGrain<TState, TEventBase>
    where TState : class, new() where TEventBase : class
{
    private NotifyingGrainInternal<TEventBase> _notifyingGrainInternal;

    protected EventEmitterJournaledGrain()
    {
        _notifyingGrainInternal =
            new NotifyingGrainInternal<TEventBase>(this.GetStreamProvider(StreamProviders.Default));
    }

    public virtual async Task NotifyAsync(string @ns, string id, TEventBase body)
    {
        await _notifyingGrainInternal.NotifyAsync(@ns, id, body);
    }
}