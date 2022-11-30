using GrainInterfaces.Wow;

namespace Wow;

public interface IWowService
{
    Task<WowCharacterView?> FetchCharacterAsync(string server, string realm, string name);
}