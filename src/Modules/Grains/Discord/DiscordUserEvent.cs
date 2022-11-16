namespace Grains.Discord;

#pragma warning disable CS1998
[Immutable] [GenerateSerializer] public record DiscordUserEvent();

[Immutable] [GenerateSerializer] public record ChangeUsername([property: Id(0)] string Username) : DiscordUserEvent();

[Immutable] [GenerateSerializer] public record ChangeAvatar([property: Id(0)] string Avatar) : DiscordUserEvent();

[Immutable]
[GenerateSerializer]
public record ChangeDiscriminator([property: Id(0)] int Discriminator) : DiscordUserEvent();

#pragma warning restore CS1998