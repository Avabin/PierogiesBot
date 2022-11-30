using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

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
        builder.AddReminders();
        builder.AddLogStorageBasedLogConsistencyProviderAsDefault();

        // exclude grains
        builder.Configure<GrainTypeOptions>(options =>
        {
            foreach (var excludedGrain in _siloType.ExcludedGrains) options.Classes.Remove(excludedGrain);
        });
    }
}