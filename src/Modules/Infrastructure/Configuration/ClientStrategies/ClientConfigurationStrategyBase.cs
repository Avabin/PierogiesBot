namespace Infrastructure.Configuration.ClientStrategies;

public abstract class ClientConfigurationStrategyBase : IClientConfigurationStrategy
{
    public virtual void Apply(IClientBuilder builder)
    {
    }
}