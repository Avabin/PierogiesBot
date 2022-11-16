namespace Core;

/// <summary>
/// Async lazy from https://devblogs.microsoft.com/pfxteam/asynclazyt/
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) :
        base(() => Task.Factory.StartNew(valueFactory))
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    {
    }
}