using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;

namespace GrainTests;

public abstract class OrleansTestBase<TSiloConfig, TClientConfig> : IAsyncDisposable 
    where TSiloConfig : ISiloConfigurator, new()
    where TClientConfig : IClientBuilderConfigurator, new()
{
    private   HashSet<ulong> _guildIds = new();
    protected TestCluster    Cluster                         { get; }
    protected IGrainFactory  GrainFactory                    => Cluster.GrainFactory;
    protected T              GetService<T>() where T : class => Cluster.GetService<T>();
    protected IClusterClient Client                          => Cluster.Client;
    protected virtual void ConfigureHostBuilder(IConfigurationBuilder builder)
    {
        builder.AddEnvironmentVariables("DOTNET_");
    }
    protected IConfiguration ClientConfiguration { get; }
    protected IConfiguration SiloConfiguration   { get; }

    protected OrleansTestBase()
    {
        var clientBuilder = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json");

        var siloBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var testCluster = new TestClusterBuilder().AddSiloBuilderConfigurator<TSiloConfig>()
                                                  .ConfigureHostConfiguration(ConfigureHostBuilder)
                                                  .AddClientBuilderConfigurator<TClientConfig>();
        Cluster = testCluster.Build();
        
        Cluster.Deploy();
    }

    public async ValueTask DisposeAsync()
    {
        await Cluster.StopAllSilosAsync();
    }


    protected ulong NextUlong()
    {
        var id = (ulong)new Random().NextInt64();

        return _guildIds.Add(id) ? id : NextUlong();
    }
}