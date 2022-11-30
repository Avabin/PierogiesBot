using GrainInterfaces.Discord.Users;

namespace Grains.Discord.Users;

#pragma warning disable CS1998
[Immutable]
[GenerateSerializer]
public class DiscordUserState
{
    [Id(0)] public ulong  Id            { get; set; }
    [Id(1)] public string Username      { get; set; } = "";
    [Id(2)] public int    Discriminator { get; set; }
    [Id(3)] public string Avatar        { get; set; } = "";
    [Id(4)] public bool   Bot           { get; set; }
    [Id(5)] public bool   System        { get; set; }

    [Id(6)] public bool? UseEphemeralResponses { get; set; } = null;

    public void Apply(ChangeUserUsername @event)
    {
        Username = @event.Username;
    }

    public void Apply(ChangeUserDiscriminator @event)
    {
        Discriminator = @event.Discriminator;
    }

    public void Apply(ChangeUserAvatar @event)
    {
        Avatar = @event.Avatar;
    }

    public void Apply(SetUseEphemeralResponses @event)
    {
        UseEphemeralResponses = @event.UseEphemeralResponses;
    }
}
#pragma warning restore CS1998