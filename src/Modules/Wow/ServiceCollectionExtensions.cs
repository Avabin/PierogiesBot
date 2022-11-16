using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Wow.Blizzard;
using Wow.Blizzard.Client;
using Wow.Warmane;
using Wow.Warmane.Client;

namespace Wow;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWowServices(this IServiceCollection    services,
                                                    Action<WowBlizzardOptions> configureBlizzard)
    {
        services.Configure(configureBlizzard);
        services.AddMemoryCache();
        services.AddSingleton<IWowService, WowService>();

        services.AddSingleton<IWowDataSource, WarmaneDataSource>();
        services.AddSingleton((_) => RestClient.For<IWarmaneApi>());

        services.AddSingleton<IWowDataSource, BlizzardDataSource>();
        services.AddSingleton((_) => RestClient.For<IBlizzardApi>());

        services.AddSingleton<IBlizzardTokenClient, BlizzardTokenClient>();
        services.AddSingleton((_) => RestClient.For<IBlizzardTokenApi>());

        services.AddSingleton<IBlizzardRealmsClient, BlizzardRealmsClient>();

        return services;
    }
}