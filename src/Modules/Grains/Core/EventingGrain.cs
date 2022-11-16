using Grains.Discord;
using Orleans.Streams.Core;

namespace Grains.Core;

internal class EventingGrainInternal<TEventBase> : IStreamSubscriptionObserver
{
    private readonly AsyncActionObserver<TEventBase> _observer;

    internal EventingGrainInternal(Func<TEventBase, Task> handler)
    {
        _observer = new AsyncActionObserver<TEventBase>(handler);
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<TEventBase>();
        await handle.ResumeAsync(_observer);
    }
}

public abstract class EventingGrain<TState, TEventBase> : Grain<TState>, IStreamSubscriptionObserver
{
    private EventingGrainInternal<TEventBase> EventingGrainInternal => new(RaiseAsync);

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        await EventingGrainInternal.OnSubscribed(handleFactory);
    }

    protected abstract Task RaiseAsync(TEventBase @event);
}

public abstract class EventingGrain<TEventBase> : Grain, IStreamSubscriptionObserver
{
    private EventingGrainInternal<TEventBase> EventingGrainInternal => new(RaiseAsync);

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        await EventingGrainInternal.OnSubscribed(handleFactory);
    }

    protected abstract Task RaiseAsync(TEventBase @event);
}