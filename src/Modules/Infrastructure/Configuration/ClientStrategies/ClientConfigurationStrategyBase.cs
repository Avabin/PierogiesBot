using Core;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Streams;
using Streams;

namespace Infrastructure.Configuration.ClientStrategies;

public abstract class ClientConfigurationStrategyBase : IClientConfigurationStrategy
{
    public virtual void Apply(IClientBuilder builder)
    {
        builder.Services.AddTransient<RabbitMQQueueAdapterFactory>();
        builder.AddStreaming().AddPersistentStreams(StreamProviders.RabbitMQ, AdapterFactory, ConfigureStream);
    }

    private void ConfigureStream(IClusterClientPersistentStreamConfigurator obj)
    {
        // empty
    }

    private IQueueAdapterFactory AdapterFactory(IServiceProvider sp, string name)
    {
        return sp.GetRequiredService<RabbitMQQueueAdapterFactory>();
    }
}