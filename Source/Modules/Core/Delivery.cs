using System.Diagnostics;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Core;

public class Delivery
{
    public string  DataContentType { get; set; } = "application/json";
    public string  Source          { get; set; } = "";
    public string  Subject         { get; set; } = "";
    public string  Type            { get; set; } = "";
    public Guid    Id              { get; set; } = Guid.Empty;
    public string  PubSubName      { get; set; } = "";
    public string  Topic           { get; set; } = "";
    public string  SpecVersion     { get; set; } = "1.0";
    public string  TraceId         { get; set; } = "";
    public string  TraceParent     { get; set; } = "";
    public string? Data            { get; set; }

    [JsonIgnore] public IEvent Event => Data.ToEvent<IEvent>();
    
    public static Delivery Of(Event @event, string source = "", string subject = "", Guid? correlationId = null, string brokerName = "pub-sub", string topic = "", string specVersion = "1.0", ActivityTraceId? traceId = null, ActivityTraceId? parentTrace = null) => new Delivery
    {
        DataContentType = "application/json",
        Source          = source,
        Subject         = subject,
        Type            = @event.GetType().Name,
        Id              = correlationId ?? Guid.NewGuid(),
        PubSubName      = brokerName,
        Topic           = topic,
        SpecVersion     = specVersion,
        TraceId         = traceId.GetValueOrDefault(ActivityTraceId.CreateRandom()).ToString(),
        TraceParent     = parentTrace.GetValueOrDefault(ActivityTraceId.CreateRandom()).ToString(),
        Data            = @event.ToMessageStringBody()
    };
}