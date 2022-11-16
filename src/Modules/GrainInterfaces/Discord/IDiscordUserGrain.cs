namespace GrainInterfaces.Discord;

public interface IDiscordUserGrain : IGrainWithStringKey
{
    Task<string> GetUsernameAsync();
    Task<int>    GetDiscriminatorAsync();
}