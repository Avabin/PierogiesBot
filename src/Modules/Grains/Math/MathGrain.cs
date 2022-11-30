using GrainInterfaces;
using GrainInterfaces.Math;

namespace Grains.Math;

public class MathGrain : Grain, IMathGrain
{
    private readonly IMathApi _api;

    public MathGrain(IMathApi api)
    {
        _api = api;
    }

    public async Task<byte[]> RenderAsync(string expression, int dpi = 300, int fontSize = 12)
    {
        var stream = await _api.GetRenderAsync(expression, fontSize, dpi);

        // read stream to byte array
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}