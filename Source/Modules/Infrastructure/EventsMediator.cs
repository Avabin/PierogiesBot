using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure;

internal class EventsMediator : IEventsMediator
{
    private IMemoryCache     _cache;
    public EventsMediator(IMemoryCache cache)
    {
        _cache = cache;
    }
    public IObservable<Delivery> GetObservableForTopic(string topic, string brokerName = "pub-sub")
    {
        var existing = _cache.GetOrCreate(CreateCacheKey(topic, brokerName), _ => new Subject<Delivery>());
        
        return existing.AsObservable();
    }

    public void Publish(Delivery @event, string topic, string brokerName = "pub-sub") => _cache.GetOrCreate(CreateCacheKey(topic, brokerName), _ => 
                                                                                                               new Subject<Delivery>()).OnNext(@event);

    private string CreateCacheKey(string topic, string brokerName = "pub-sub") => $"{brokerName}:{topic}";
}