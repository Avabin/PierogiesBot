using Infrastructure.Configuration.ClientStrategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;

namespace GrainTests;

public class UnitTestsClientConfigurator : IClientBuilderConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        
    }

    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        var strategy = new LocalClientConfigurationStrategy(false);
        
        strategy.Apply(clientBuilder);
        
        clientBuilder.ConfigureServices(ConfigureServices);
    }
}