// ReSharper disable once CheckNamespace

namespace GrainInterfaces.Discord;

public interface IDiscordGuildGrain : IGrainWithStringKey
{
    Task<string> GetNameAsync();
}

[Immutable] [GenerateSerializer] public record DiscordGuildEvent();

[Immutable] [GenerateSerializer] public record ChangeGuildName([property: Id(0)] string Name) : DiscordGuildEvent;

public record ChangeTimezone([property: Id(0)] string Timezone) : DiscordGuildEvent;