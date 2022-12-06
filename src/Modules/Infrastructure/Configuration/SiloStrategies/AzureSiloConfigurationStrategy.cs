using System.Net;
using System.Net.Sockets;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Streams;
using Streams;

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
        var hostname = Dns.GetHostName();
        var outboundIp = Dns.GetHostEntry(hostname).AddressList
            .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        builder.ConfigureEndpoints(outboundIp, 11111, 30000, true);

        builder.Services.AddSingleton<RabbitMQQueueAdapterFactory>()
            .Configure<RabbitMQSettings>(configuration.GetRequiredSection("RabbitMQSettings"));
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

        builder.AddStreaming().AddPersistentStreams(StreamProviders.Default, AdapterFactory, ConfigureStream);

        base.Apply(builder, configuration);
    }


    private void ConfigureStream(ISiloPersistentStreamConfigurator configurator)
    {
        // empty
    }

    private IQueueAdapterFactory AdapterFactory(IServiceProvider sp, string name)
    {
        return sp.GetRequiredService<RabbitMQQueueAdapterFactory>();
    }
}