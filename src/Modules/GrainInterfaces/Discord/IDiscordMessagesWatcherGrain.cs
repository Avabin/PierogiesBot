namespace GrainInterfaces.Discord;

public interface IDiscordMessagesWatcherGrain
{
    public const string Id = "DiscordMessagesWatcher";

    Task StartWatchingAsync();

    Task StopWatchingAsync();
}