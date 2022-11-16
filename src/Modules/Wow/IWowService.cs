using GrainInterfaces.Wow;

namespace Wow;

public interface IWowService
{
    Task<CharacterView?> FetchCharacterAsync(string server, string realm, string name);
}