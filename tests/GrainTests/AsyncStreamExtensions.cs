using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord;
using Orleans.Streams;

namespace GrainTests;

public static class AsyncStreamExtensions
{
    public static Task<T> ReceiveAsync<T>(this IAsyncStream<T> stream) => stream.ToObservable().Take(1).ToTask();

    public static Task<TResult> ReceiveAsync<T, TResult>(this IAsyncStream<T> stream) where TResult : class, T =>
        stream.ToObservable()
              .Select(x => x as TResult)
              .Where(x => x is not null)
              .Select(x => x!)
              .Take(1).ToTask();
}