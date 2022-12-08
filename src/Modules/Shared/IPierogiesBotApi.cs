using RestEase;
using Shared.MessageTriggers;
using Shared.ScheduledMessages;

namespace Shared;

public interface IPierogiesBotApi
{
    [Get("guilds")]
    Task<List<BasicDiscordGuildInfo>> GetGuildsAsync();

    [Get("guilds/{guildId}/messageTriggers")]
    Task<List<IMessageTrigger>> GetMessageTriggersAsync([Path] ulong guildId);

    [Get("guilds/{guildId}/messageTriggers/{messageTriggerName}")]
    Task<IMessageTrigger> GetMessageTriggerAsync([Path] ulong guildId, [Path] string messageTriggerName);

    [Post("guilds/{guildId}/messageTriggers")]
    Task AddMessageTriggerAsync([Path] ulong guildId, [Body] IMessageTrigger messageTrigger);

    [Delete("guilds/{guildId}/messageTriggers/{messageTriggerName}")]
    Task DeleteMessageTriggerAsync([Path] ulong guildId, [Path] string messageTriggerName);

    [Get("guilds/{guildId}/scheduledMessages")]
    Task<List<IScheduledMessage>> GetScheduledMessagesAsync([Path] ulong guildId);

    [Post("guilds/{guildId}/scheduledMessages")]
    Task AddScheduledMessageAsync([Path] ulong guildId, [Body] IScheduledMessage scheduledMessage);

    [Delete("guilds/{guildId}/scheduledMessages/{scheduledMessageName}")]
    Task DeleteScheduledMessageAsync([Path] ulong guildId, [Path] string scheduledMessageName);
}