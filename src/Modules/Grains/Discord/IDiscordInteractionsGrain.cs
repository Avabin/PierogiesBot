using Discord;
using GrainInterfaces.Discord;

namespace Grains.Discord;

public class DiscordInteractionsGrain : IDiscordInteractionsGrain
{
    private readonly IDiscordService _discordService;

    public DiscordInteractionsGrain(IDiscordService discordService)
    {
        _discordService = discordService;
    }

    public Task InstallInteractionsAsync()
    {
        return _discordService.InstallInteractionsAsync();
    }
}