using Core;
using Discord.Shared;
using GrainInterfaces.Discord;
using Grains.Core;
using TimeZoneConverter;

namespace Grains.Discord.Guilds;

public class DiscordGuildGrain : EventEmitterJournaledGrain<DiscordGuildState, DiscordGuildEvent>, IDiscordGuildGrain
{
    public Task<string> GetNameAsync()
    {
        return Task.FromResult(State.Name);
    }

    public Task<DiscordGuildView> GetViewAsync()
    {
        return Task.FromResult<DiscordGuildView>(new DiscordGuildView(State.Id, State.Name, State.Timezone));
    }

    public Task<string> GetTimezoneAsync()
    {
        return Task.FromResult(State.Timezone);
    }

    public async Task RaiseAsync(DiscordGuildEvent @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
        await NotifyAsync(StreamNamespaces.ForDiscordGuild(State.Id), this.GetPrimaryKeyString(), @event);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (State.Id == 0) State.Id = ulong.Parse(this.GetPrimaryKeyString());
        await base.OnActivateAsync(cancellationToken);
    }
}

[GenerateSerializer]
public class DiscordGuildState
{
    [Id(0)] public ulong  Id   { get; set; } = 0;
    [Id(1)] public string Name { get; set; } = string.Empty;

    [Id(2)] public string Timezone { get; set; } = TZConvert.GetTimeZoneInfo("UTC").Id;

    public void Apply(ChangeGuildName command)
    {
        Name = command.Name;
    }

    public void Apply(SetGuildTimezone command)
    {
        Timezone = command.Timezone;
    }
}