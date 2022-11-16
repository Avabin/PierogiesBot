namespace Infrastructure.Configuration.ClientStrategies;

public interface IClientConfigurationStrategy
{
    void Apply(IClientBuilder builder);
}