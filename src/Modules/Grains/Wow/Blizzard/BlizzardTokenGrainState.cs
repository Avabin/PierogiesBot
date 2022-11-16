using Wow.Blizzard.Client;

namespace Grains.Wow.Blizzard;

[GenerateSerializer]
public class BlizzardTokenGrainState
{
    [Id(0)] public string         Token     { get; set; } = string.Empty;
    [Id(1)] public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
    [Id(2)] public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.MinValue;
    [Id(3)] public string         TokenType { get; set; } = string.Empty;

    public void Update(TokenResponse token)
    {
        Token     = token.AccessToken;
        CreatedAt = token.CreatedAt;
        ExpiresAt = token.CreatedAt.AddSeconds(token.ExpiresIn);
        TokenType = token.TokenType;
    }
}