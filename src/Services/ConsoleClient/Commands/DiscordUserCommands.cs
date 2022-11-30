using System.Reactive.Linq;
using System.Reactive.Subjects;
using GrainInterfaces;
using GrainInterfaces.Discord.Users;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace ConsoleClient.Commands;

[Command("user", "User operations")]
public class DiscordUserCommands : ConsoleAppBase
{
    [Command("change")]
    public class DiscordChangeUserInfoCommands : ConsoleAppBase
    {
        private readonly IClusterClient _clusterClient;

        public DiscordChangeUserInfoCommands(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [Command("username", "Change username")]
        public async Task ChangeUsernameAsync([Option(0, "User Snowflake ID")] ulong  userId,
                                              [Option(1, "New username")]      string username)
        {
            var @event  = new ChangeUserUsername(username);
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
                var result = await subject.OfType<ChangeUserUsername>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Console.WriteLine($"Username changed to {result.Username}");
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
                Console.WriteLine("Timeout of 5 seconds reached");
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
            var @event  = new ChangeUserDiscriminator(discriminator);
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
                var result = await subject.OfType<ChangeUserDiscriminator>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Context.Logger.LogWarning("Discriminator changed to {Discriminator}", result.Discriminator);
                Console.WriteLine($"Discriminator changed to {result.Discriminator}");
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
                Console.WriteLine("Timeout of 5 seconds reached");
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
            var @event  = new ChangeUserAvatar(avatar);
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
                var result = await subject.OfType<ChangeUserAvatar>().Take(1).Timeout(TimeSpan.FromSeconds(5));
                Context.Logger.LogWarning("Avatar changed to {Avatar}", result.Avatar);
                Console.WriteLine($"Avatar changed to {result.Avatar}");
            }
            catch (TimeoutException)
            {
                Context.Logger.LogWarning("Timeout of 5 seconds reached");
                Console.WriteLine("Timeout of 5 seconds reached");
            }
            finally
            {
                await sub.UnsubscribeAsync();
            }
        }

        private IAsyncStream<DiscordUserEvent> Stream(ulong userId)
        {
            return _clusterClient.GetDiscordUserStream(userId);
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
            var grain    = _clusterClient.GetDiscordUserGrain(userId);
            var username = await grain.GetUsernameAsync();

            Console.WriteLine($"Username: {username}");
        }
    }
}