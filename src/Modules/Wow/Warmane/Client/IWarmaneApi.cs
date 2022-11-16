using System.Runtime.CompilerServices;
using RestEase;

[assembly: InternalsVisibleTo(RestClient.FactoryAssemblyName)]

namespace Wow.Warmane.Client;

[Header("Accept", "application/json")]
[BaseAddress("http://armory.warmane.com/")]
internal interface IWarmaneApi
{
    [Get("api/character/{name}/{realm}/summary")]
    Task<Response<CharacterResponse?>> GetCharacterAsyncResponse([Path] string name, [Path] string realm);
}