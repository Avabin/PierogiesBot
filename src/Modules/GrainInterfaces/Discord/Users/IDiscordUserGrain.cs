namespace GrainInterfaces.Discord.Users;

public interface IDiscordUserGrain : IGrainWithStringKey
{
    Task<string> GetUsernameAsync();
    Task<int>    GetDiscriminatorAsync();
    Task<bool?>  ShouldUseEphemeralResponsesAsync();

    Task         RaiseAsync(DiscordUserEvent @event);
    Task<string> GetAvatarAsync();
}