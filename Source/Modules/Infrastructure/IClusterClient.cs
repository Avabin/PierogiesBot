using Core;

namespace Infrastructure;

public interface IClusterClient
{
    IObservable<T> GetObservable<T>(string topic, string brokerName = "pub-sub") where T : IEvent;
    Task    PublishAsync(IEvent       @event,  string topic, string            brokerName = "pub-sub");
    Task<T> RequestAsync<T>(ServiceId service, string path,  CancellationToken cancellationToken = default);
    Task    InvokeAsync(ServiceId     service, string path,  CancellationToken cancellationToken = default);
}