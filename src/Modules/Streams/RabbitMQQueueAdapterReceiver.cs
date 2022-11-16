using System.Reactive.Disposables;
using System.Text;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using RabbitMQ.Client;

namespace Streams;

public class RabbitMQQueueAdapterReceiver : IQueueAdapterReceiver, IDisposable
{
    private readonly Lazy<RabbitMQObservableModel> _channel;
    private readonly IConnection                   _connection;
    private readonly CompositeDisposable           _disposables = new();

    public RabbitMQQueueAdapterReceiver(IConnection connection, QueueId queueId)
    {
        _connection = connection;
        var queueId1 = queueId;

        _channel = new Lazy<RabbitMQObservableModel>(() =>
        {
            var channel = _connection.CreateModel();
            channel.ExchangeDeclare("orleans", ExchangeType.Topic, true);
            channel.QueueDeclare(queueId1.ToString(), true, false, false);
            channel.QueueBind(queueId1.ToString(), "orleans", "#");
            return new RabbitMQObservableModel(channel, queueId);
        });
    }

    private RabbitMQObservableModel Channel => _channel.Value;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task Initialize(TimeSpan timeout)
    {
        var sub = Channel.Subscribe();
        _disposables.Add(sub);
        return Task.CompletedTask;
    }

    public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        var msgs = Channel.Get(maxCount)
                          .GroupBy(x => x.BasicProperties.CorrelationId)
                          .Select((x, i) =>
                           {
                               var streamId = StreamId.Parse(Encoding.UTF8.GetBytes(x.Key));
                               return new RabbitMQBatchContainer(streamId, new EventSequenceTokenV2(i),
                                                                 x.AsEnumerable().Select(Delivery.From));
                           }).Cast<IBatchContainer>()
                          .ToList()
                          .AsReadOnly();

        return Task.FromResult<IList<IBatchContainer>>(msgs);
    }

    public async Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        await Task.Run(() =>
        {
            var channel = Channel;

            foreach (var message in messages)
            {
                var batch             = (RabbitMQBatchContainer)message;
                var latestDeliveryTag = batch.GetLatestDeliveryTag();
                channel.Model.BasicAck((ulong)latestDeliveryTag, true);
            }
        });
    }

    public Task Shutdown(TimeSpan timeout)
    {
        Channel.Model.Close();
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _connection.Dispose();
        _disposables.Dispose();
    }
}