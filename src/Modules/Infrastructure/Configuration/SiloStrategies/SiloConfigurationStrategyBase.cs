using Core;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Streams;
using Streams;

namespace Infrastructure.Configuration.SiloStrategies;

public abstract class SiloConfigurationStrategyBase : ISiloConfigurationStrategy
{
    private readonly IConfiguration _configuration;
    private readonly SiloType       _siloType;

    protected SiloConfigurationStrategyBase(SiloType       siloType,
                                            IConfiguration configuration)
    {
        _siloType      = siloType;
        _configuration = configuration;
    }

    public virtual void Apply(ISiloBuilder builder)
    {
        builder.AddLogStorageBasedLogConsistencyProviderAsDefault();
        builder.Services.AddTransient<RabbitMQQueueAdapterFactory>();
        builder.AddStreaming().AddPersistentStreams(StreamProviders.RabbitMQ, AdapterFactory, ConfigureStream);

        // exclude grains
        builder.Configure<GrainTypeOptions>(options =>
        {
            foreach (var excludedGrain in _siloType.ExcludedGrains) options.Classes.Remove(excludedGrain);
        });

        if (_siloType != SiloTypes.Discord && _siloType != SiloTypes.All) return;

        var token = _configuration.GetValue<string>("DiscordToken");
        if (token is not null or "") builder.Services.AddDiscord(token);
    }

    private void ConfigureStream(ISiloPersistentStreamConfigurator configurator)
    {
        // empty
    }

    private IQueueAdapterFactory AdapterFactory(IServiceProvider sp, string name)
    {
        return sp.GetRequiredService<RabbitMQQueueAdapterFactory>();
    }
}