namespace GrainInterfaces.Discord.Users;

#pragma warning disable CS1998
[Immutable] [GenerateSerializer] public record DiscordUserEvent();

[Immutable]
[GenerateSerializer]
public record ChangeUserUsername([property: Id(0)] string Username) : DiscordUserEvent();

[Immutable] [GenerateSerializer] public record ChangeUserAvatar([property: Id(0)] string Avatar) : DiscordUserEvent();

[Immutable]
[GenerateSerializer]
public record ChangeUserDiscriminator([property: Id(0)] int Discriminator) : DiscordUserEvent();

// Sets all bot responses to the user as ephemeral if true
[Immutable]
[GenerateSerializer]
public record SetUseEphemeralResponses([property: Id(0)] bool UseEphemeralResponses) : DiscordUserEvent();

#pragma warning restore CS1998