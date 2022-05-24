using System.Reactive.Linq;
using Core;
using Dapr.Client;

namespace Infrastructure;

internal class ClusterClient : IClusterClient
{
    private readonly        DaprClient      _daprClient;
    private readonly        IEventsMediator _mediator;
    private static string _daprHttp = $"http://localhost:{Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")}";
    private static readonly HttpClient      HttpClient = new()
    {
        BaseAddress = new Uri(_daprHttp)
    };
    public ClusterClient(DaprClient daprClient, IEventsMediator mediator)
    {
        _daprClient    = daprClient;
        _mediator = mediator;
        
    }

    public IObservable<T> GetObservable<T>(string topic, string brokerName = "pub-sub") where T : IEvent => 
        _mediator.GetObservableForTopic(topic, brokerName).Select(x => x.Event).OfType<T>();

    public async Task PublishAsync(IEvent @event, string topic, string brokerName = "pub-sub")
    {
        var serialized       = @event.ToMessageStringBody();
        var content          = new StringContent(serialized, System.Text.Encoding.UTF8, "text/plain");
        
        var result =await HttpClient.PostAsync($"{_daprHttp}/v1.0/publish/{brokerName}/{topic}", content);
        var resultContent = await result.Content.ReadAsStringAsync();
    }

    public async Task<T> RequestAsync<T>(ServiceId service, string path, CancellationToken cancellationToken = default)
    {
        var result = await _daprClient.InvokeMethodAsync<T>(service.Value, path, cancellationToken);
        
        return result;
    }

    public async Task InvokeAsync(ServiceId service, string path, CancellationToken cancellationToken = default) => 
        await _daprClient.InvokeMethodAsync(service.Value, path, cancellationToken);
}