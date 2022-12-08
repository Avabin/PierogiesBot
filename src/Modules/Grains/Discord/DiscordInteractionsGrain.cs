using Discord;
using GrainInterfaces.Discord;

namespace Grains.Discord;

public class DiscordInteractionsGrain : Grain, IDiscordInteractionsGrain
{
    private readonly IDiscordService _discordService;

    public DiscordInteractionsGrain(IDiscordService discordService)
    {
        _discordService = discordService;
    }

    public async Task InstallInteractionsAsync()
    {
        await _discordService.InstallInteractionsAsync();
    }
}