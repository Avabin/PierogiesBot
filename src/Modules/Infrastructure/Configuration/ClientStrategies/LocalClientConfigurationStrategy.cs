namespace Infrastructure.Configuration.ClientStrategies;

public class LocalClientConfigurationStrategy : ClientConfigurationStrategyBase
{
    public override void Apply(IClientBuilder builder)
    {
        builder.UseLocalhostClustering();
        base.Apply(builder);
    }
}