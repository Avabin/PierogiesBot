using Newtonsoft.Json;
using Orleans.Serialization;

namespace Infrastructure.Configuration.ClientStrategies;

public abstract class ClientConfigurationStrategyBase : IClientConfigurationStrategy
{
    public virtual void Apply(IClientBuilder builder)
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
    }
}