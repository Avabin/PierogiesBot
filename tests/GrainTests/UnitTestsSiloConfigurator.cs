using Discord;
using Infrastructure.Configuration;
using Infrastructure.Configuration.SiloStrategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Orleans.TestingHost;

namespace GrainTests;

public class UnitTestsSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        var strategy = new LocalSiloConfigurationStrategy(0, SiloType.All, false);

        strategy.Apply(siloBuilder, new ConfigurationManager());

        ConfigureServices(siloBuilder.Services);
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDiscordService>(Substitute.For<IDiscordService>());
    }
}