using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;

namespace Infrastructure.Configuration.ClientStrategies;

public class MongoClientConfigurationStrategy : ClientConfigurationStrategyBase
{
    private readonly string _clusterId;
    private readonly string _connectionString;
    private readonly string _serviceId;

    public MongoClientConfigurationStrategy(string clusterId, string serviceId, string? connectionString)
    {
        if (connectionString is null or "")
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty");
        _clusterId        = clusterId;
        _serviceId        = serviceId;
        _connectionString = connectionString;
    }

    public override void Apply(IClientBuilder builder)
    {
        builder.Configure<ClusterOptions>(x =>
        {
            x.ClusterId = _clusterId;
            x.ServiceId = _serviceId;
        });
        builder.UseMongoDBClient(_connectionString);
        builder.UseMongoDBClustering(options =>
        {
            options.DatabaseName = _clusterId;
            options.ClientName   = _serviceId;
            options.Strategy     = MongoDBMembershipStrategy.SingleDocument;
        });
        base.Apply(builder);
    }
}