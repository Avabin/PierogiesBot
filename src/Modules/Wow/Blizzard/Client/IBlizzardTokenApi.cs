using Newtonsoft.Json;
using RestEase;

namespace Wow.Blizzard.Client;

[BaseAddress("https://oauth.battle.net")]
internal interface IBlizzardTokenApi
{
    [Header("Authorization")] string Authorization { get; set; }

    [Post("token")]
    [AllowAnyStatusCode]
    Task<Response<BlizzardAccessToken>> GetAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        TokenParameters parameters);
}

internal class TokenParameters : Dictionary<string, object>
{
    public TokenParameters()
    {
        Add("grant_type", "client_credentials");
    }

    [JsonProperty("grant_type")]
    public string GrantType
    {
        get => this["grant_type"] as string ?? string.Empty;
        set => this["grant_type"] = value;
    }
}

internal class BlizzardAccessToken
{
    [JsonProperty("access_token")] public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("token_type")] public string TokenType { get; set; } = string.Empty;

    [JsonProperty("expires_in")] public int ExpiresIn { get; set; } = 0;

    [JsonProperty("scope")] public string Scope { get; set; } = string.Empty;
}