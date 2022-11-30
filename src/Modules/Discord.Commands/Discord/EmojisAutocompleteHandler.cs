using Core;
using Discord.Interactions;
using Discord.Shared;

namespace Discord.Commands.Discord;

public class EmojisAutocompleteHandler : AutocompleteHandler
{
    private readonly IDiscordService               _discordService;
    private          AsyncLazy<List<DiscordEmoji>> _emojis;

    public EmojisAutocompleteHandler(IDiscordService discordService)
    {
        _discordService = discordService;

        _emojis = new AsyncLazy<List<DiscordEmoji>>(async () => await _discordService.GetEmojisAsync());
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,   IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo      parameter, IServiceProvider         services)
    {
        var                       filter = autocompleteInteraction.Data.Current.Value as string ?? "";
        IEnumerable<DiscordEmoji> emojis = await _emojis.Value;


        IEnumerable<AutocompleteResult> suggestions = emojis
                                                     .Where(x => x.Name.Contains(filter,
                                                                StringComparison.OrdinalIgnoreCase)).Take(25)
                                                     .Select(x => new AutocompleteResult(x.Name, x.Name)).ToList();


        if (Emoji.TryParse($":{filter}:", out var emoji))
            suggestions = suggestions.Prepend(new AutocompleteResult(emoji.Name, filter));

        var suggestionsList = suggestions.ToList();
        if (suggestionsList.Count != 0) return AutocompletionResult.FromSuccess(suggestionsList);

        return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "No emojis found");
    }
}