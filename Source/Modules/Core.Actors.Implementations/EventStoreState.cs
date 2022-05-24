using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Core.Actors.Implementations;

[DataContract]
public record EventStoreState([property: DataMember] ulong Version, [property: DataMember] ImmutableList<Event> Events)
{
    public static EventStoreState Empty => new(0, ImmutableList<Event>.Empty);
}