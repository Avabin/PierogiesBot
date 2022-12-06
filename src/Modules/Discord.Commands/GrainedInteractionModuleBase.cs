using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord.Interactions;
using GrainInterfaces.Discord.Users;

namespace Discord.Commands;

public abstract class GrainedInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    private IDisposable _subscription;

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
        if (Context.Interaction.HasResponded)
        {
            await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed);
            return;
        }

        var userGrain = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.RespondAsync(text, embeds, isTTS, shouldReplyEphemeral ?? ephemeral, allowedMentions, options,
            components, embed);
    }

    protected override async Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        var userGrain = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.DeferAsync(shouldReplyEphemeral ?? ephemeral, options);
    }

    protected override async Task RespondWithFileAsync(Stream fileStream, string fileName, string text = null,
        Embed[] embeds = null,
        bool isTTS = false, bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null, Embed embed = null,
        RequestOptions options = null)
    {
        if (Context.Interaction.HasResponded)
        {
            await FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions,
                components, embed, options);
            return;
        }

        var userGrain = Client.GetGrain<IDiscordUserGrain>(Context.User.Id.ToString("D"));
        var shouldReplyEphemeral = await userGrain.ShouldUseEphemeralResponsesAsync();
        await base.RespondWithFileAsync(fileStream, fileName, text, embeds, isTTS, shouldReplyEphemeral ?? ephemeral,
            allowedMentions, components, embed, options);
    }

    public override async Task BeforeExecuteAsync(ICommandInfo command)
    {
        var timer = Observable.Timer(TimeSpan.FromSeconds(2)).Select(x => DeferAsync().ToObservable()).Concat();

        _subscription = timer.Subscribe();
        await base.BeforeExecuteAsync(command);
    }

    public override Task AfterExecuteAsync(ICommandInfo command)
    {
        _subscription?.Dispose();
        return base.AfterExecuteAsync(command);
    }
}