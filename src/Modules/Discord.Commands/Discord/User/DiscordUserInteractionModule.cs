using Discord.Interactions;
using GrainInterfaces;
using GrainInterfaces.Discord.Users;

namespace Discord.Commands.Discord.User;

public class DiscordUserInteractionModule : GrainedInteractionModuleBase
{
    public DiscordUserInteractionModule(IClusterClient clusterClient) : base(clusterClient)
    {
    }

    [SlashCommand("set_ephemeral", "If true, all bot responses to your commands will be ephemeral")]
    public async Task SetEphemeral(bool value)
    {
        var stream = Client.GetDiscordUserStream(Context.User.Id);

        var command = new SetUseEphemeralResponses(value);

        await stream.OnNextAsync(command);

        await RespondAsync($"Ephemeral responses set to {value}");
    }
}