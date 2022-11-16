namespace Grains.Discord;

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

    public void Apply(ChangeUsername @event)
    {
        Username = @event.Username;
    }

    public void Apply(ChangeDiscriminator @event)
    {
        Discriminator = @event.Discriminator;
    }

    public void Apply(ChangeAvatar @event)
    {
        Avatar = @event.Avatar;
    }
}
#pragma warning restore CS1998