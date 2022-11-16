using System.Collections.Immutable;
using System.Reflection;

namespace Discord;

public interface IDiscordService
{
    IObservable<IMessage>            MessagesObservable { get; }
    Task                             StartAsync();
    Task                             StopAsync();
    Task                             AddReactionAsync(ulong channelId, ulong messageId, string emoteName);
    Task<ulong>                      GetChannelGuildIdAsync(ulong channelId);
    Task<IReadOnlyCollection<ulong>> GetGuildsAsync();
    Task                             SendMessageAsync(ulong channelId, string message);
    Task                             SendEmoteAsync(ulong channelId, string emoteName);
    Task                             InstallInteractionsAsync(Assembly assembly);
    Task                             MuteUserAsync(ulong userId, ulong guildId, TimeSpan? duration = null);
    ImmutableList<ulong>             GetUserRoles(ulong guildId, ulong userId);
    Task                             UnmuteUserAsync(ulong userId, ulong guildId);
    Task                             BanUserAsync(ulong userId, ulong guildId);
    Task                             UnbanUserAsync(ulong userId, ulong guildId);
    Task                             KickUserAsync(ulong userId, ulong guildId);
    Task<ulong>                      CreateRoleAsync(ulong guildId, string name, byte r, byte g, byte b);
    Task                             DeleteRoleAsync(ulong guildId, ulong id);
    Task                             ArchiveThreadAsync(ulong guildId, ulong threadId);
}