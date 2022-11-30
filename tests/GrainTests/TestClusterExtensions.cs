using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;

namespace GrainTests;

public static class TestClusterExtensions
{
    public static T GetService<T>(this TestCluster cluster) where T : class
    {
        var fromPrimary = ((InProcessSiloHandle) cluster.Primary).SiloHost.Services.GetRequiredService<T>();
        return fromPrimary;
    }
}