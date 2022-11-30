using System.Text;
using Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Orleans.Streams;
using RabbitMQ.Client;

namespace Streams;

public class RabbitMQStreamProvider : IQueueAdapter
{
    private readonly Lazy<IConnection>        _connection;
    private readonly Lazy<IConnectionFactory> _connectionFactory;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling               = TypeNameHandling.Objects,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        NullValueHandling              = NullValueHandling.Ignore
    };

    private readonly ILogger<RabbitMQStreamProvider> _logger;
    private readonly IOptions<RabbitMQSettings>      _options;

    public RabbitMQStreamProvider(ILogger<RabbitMQStreamProvider> logger, IOptions<RabbitMQSettings> options)
    {
        _logger  = logger;
        _options = options;

        _connectionFactory = new Lazy<IConnectionFactory>(() =>
        {
            var factory = new ConnectionFactory
            {
                HostName    = _options.Value.Host,
                UserName    = _options.Value.UserName,
                Password    = _options.Value.Password,
                VirtualHost = _options.Value.VirtualHost
            };
            return factory;
        });

        _connection = new Lazy<IConnection>(() => ConnectionFactory.CreateConnection());
    }

    private JsonSerializer     EventSerializer   => JsonSerializer.Create(_jsonSerializerSettings);
    public  IConnectionFactory ConnectionFactory => _connectionFactory.Value;
    public  IConnection        Connection        => _connection.Value;

    public Task QueueMessageBatchAsync<T>(StreamId streamId, IEnumerable<T> events, StreamSequenceToken token,
                                          Dictionary<string, object> requestContext)
    {
        _logger.LogInformation("Sending messages to RabbitMQ");
        var channel      = Connection.CreateModel();
        var exchangeName = "orleans";
        Parallel.ForEach(@events, @event =>
        {
            var typeName = @event!.GetType().Name;
            _logger.LogDebug("Sending message {TypeName} to RabbitMQ to stream {StreamId}", typeName, streamId);
            var       sb         = new StringBuilder();
            using var jsonWriter = new JsonTextWriter(new StringWriter(sb));
            EventSerializer.Serialize(jsonWriter, @event);
            var body  = Encoding.UTF8.GetBytes(sb.ToString());
            var props = channel.CreateBasicProperties();
            props.MessageId       = Guid.NewGuid().ToString();
            props.Timestamp       = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            props.Type            = typeName;
            props.Persistent      = true;
            props.ContentType     = "application/json";
            props.ContentEncoding = Encoding.UTF8.HeaderName;
            props.CorrelationId   = streamId.ToString().ToLowerInvariant();

            channel.BasicPublish(exchangeName, "#", props, body);
        });

        return Task.CompletedTask;
    }

    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        _logger.LogInformation("Creating RabbitMQ receiver");
        return new RabbitMQQueueAdapterReceiver(Connection, queueId);
    }

    public string                  Name         { get; } = StreamProviders.Default;
    public bool                    IsRewindable { get; } = false;
    public StreamProviderDirection Direction    { get; } = StreamProviderDirection.ReadWrite;
}