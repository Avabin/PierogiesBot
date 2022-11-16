namespace Discord;

public interface IDiscordMessagesWatcherService
{
    Task StartAsync();
    Task StopAsync();
}