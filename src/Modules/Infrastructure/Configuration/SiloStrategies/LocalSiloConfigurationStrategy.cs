using System.Net;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.SiloStrategies;

public class LocalSiloConfigurationStrategy : SiloConfigurationStrategyBase
{
    private readonly int _siloPort;

    public LocalSiloConfigurationStrategy(int siloPort, SiloType siloType, IConfiguration configuration) :
        base(siloType, configuration)
    {
        _siloPort = siloPort;
    }

    public override void Apply(ISiloBuilder builder)
    {
        builder.UseLocalhostClustering();
        builder.ConfigureEndpoints(IPAddress.Parse("127.0.0.1"), _siloPort, 30000, true);
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.AddLogStorageBasedLogConsistencyProviderAsDefault();
        base.Apply(builder);
    }
}