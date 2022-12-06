using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Streams;
using Streams;

namespace Infrastructure.Configuration.ClientStrategies;

public class AzureClientConfigurationStrategy : ClientConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly IConfigurationSection _configure;
    private readonly string _connectionString;
    private readonly string _serviceId;

    public AzureClientConfigurationStrategy(string clusterId, string serviceId, string? connectionString,
        IConfigurationSection configure)
    {
        if (connectionString is null or "")
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty");
        _clusterId = clusterId;
        _serviceId = serviceId;
        _connectionString = connectionString;
        _configure = configure;
    }

    public override void Apply(IClientBuilder builder)
    {
        builder.Services.AddSingleton<RabbitMQQueueAdapterFactory>().Configure<RabbitMQSettings>(_configure);
        builder.AddStreaming().AddPersistentStreams(StreamProviders.Default, AdapterFactory, ConfigureStream);
        builder.Configure<ClusterOptions>(x =>
        {
            x.ClusterId = _clusterId;
            x.ServiceId = _serviceId;
        });
        builder.UseAzureStorageClustering(optionsBuilder =>
        {
            optionsBuilder.ConfigureTableServiceClient(_connectionString);
        });
        base.Apply(builder);
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