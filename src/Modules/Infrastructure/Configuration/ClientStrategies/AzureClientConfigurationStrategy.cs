using Core;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Streams;
using Streams;

namespace Infrastructure.Configuration.ClientStrategies;

public class AzureClientConfigurationStrategy : ClientConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly string _connectionString;
    private readonly string _serviceId;

    public AzureClientConfigurationStrategy(string clusterId, string serviceId, string? connectionString)
    {
        if (connectionString is null or "")
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty");
        _clusterId = clusterId;
        _serviceId = serviceId;
        _connectionString = connectionString;
    }

    public override void Apply(IClientBuilder builder)
    {
        builder.AddAzureQueueStreams(StreamProviders.Default,
            options => { options.Configure(x => x.ConfigureQueueServiceClient(_connectionString)); });
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