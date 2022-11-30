using Discord;
using Infrastructure.Configuration;
using Infrastructure.Configuration.SiloStrategies;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;

namespace GrainTests;

public class UnitTestsSiloConfigurator : ISiloConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDiscordService>(NSubstitute.Substitute.For<IDiscordService>());
    }
    public void Configure(ISiloBuilder siloBuilder)
    {
        
        var strategy = new LocalSiloConfigurationStrategy(0, SiloType.All, false);

        strategy.Apply(siloBuilder);
        
        ConfigureServices(siloBuilder.Services);
    }
}