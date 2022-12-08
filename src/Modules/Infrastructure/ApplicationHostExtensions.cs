using Azure.Monitor.OpenTelemetry.Exporter;
using Infrastructure.Configuration;
using Infrastructure.Configuration.ClientStrategies;
using Infrastructure.Configuration.SiloStrategies;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Events;

namespace Infrastructure;

public static class ApplicationHostExtensions
{
    public static WebApplicationBuilder UseOrleans(this WebApplicationBuilder builder, int siloPort, int gatewayPort,
        string clusterId = "dev", string serviceId = "dev")
    {
        var seqUrl = builder.Configuration.GetValue<string>("Seq:ServerUrl");
        if (seqUrl is not ("" or null))
        {
            var logger = new LoggerConfiguration()
                .Filter.ByExcluding(@event =>
                    @event.Properties["SourceContext"].ToString().Replace("\"", "").StartsWith("Microsoft"))
                .Filter.ByExcluding(@event =>
                    @event.Properties["SourceContext"].ToString().Replace("\"", "").StartsWith("Orleans") &&
                    @event.Level < LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .MinimumLevel.Verbose().WriteTo.Console()
                .MinimumLevel.Debug().WriteTo.Seq(seqUrl)
                .CreateLogger();

            builder.Logging.ClearProviders().AddSerilog(logger);
        }
        else
        {
            builder.Logging.AddConsole();
        }

        builder.Configuration.AddEnvironmentVariables("DOTNET_");
        builder.Configuration.AddEnvironmentVariables("ASPNETCORE_");
        var configuration = builder.Configuration;
        var appInsightsConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
        if (appInsightsConnectionString is not (null or ""))
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            });
        builder.Host.UseOrleans((context, siloBuilder) =>
        {
            siloBuilder.AddActivityPropagation();
            if (appInsightsConnectionString is not (null or ""))
                siloBuilder.Configure<OpenTelemetryLoggerOptions>(options =>
                {
                    options.IncludeScopes = true;
                    options.AddAzureMonitorLogExporter(exporterOptions =>
                    {
                        exporterOptions.ConnectionString = appInsightsConnectionString;
                    });
                });
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
                "Docker" => new DockerSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken),
                "Kubernetes" => new KubernetesSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken),
                "Azure" => new AzureSiloConfigurationStrategy(siloType, clusterId, serviceId),
                _ => new MongoSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken)
            };

            strategy.Apply(siloBuilder, builder.Configuration);
        });
        return builder;
    }

    public static HostApplicationBuilder UseOrleans(this HostApplicationBuilder builder, int siloPort, int gatewayPort,
        string clusterId = "dev", string serviceId = "dev")
    {
        var configuration = builder.Configuration;

        builder.Services.AddOrleans(siloBuilder =>
        {
            siloBuilder.AddActivityPropagation();
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
                "Kubernetes" => new KubernetesSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken),
                "Docker" => new DockerSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken),
                "Azure" => new AzureSiloConfigurationStrategy(siloType, clusterId, serviceId),
                _ => new MongoSiloConfigurationStrategy(siloPort, clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB"), gatewayPort,
                    siloType, maybeDiscordToken)
            };

            strategy.Apply(siloBuilder, builder.Configuration);
        });

        return builder;
    }

    public static HostApplicationBuilder AddClusterClient(this HostApplicationBuilder builder, string clusterId = "dev",
        string serviceId = "dev")
    {
        builder.Services.AddClusterClient(clusterId, serviceId, builder.Environment, builder.Configuration);

        return builder;
    }

    public static IHostBuilder AddClusterClient(this IHostBuilder builder, IHostEnvironment environment,
        IConfiguration configuration, string clusterId = "dev",
        string serviceId = "dev")
    {
        builder.ConfigureServices((context, collection) =>
        {
            collection.AddClusterClient(clusterId, serviceId, environment, configuration);
        });

        return builder;
    }

    public static IServiceCollection AddClusterClient(this IServiceCollection collection, string clusterId,
        string serviceId,
        IHostEnvironment environment, IConfiguration configuration)
    {
        return collection.AddOrleansClient(clientBuilder =>
        {
            IClientConfigurationStrategy strategy = environment.EnvironmentName switch
            {
                "Development" => new LocalClientConfigurationStrategy(),
                "Mongo" => new MongoClientConfigurationStrategy(clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB")),
                "Docker" => new MongoClientConfigurationStrategy(clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB")),
                "Kubernetes" => new KubernetesClientConfigurationStrategy(clusterId, serviceId,
                    configuration.GetConnectionString("MongoDB")),
                "Azure" => new AzureClientConfigurationStrategy(clusterId, serviceId,
                    configuration.GetConnectionString("AzureStorage"),
                    configuration.GetRequiredSection("RabbitMQSettings")),
                _ => new LocalClientConfigurationStrategy()
            };

            strategy.Apply(clientBuilder);
        });
    }
}