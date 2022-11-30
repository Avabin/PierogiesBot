using Discord.Interactions;
using GrainInterfaces;

namespace Discord.Commands.Wow;

public class WowInteractionModule : GrainedInteractionModuleBase
{
    public WowInteractionModule(IClusterClient clusterClient) : base(clusterClient)
    {
    }

    [SlashCommand("wow-fetch", "Fetches a WoW character")]
    public async Task FetchCharacterAsync(string server, string realm, string name)
    {
        var character = await Client.GetWowCharacterGrain(server, realm, name).GetViewAsync();

        await RespondAsync(embed: character.ToEmbed().Build());
    }
}