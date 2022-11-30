using Core;

namespace Infrastructure.Configuration.ClientStrategies;

public class LocalClientConfigurationStrategy : ClientConfigurationStrategyBase
{
    private readonly bool _configureGateway;

    public LocalClientConfigurationStrategy(bool configureGateway = true)
    {
        _configureGateway = configureGateway;
    }

    public override void Apply(IClientBuilder builder)
    {
        if (_configureGateway) builder.UseLocalhostClustering();
        builder.AddStreaming().AddMemoryStreams(StreamProviders.Default);
        base.Apply(builder);
    }
}