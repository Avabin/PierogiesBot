using System.Collections.Immutable;
using GrainInterfaces.Wow.Blizzard;

namespace Wow.Blizzard.Client;

public interface IBlizzardRealmsClient
{
    Task<ImmutableList<BlizzardRealm>> GetRealmsAsync(string region = "eu");

    Task<BlizzardRealm?> GetRealmAsync(string slug);
}