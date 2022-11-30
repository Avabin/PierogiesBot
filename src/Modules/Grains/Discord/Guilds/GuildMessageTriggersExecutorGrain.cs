using Core;
using Discord;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds;
using GrainInterfaces.Discord.Guilds.Events;
using GrainInterfaces.Discord.Guilds.MessageTriggers;
using Grains.Core;
using Microsoft.Extensions.Logging;

namespace Grains.Discord.Guilds;

[RegexImplicitStreamSubscription(StreamNamespaces.Triggers)]
public class GuildMessageTriggersExecutorGrain : EventSubscriberGrain<TriggerEvent>, IGuildMessageTriggersExecutorGrain
{
    private readonly IDiscordService                            _discordService;
    private readonly ILogger<GuildMessageTriggersExecutorGrain> _logger;
    private          ulong                                      _guildId;

    public GuildMessageTriggersExecutorGrain(IDiscordService                            discordService,
                                             ILogger<GuildMessageTriggersExecutorGrain> logger)
    {
        _discordService = discordService;
        _logger         = logger;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var idString = this.GetPrimaryKeyString();
        _guildId = ulong.Parse(idString);
        await base.OnActivateAsync(cancellationToken);
    }

    protected override async Task RaiseAsync(TriggerEvent @event)
    {
        if (@event is not ExecuteTriggers(var triggers, var channelId, var messageId)) return;

        await Parallel.ForEachAsync(triggers, async (t, ct) => await HandleTriggerAsync(t, channelId, messageId));

        await this.GetTriggersStream(_guildId)
                  .OnNextAsync(new TriggersExecuted(triggers.Select(x => x.Name).ToList(), _guildId, channelId,
                                                    messageId));
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
            _logger.LogError(e, "Failed to execute trigger {@Trigger}", t);
        }
    }
}