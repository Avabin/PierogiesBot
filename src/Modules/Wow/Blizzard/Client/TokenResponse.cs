namespace Wow.Blizzard.Client;

public class TokenResponse
{
    public string         AccessToken { get; set; } = string.Empty;
    public string         TokenType   { get; set; } = string.Empty;
    public int            ExpiresIn   { get; set; }
    public string         Scope       { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt   { get; set; } = DateTimeOffset.MinValue;
}