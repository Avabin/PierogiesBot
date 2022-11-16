using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Providers.Streams.Common;
using Orleans.Streams;

namespace Streams;

public class RabbitMQQueueAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory _factory;

    public RabbitMQQueueAdapterFactory(ILoggerFactory factory)
    {
        _factory = factory;
    }

    public Task<IQueueAdapter> CreateAdapter()
    {
        return
            Task.FromResult<IQueueAdapter>(new RabbitMQStreamProvider(_factory.CreateLogger<RabbitMQStreamProvider>()));
    }

    public IQueueAdapterCache GetQueueAdapterCache()
    {
        return new SimpleQueueAdapterCache(new SimpleQueueCacheOptions()
        {
            CacheSize = 256
        }, "RabbitMQ", _factory);
    }

    public IStreamQueueMapper GetStreamQueueMapper()
    {
        return new HashRingBasedStreamQueueMapper(new HashRingStreamQueueMapperOptions
        {
            TotalQueueCount = 1
        }, "orleans");
    }

    public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
    {
        return Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());
    }
}