using System.Net;
using Core;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.SiloStrategies;

public class LocalSiloConfigurationStrategy : SiloConfigurationStrategyBase
{
    private readonly bool _configureEndpoint;
    private readonly int  _siloPort;

    public LocalSiloConfigurationStrategy(int     siloPort, SiloType siloType, bool configureEndpoint = true,
                                          string? discordToken = "") :
        base(siloType)
    {
        _siloPort          = siloPort;
        _configureEndpoint = configureEndpoint;
    }

    public override void Apply(ISiloBuilder builder, IConfiguration configuration)
    {
        if (_configureEndpoint)
        {
            builder.UseLocalhostClustering();
            builder.ConfigureEndpoints(IPAddress.Parse("127.0.0.1"), _siloPort, 30000, true);
        }

        builder.UseInMemoryReminderService();
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.AddMemoryStreams(StreamProviders.Default);
        base.Apply(builder, configuration);
    }
}