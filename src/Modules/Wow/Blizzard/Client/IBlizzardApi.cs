using System.Runtime.CompilerServices;
using GrainInterfaces.Wow.Blizzard;
using RestEase;

[assembly: InternalsVisibleTo(RestClient.FactoryAssemblyName)]

namespace Wow.Blizzard.Client;

internal interface IBlizzardApi
{
    [Header("Authorization")] string Authorization { get; set; }

    [Get("https://{region}.api.blizzard.com/profile/wow/character/{realm}/{name}?namespace=profile-{region}&locale=en_GB")]
    Task<Response<BlizzardCharacter>> GetCharacterAsync([Path]                         string realm, [Path] string name,
                                                        [Query(Name = "access_token")] string accessToken,
                                                        [Path("region")]               string region);

    [Get("https://{region}.api.blizzard.com/data/wow/realm?namespace=dynamic-{region}&locale=en_GB")]
    Task<Response<BlizzardRealms>> GetRealmsAsync([Query(Name = "access_token")] string accessToken,
                                                  [Path("region")]               string region);

    [Get("https://{region}.api.blizzard.com/data/wow/realm/{realmSlug}?namespace=dynamic-{region}&locale=en_GB")]
    Task<Response<RealmResponse>> GetRealmAsync([Path]                         string realmSlug,
                                                [Query(Name = "access_token")] string accessToken,
                                                [Path("region")]               string region);
}

internal class BlizzardRealms
{
    public List<BlizzardRealm> Realms { get; set; } = new();
}

internal class BlizzardCharacter
{
    public ulong         Id    { get; set; } = 0;
    public string        Name  { get; set; } = "";
    public BlizzardRealm Realm { get; set; } = new();
    public int           Level { get; set; } = 0;
}