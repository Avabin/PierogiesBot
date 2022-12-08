using System.Net;
using System.Net.Sockets;
using Core;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;
using Orleans.Streams;
using Streams;

namespace Infrastructure.Configuration.SiloStrategies;

public class DockerSiloConfigurationStrategy : SiloConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly string _connectionString;
    private readonly int _gatewayPort;
    private readonly string _serviceId;
    private readonly int _siloPort;

    public DockerSiloConfigurationStrategy(int siloPort, string clusterId, string serviceId, string? connectionString,
        int gatewayPort, SiloType siloType, string? discordToken = "") :
        base(siloType)
    {
        _siloPort = siloPort;
        _clusterId = clusterId;
        _serviceId = serviceId;
        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));
        _connectionString = connectionString;
        _gatewayPort = gatewayPort;
    }

    public override void Apply(ISiloBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddOptions().Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));
        builder.Services.AddTransient<RabbitMQQueueAdapterFactory>();
        builder.AddStreaming().AddPersistentStreams(StreamProviders.Default, AdapterFactory, ConfigureStream);
        var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .First(x => x.AddressFamily == AddressFamily.InterNetwork);
        builder.ConfigureEndpoints(ip, _siloPort, _gatewayPort, true);
        builder.Configure<ClusterOptions>(x =>
        {
            x.ClusterId = _clusterId;
            x.ServiceId = _serviceId;
        });
        builder.UseMongoDBClient(_connectionString);
        builder.UseMongoDBClustering(options =>
        {
            options.Strategy = MongoDBMembershipStrategy.SingleDocument;
            options.ClientName = _serviceId;
            options.DatabaseName = _clusterId;
        });

        builder.AddMongoDBGrainStorageAsDefault(options =>
        {
            options.DatabaseName = _clusterId;
            options.ClientName = _serviceId;
        });

        builder.AddMongoDBGrainStorage("PubSubStore", options =>
        {
            options.ClientName = _serviceId;
            options.DatabaseName = _clusterId;
        });
        builder.UseMongoDBReminders(options =>
        {
            options.ClientName = _serviceId;
            options.DatabaseName = _clusterId;
        });
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