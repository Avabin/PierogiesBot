using Core;
using GrainInterfaces.Discord;
using Grains.Core;
using TimeZoneConverter;

namespace Grains.Discord.Guilds;

[ImplicitStreamSubscription(StreamNamespaces.DiscordGuild)]
public class DiscordGuildGrain : EventingJournaledGrain<DiscordGuildState, DiscordGuildEvent>, IDiscordGuildGrain
{
    public Task<string> GetNameAsync()
    {
        return Task.FromResult(State.Name);
    }
}

[GenerateSerializer]
public class DiscordGuildState
{
    [Id(0)] public ulong  Id   { get; set; } = 0;
    [Id(1)] public string Name { get; set; } = string.Empty;

    public string Timezone { get; set; } = TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);

    public void Apply(ChangeGuildName command)
    {
        Name = command.Name;
    }

    public void Apply(ChangeTimezone command)
    {
        Timezone = command.Timezone;
    }
}