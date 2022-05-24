using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;

namespace Infrastructure;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddInfrastructure(this IHostBuilder builder)
    {
        builder.ConfigureServices(x =>
        {
            x.AddSingleton<IClusterClient, ClusterClient>()
             .AddSingleton<IEventsMediator, EventsMediator>()
             .AddMemoryCache();
            x.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                options.SerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            }).AddDapr();
        });

        builder.ConfigureLogging(x => x.ClearProviders().AddNLog("NLog.config"));

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseRouting();

        app.UseEndpoints(x =>
        {
            x.MapActorsHandlers();
            x.MapSubscribeHandler();
            x.MapControllers();
        });
        return app;
    }
}