using Core;
using Discord;
using GrainInterfaces.Discord.Guilds.Events;
using GrainInterfaces.Discord.Guilds.Grains;
using GrainInterfaces.Discord.Guilds.MessageTriggers;
using Grains.Core;
using Microsoft.Extensions.Logging;

namespace Grains.Discord.Guilds;

[ImplicitStreamSubscription(StreamNamespaces.RuleHits)]
public class GuildRuleExecutorGrain : EventingGrain<ExecuteTriggers>, IGuildRuleExecutorGrain
{
    private readonly IDiscordService                 _discordService;
    private readonly ILogger<GuildRuleExecutorGrain> _logger;

    public GuildRuleExecutorGrain(IDiscordService discordService, ILogger<GuildRuleExecutorGrain> logger)
    {
        _discordService = discordService;
        _logger         = logger;
    }

    protected override async Task RaiseAsync(ExecuteTriggers @event)
    {
        var channelId = @event.ChannelId;
        var messageId = @event.MessageId;
        await Parallel.ForEachAsync(@event.Triggers,
                                    async (t, ct) => { await HandleTriggerAsync(t, channelId, messageId); });
    }

    private async Task HandleTriggerAsync(MessageTrigger t, ulong channelId, ulong messageId)
    {
        try
        {
            switch (t)
            {
                case ResponseMessageTrigger responseMessageTrigger:
                    await _discordService.SendMessageAsync(channelId, responseMessageTrigger.Response);
                    break;
                case ReactionMessageTrigger reactionMessageTrigger:
                    await _discordService.AddReactionAsync(channelId, messageId, reactionMessageTrigger.Emoji);
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to execute trigger {Trigger}", t);
        }
    }
}