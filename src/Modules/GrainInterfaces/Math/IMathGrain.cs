namespace GrainInterfaces.Math;

public interface IMathGrain : IGrainWithStringKey
{
    Task<byte[]> RenderAsync(string expression, int dpi = 300, int fontSize = 12);
}