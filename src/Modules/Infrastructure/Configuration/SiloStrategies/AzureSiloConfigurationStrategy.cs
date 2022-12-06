using System.Net;
using Core;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

namespace Infrastructure.Configuration.SiloStrategies;

public class AzureSiloConfigurationStrategy : SiloConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly string _serviceId;

    public AzureSiloConfigurationStrategy(SiloType siloType, string clusterId, string serviceId) : base(siloType)
    {
        _clusterId = clusterId;
        _serviceId = serviceId;
    }

    public override void Apply(ISiloBuilder builder, IConfiguration configuration)
    {
        builder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = _clusterId;
            options.ServiceId = _serviceId;
        });
        builder.ConfigureEndpoints(Dns.GetHostName(), 11111, 30000);
        var connectionString = configuration.GetConnectionString("AzureStorage");
        builder.UseAzureStorageClustering(options => { options.ConfigureTableServiceClient(connectionString); });
        builder.AddAzureTableGrainStorageAsDefault(options =>
            options.Configure(storageOptions =>
                storageOptions.ConfigureTableServiceClient(connectionString)));

        builder.AddAzureBlobGrainStorage("PubSubStore", options =>
            options.Configure(storageOptions =>
                storageOptions.ConfigureBlobServiceClient(connectionString)));

        builder.UseAzureTableReminderService(options =>
            options.ConfigureTableServiceClient(connectionString));

        builder.AddAzureQueueStreams(StreamProviders.Default,
            configurator => { configurator.Configure(x => { x.ConfigureQueueServiceClient(connectionString); }); });

        base.Apply(builder, configuration);
    }
}