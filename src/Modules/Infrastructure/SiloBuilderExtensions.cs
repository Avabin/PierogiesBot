using Infrastructure.Configuration.SiloStrategies;

namespace Infrastructure;

public static class SiloBuilderExtensions
{
    public static ISiloBuilder UseStrategy(this ISiloBuilder builder, ISiloConfigurationStrategy strategy)
    {
        strategy.Apply(builder);

        return builder;
    }
}