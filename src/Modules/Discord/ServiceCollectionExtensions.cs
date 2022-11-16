using Microsoft.Extensions.DependencyInjection;

namespace Discord;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordSettings> configure)
    {
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>()
                .AddOptions<DiscordSettings>().Configure(configure);
        return services;
    }

    public static IServiceCollection AddDiscord(this IServiceCollection services, string token)
    {
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>()
                .AddOptions<DiscordSettings>().Configure(x => x.Token = token);
        return services;
    }
}