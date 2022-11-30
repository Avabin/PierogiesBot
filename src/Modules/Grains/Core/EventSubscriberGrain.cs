using Orleans.Streams.Core;

namespace Grains.Core;

internal class EventSubscriberGrainInternal<TEventBase> : IStreamSubscriptionObserver
{
    private readonly AsyncActionObserver<TEventBase> _observer;

    internal EventSubscriberGrainInternal(Func<TEventBase, Task> handler)
    {
        _observer = new AsyncActionObserver<TEventBase>(handler);
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<TEventBase>();
        await handle.ResumeAsync(_observer);
    }
}

public abstract class EventSubscriberGrain<TState, TEventBase> : Grain<TState>, IStreamSubscriptionObserver
{
    private EventSubscriberGrainInternal<TEventBase> EventSubscriberGrainInternal => new(RaiseAsync);

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        await EventSubscriberGrainInternal.OnSubscribed(handleFactory);
    }

    protected abstract Task RaiseAsync(TEventBase @event);
}

public abstract class EventSubscriberGrain<TEventBase> : Grain, IStreamSubscriptionObserver
{
    private EventSubscriberGrainInternal<TEventBase> EventSubscriberGrainInternal => new(RaiseAsync);

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        await EventSubscriberGrainInternal.OnSubscribed(handleFactory);
    }

    protected abstract Task RaiseAsync(TEventBase @event);
}