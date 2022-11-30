namespace GrainInterfaces.Discord;

public interface IDiscordMessagesWatcherGrain : IGrainWithStringKey
{
    public const string Id = "DiscordMessagesWatcher";

    Task StartWatchingAsync();

    Task StopWatchingAsync();
}