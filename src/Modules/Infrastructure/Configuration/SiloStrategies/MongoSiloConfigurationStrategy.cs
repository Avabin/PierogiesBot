using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;

namespace Infrastructure.Configuration.SiloStrategies;

public class MongoSiloConfigurationStrategy : SiloConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly string _connectionString;
    private readonly int    _gatewayPort;
    private readonly string _serviceId;
    private readonly int    _siloPort;

    public MongoSiloConfigurationStrategy(int siloPort,    string clusterId, string serviceId, string? connectionString,
                                          int gatewayPort, SiloType siloType, IConfiguration configuration) :
        base(siloType, configuration)
    {
        _siloPort  = siloPort;
        _clusterId = clusterId;
        _serviceId = serviceId;
        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));
        _connectionString = connectionString;
        _gatewayPort      = gatewayPort;
    }

    public override void Apply(ISiloBuilder builder)
    {
        builder.ConfigureEndpoints(_siloPort, _gatewayPort, listenOnAnyHostAddress: true);
        builder.Configure<ClusterOptions>(x =>
        {
            x.ClusterId = _clusterId;
            x.ServiceId = _serviceId;
        });
        builder.UseMongoDBClient(_connectionString);
        builder.UseMongoDBClustering(options =>
        {
            options.Strategy     = MongoDBMembershipStrategy.SingleDocument;
            options.ClientName   = _serviceId;
            options.DatabaseName = _clusterId;
        });

        builder.AddMongoDBGrainStorageAsDefault(options =>
        {
            options.DatabaseName = _clusterId;
            options.ClientName   = _serviceId;
        });

        builder.AddMongoDBGrainStorage("PubSubStore", options =>
        {
            options.ClientName   = _serviceId;
            options.DatabaseName = _clusterId;
        });
        builder.UseMongoDBReminders(options =>
        {
            options.ClientName   = _serviceId;
            options.DatabaseName = _clusterId;
        });
        base.Apply(builder);
    }
}