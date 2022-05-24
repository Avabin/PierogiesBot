using Core;

namespace Guilds.DataStore.Actors.Implementations;

public record GuildState(ulong Id, string Name) : Event
{
    public static GuildState Empty => new(0, string.Empty);
}