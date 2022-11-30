using Core;
using GrainInterfaces.Discord.Users;
using Grains.Core;

#pragma warning disable CS1998

namespace Grains.Discord.Users;

public class DiscordUserGrain : EventEmitterJournaledGrain<DiscordUserState, DiscordUserEvent>, IDiscordUserGrain
{
    private ulong _userId;

    public Task<string> GetUsernameAsync()
    {
        return Task.FromResult(State.Username);
    }

    public Task<int> GetDiscriminatorAsync()
    {
        return Task.FromResult(State.Discriminator);
    }

    public Task<bool?> ShouldUseEphemeralResponsesAsync()
    {
        return Task.FromResult(State.UseEphemeralResponses);
    }

    public async Task RaiseAsync(DiscordUserEvent @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
        await NotifyAsync(StreamNamespaces.ForDiscordUser(_userId), this.GetPrimaryKeyString(), @event);
    }

    public Task<string> GetAvatarAsync()
    {
        return Task.FromResult(State.Avatar);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var idString = this.GetPrimaryKeyString();
        if (!ulong.TryParse(idString, out _userId)) throw new ArgumentException($"Invalid Discord user id: {idString}");
        await base.OnActivateAsync(cancellationToken);
    }
}