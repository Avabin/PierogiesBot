using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discord;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordSettings> configure)
    {
        services.AddHostedService<DiscordHostedService>();
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>()
                .AddOptions<DiscordSettings>().Configure(configure);
        return services;
    }

    public static IServiceCollection AddDiscord(this IServiceCollection services, string token)
    {
        return services.AddDiscord(x => x.Token = token);
    }

    /// <summary>
    /// Installs interactions from a given assembly
    /// </summary>
    /// <param name="services">this</param>
    /// <param name="token">discord token</param>
    /// <typeparam name="T">Assembly marker type</typeparam>
    /// <returns>this</returns>
    public static IServiceCollection AddDiscordCommands<T>(this IServiceCollection services, string token)
    {
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>()
                .AddOptions<DiscordSettings>().Configure(x =>
                 {
                     x.Token = token;
                     x.CommandsAssemblies.Add(typeof(T).Assembly.FullName!);
                 });
        services.AddHostedService<DiscordHostedService>();
        return services;
    }

    /// <summary>
    /// Installs interactions from a given assembly
    /// </summary>
    /// <param name="services">this</param>
    /// <param name="configure">configure settings</param>
    /// <typeparam name="T">Assembly marker type</typeparam>
    /// <returns>this</returns>
    public static IServiceCollection AddDiscordCommands<T>(this IServiceCollection services,
                                                           Action<DiscordSettings> configure)
    {
        services.AddOptions().Configure(configure);
        services.Configure<DiscordSettings>(x => x.CommandsAssemblies.Add(typeof(T).Assembly.FullName!));
        services.AddHostedService<DiscordHostedService>();
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>();
        return services;
    }

    /// <summary>
    /// Installs interactions from a given assembly
    /// </summary>
    /// <param name="services">this</param>
    /// <param name="section">Configuration section</param>
    /// <returns>this</returns>
    public static IServiceCollection AddDiscordCommands(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions().Configure<DiscordSettings>(section);
        services.AddHostedService<DiscordHostedService>();
        services.AddSingleton<IDiscordMessagesWatcherService, DiscordMessagesWatcherService>();
        services.AddSingleton<IDiscordService, DiscordService>();
        return services;
    }

    /// <summary>
    /// Installs interactions from a given assembly
    /// </summary>
    /// <param name="services">this</param>
    /// <param name="section">Configuration section</param>
    /// <returns>this</returns>
    public static IServiceCollection AddDiscordCommands<T>(this IServiceCollection services,
                                                           IConfigurationSection   section)
    {
        return services.AddDiscordCommands<T>(section.Bind);
    }
}