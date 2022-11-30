using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orleans.Configuration;
using Orleans.Serialization;

namespace Infrastructure.Configuration.SiloStrategies;

public abstract class SiloConfigurationStrategyBase : ISiloConfigurationStrategy
{
    private readonly SiloType _siloType;

    protected SiloConfigurationStrategyBase(SiloType siloType)
    {
        _siloType = siloType;
    }

    public virtual void Apply(ISiloBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddNewtonsoftJsonSerializer(type => type.Namespace.StartsWith("Shared"), options =>
            {
                options.Configure(x =>
                {
                    x.SerializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                    };
                });
            });
        });
        builder.AddReminders();
        builder.AddLogStorageBasedLogConsistencyProviderAsDefault();

        // exclude grains
        builder.Configure<GrainTypeOptions>(options =>
        {
            foreach (var excludedGrain in _siloType.ExcludedGrains) options.Classes.Remove(excludedGrain);
        });
    }
}