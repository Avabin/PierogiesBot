using Infrastructure.Configuration;
using Infrastructure.Configuration.ClientStrategies;
using Infrastructure.Configuration.SiloStrategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class ApplicationHostExtensions
{
    public static HostApplicationBuilder AddSeq(this HostApplicationBuilder builder)
    {
        var seqOptions = builder.Configuration.GetRequiredSection("Seq");

        builder.Services.AddLogging(b => b.AddSeq(seqOptions));

        return builder;
    }

    public static HostApplicationBuilder UseOrleans(this HostApplicationBuilder builder, int siloPort, int gatewayPort,
                                                    string clusterId = "dev", string serviceId = "dev")
    {
        builder.Logging.AddDebug();
        var configuration = builder.Configuration;
        builder.Services.AddOrleans(siloBuilder =>
        {
            var siloTypeName = configuration.GetValue<string>("SiloType");
            var siloType =
                SiloType.Parse(siloTypeName ?? throw new InvalidOperationException("SiloType is not defined"));

            var maybeDiscordToken = configuration.GetValue<string>("DiscordToken");

            ISiloConfigurationStrategy strategy = builder.Environment.EnvironmentName switch
            {
                "Development" =>
                    new LocalSiloConfigurationStrategy(siloPort, siloType, discordToken: maybeDiscordToken),
                "Mongo" => new MongoSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                                                              configuration.GetConnectionString("MongoDB"), gatewayPort,
                                                              siloType, maybeDiscordToken),
                _ => new MongoSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                                                        configuration.GetConnectionString("MongoDB"), gatewayPort,
                                                        siloType, maybeDiscordToken)
            };

            strategy.Apply(siloBuilder, builder.Configuration);
        });

        return builder;
    }

    public static HostApplicationBuilder AddClusterClient(this HostApplicationBuilder builder, string clusterId = "dev",
                                                          string                      serviceId = "dev")
    {
        builder.Services.AddClusterClient(clusterId, serviceId, builder.Environment, builder.Configuration);

        return builder;
    }

    public static IHostBuilder AddClusterClient(this IHostBuilder builder,       IHostEnvironment environment,
                                                IConfiguration    configuration, string           clusterId = "dev",
                                                string            serviceId = "dev")
    {
        builder.ConfigureServices((context, collection) =>
        {
            collection.AddClusterClient(clusterId, serviceId, environment, configuration);
        });

        return builder;
    }

    public static IServiceCollection AddClusterClient(this IServiceCollection collection, string clusterId,
                                                      string                  serviceId,
                                                      IHostEnvironment        environment, IConfiguration configuration)
    {
        return collection.AddOrleansClient(clientBuilder =>
        {
            IClientConfigurationStrategy strategy = environment.EnvironmentName switch
            {
                "Development" => new LocalClientConfigurationStrategy(),
                "Mongo" => new MongoClientConfigurationStrategy(clusterId, serviceId,
                                                                configuration.GetConnectionString("MongoDB")),
                _ => new LocalClientConfigurationStrategy()
            };

            strategy.Apply(clientBuilder);
        });
    }
}