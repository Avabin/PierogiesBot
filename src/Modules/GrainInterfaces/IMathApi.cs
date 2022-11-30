using RestEase;

namespace GrainInterfaces;

public interface IMathApi
{
    [Post]
    Task<Stream> GetRenderAsync([Body]  string latex, [Query] int fontSize = 12, [Query] int dpi = 300,
                                [Query] bool   euler = false);

    [Get("/health")] Task<string> GetHealthAsync();
}