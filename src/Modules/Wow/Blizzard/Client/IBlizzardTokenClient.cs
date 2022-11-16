namespace Wow.Blizzard.Client;

public interface IBlizzardTokenClient
{
    Task<TokenResponse> GetTokenAsync(string clientId, string clientSecret);
}