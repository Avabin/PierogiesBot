using System.Collections.Immutable;
using System.Runtime.Serialization;
using Core.Actors.Implementations;
using Guilds.Shared;

namespace Guilds.DataStore.Actors.Implementations;

[DataContract]
public record GuildEventStoreState([property: DataMember] int Version, [property: DataMember] ImmutableList<GuildCommand> Events)
{
    public static GuildEventStoreState Empty => new(0, ImmutableList<GuildCommand>.Empty);
}