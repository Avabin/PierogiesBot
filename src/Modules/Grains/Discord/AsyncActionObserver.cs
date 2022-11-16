using Orleans.Streams;

namespace Grains.Discord;

#pragma warning disable CS1998
public class AsyncActionObserver<T> : IAsyncObserver<T>
{
    private readonly Func<T, Task> _action;

    public AsyncActionObserver(Func<T, Task> action)
    {
        _action = action;
    }

    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        return Task.CompletedTask;
    }

    public Task OnNextAsync(T item, StreamSequenceToken? token = null)
    {
        return _action(item);
    }
}