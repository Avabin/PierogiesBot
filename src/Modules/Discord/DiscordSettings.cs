namespace Discord;

public class DiscordSettings
{
    public bool         CommandsEnabled    { get; set; } = false;
    public string       Token              { get; set; } = "";
    public List<string> CommandsAssemblies { get; set; } = new();
}