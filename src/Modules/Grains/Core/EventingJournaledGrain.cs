using Grains.Discord;
using Orleans.EventSourcing;
using Orleans.Streams.Core;

namespace Grains.Core;

public abstract class EventingJournaledGrain<TState, TEventBase> : JournaledGrain<TState, TEventBase>,
                                                                   IStreamSubscriptionObserver
    where TEventBase : class where TState : class, new()
{
    private readonly AsyncActionObserver<TEventBase> _observer;

    protected EventingJournaledGrain()
    {
        _observer = new AsyncActionObserver<TEventBase>(RaiseAsync);
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<TEventBase>();
        await handle.ResumeAsync(_observer);
    }

    public async Task RaiseAsync(TEventBase @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
    }
}