using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core;
using GrainInterfaces.Discord;
using Grains.Discord;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace ConsoleClient.Commands;

[Command("user", "User operations")]
public class DiscordUserCommands : ConsoleAppBase
{
    [Command("change")]
    public class DiscordChangeUserInfoCommands : ConsoleAppBase
    {
        private readonly Lazy<IStreamProvider> _streamProvider;

        public DiscordChangeUserInfoCommands(IClusterClient clusterClient)
        {
            _streamProvider =
                new Lazy<IStreamProvider>(() => clusterClient.GetStreamProvider(StreamProviders.RabbitMQ));
        }

        private IStreamProvider StreamProvider => _streamProvider.Value;

        [Command("username", "Change username")]
        public async Task ChangeUsernameAsync([Option(0, "User Snowflake ID")] ulong  userId,
                                              [Option(1, "New username")]      string username)
        {
            var @event  = new ChangeUsername(username);
            var stream  = Stream(userId);
            var subject = new Subject<DiscordUserEvent>();
            var sub = await stream.SubscribeAsync((userEvent, token) =>
            {
                subject.OnNext(userEvent);
                return Task.CompletedTask;
            });

            await stream.OnNextAsync(@event);

            try
            {
                var result = await subject.OfType<ChangeUsername>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Console.WriteLine($"Username changed to {result.Username}");
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
            }
            finally
            {
                await sub.UnsubscribeAsync();
            }
        }

        [Command("discriminator", "Change user discriminator")]
        public async Task ChangeDiscriminatorAsync([Option(0, "User Snowflake ID")] ulong userId,
                                                   [Option(1, "New discriminator")] int   discriminator)
        {
            var @event  = new ChangeDiscriminator(discriminator);
            var stream  = Stream(userId);
            var subject = new Subject<DiscordUserEvent>();
            var sub = await stream.SubscribeAsync((userEvent, token) =>
            {
                subject.OnNext(userEvent);
                return Task.CompletedTask;
            });

            await stream.OnNextAsync(@event);

            try
            {
                var result = await subject.OfType<ChangeDiscriminator>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Context.Logger.LogWarning("Discriminator changed to {Discriminator}", result.Discriminator);
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
            }
            finally
            {
                await sub.UnsubscribeAsync();
            }
        }

        [Command("avatar", "Change user avatar")]
        public async Task ChangeAvatarAsync([Option(0, "User Snowflake ID")] ulong  userId,
                                            [Option(1, "New avatar")]        string avatar)
        {
            var @event  = new ChangeAvatar(avatar);
            var stream  = Stream(userId);
            var subject = new Subject<DiscordUserEvent>();
            var sub = await stream.SubscribeAsync((userEvent, token) =>
            {
                subject.OnNext(userEvent);
                return Task.CompletedTask;
            });

            await stream.OnNextAsync(@event);

            try
            {
                var result = await subject.OfType<ChangeAvatar>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Context.Logger.LogWarning("Avatar changed to {Avatar}", result.Avatar);
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
            }
            finally
            {
                await sub.UnsubscribeAsync();
            }
        }

        private IAsyncStream<DiscordUserEvent> Stream(ulong userId)
        {
            return StreamProvider.GetStream<DiscordUserEvent>(StreamNamespaces.DiscordUser, userId.ToString());
        }
    }

    [Command("get")]
    public class DiscordGetUserInfoCommands : ConsoleAppBase
    {
        private readonly IClusterClient _clusterClient;

        public DiscordGetUserInfoCommands(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [Command("username", "Get username")]
        public async Task GetUsernameAsync([Option(0, "User Snowflake ID")] ulong userId)
        {
            var grain    = _clusterClient.GetGrain<IDiscordUserGrain>(userId.ToString());
            var username = await grain.GetUsernameAsync();

            Context.Logger.LogWarning($"Username: {username}");
        }
    }
}