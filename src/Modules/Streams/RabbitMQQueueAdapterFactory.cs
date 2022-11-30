using Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Providers.Streams.Common;
using Orleans.Streams;

namespace Streams;

public class RabbitMQQueueAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory             _factory;
    private readonly IOptions<RabbitMQSettings> _options;

    public RabbitMQQueueAdapterFactory(ILoggerFactory factory, IOptions<RabbitMQSettings> options)
    {
        _factory = factory;
        _options = options;
    }

    public Task<IQueueAdapter> CreateAdapter()
    {
        return
            Task.FromResult<IQueueAdapter>(new RabbitMQStreamProvider(_factory.CreateLogger<RabbitMQStreamProvider>(),
                                                                      _options));
    }

    public IQueueAdapterCache GetQueueAdapterCache()
    {
        return new SimpleQueueAdapterCache(new SimpleQueueCacheOptions()
        {
            CacheSize = 256
        }, StreamProviders.Default, _factory);
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