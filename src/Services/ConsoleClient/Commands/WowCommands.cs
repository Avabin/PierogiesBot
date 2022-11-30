using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace ConsoleClient.Commands;

[Command("wow", "World of Warcraft commands")]
public class WowCommands : ConsoleAppBase
{
    [Command("character", "Character commands")]
    public class CharacterCommands : ConsoleAppBase
    {
        private readonly IClusterClient _clusterClient;

        public CharacterCommands(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [Command("get", "Get character data")]
        public async Task GetCharacterAsync([Option(0, "World of Warcraft server name")] string server,
                                            [Option(1, "Realm name")]                    string realm,
                                            [Option(2, "Character name")]                string name)
        {
            var grain = _clusterClient.GetWowCharacterGrain(server, realm, name);

            var character = await grain.GetViewAsync();

            Context.Logger.LogWarning("Character: {@Character}", character);
        }
    }
}