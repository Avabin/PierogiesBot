using Discord.Interactions;
using GrainInterfaces.Discord.Users;

namespace Discord.Commands;

public abstract class GrainedInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected GrainedInteractionModuleBase(IClusterClient clusterClient)
    {
        Client = clusterClient;
    }

    protected IClusterClient Client { get; }

    protected override async Task RespondAsync(string? text = null, Embed[]? embeds = null, bool isTTS = false,
                                               bool ephemeral = false,
                                               AllowedMentions? allowedMentions = null, RequestOptions? options = null,
                                               MessageComponent? components = null, Embed? embed = null)
    {
        var userGrain            = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.RespondAsync(text, embeds, isTTS, shouldReplyEphemeral ?? ephemeral, allowedMentions, options,
                                components, embed);
    }

    protected override async Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        var userGrain            = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.DeferAsync(shouldReplyEphemeral ?? ephemeral, options);
    }

    protected override async Task RespondWithFileAsync(Stream           fileStream, string fileName, string text = null,
                                                       Embed[]          embeds          = null,
                                                       bool             isTTS           = false, bool ephemeral = false,
                                                       AllowedMentions  allowedMentions = null,
                                                       MessageComponent components      = null, Embed embed = null,
                                                       RequestOptions   options         = null)
    {
        var userGrain            = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.RespondWithFileAsync(fileStream, fileName, text, embeds, isTTS, shouldReplyEphemeral ?? ephemeral,
                                        allowedMentions, components, embed, options);
    }
}