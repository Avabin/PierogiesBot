// ReSharper disable once CheckNamespace

using Discord.Shared;

namespace GrainInterfaces.Discord;

public interface IDiscordGuildGrain : IGrainWithStringKey
{
    Task<string>           GetNameAsync();
    Task<DiscordGuildView> GetViewAsync();
    Task<string>           GetTimezoneAsync();

    Task RaiseAsync(DiscordGuildEvent @event);
}

[Immutable] [GenerateSerializer] public record DiscordGuildEvent();

[Immutable] [GenerateSerializer] public record ChangeGuildName([property: Id(0)] string Name) : DiscordGuildEvent;

[Immutable] [GenerateSerializer] public record SetGuildTimezone([property: Id(0)] string Timezone) : DiscordGuildEvent;