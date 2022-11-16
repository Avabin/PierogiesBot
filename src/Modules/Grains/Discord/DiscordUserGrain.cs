using Core;
using GrainInterfaces.Discord;
using Grains.Core;

#pragma warning disable CS1998

namespace Grains.Discord;

[ImplicitStreamSubscription(StreamNamespaces.DiscordUser)]
public class DiscordUserGrain : EventingJournaledGrain<DiscordUserState, DiscordUserEvent>, IDiscordUserGrain
{
    public async Task<string> GetUsernameAsync()
    {
        return State.Username;
    }

    public async Task<int> GetDiscriminatorAsync()
    {
        return State.Discriminator;
    }
}