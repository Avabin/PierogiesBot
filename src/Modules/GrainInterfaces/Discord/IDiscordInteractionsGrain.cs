namespace GrainInterfaces.Discord;

public interface IDiscordInteractionsGrain : IGrainWithStringKey
{
    public const string Key = "DiscordInteractions";
    Task InstallInteractionsAsync();
}