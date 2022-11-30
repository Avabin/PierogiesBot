using Discord;
using GrainInterfaces;
using GrainInterfaces.Discord.Guilds.Events;
using NSubstitute;
using Shared.MessageTriggers;

namespace GrainTests;

[Category("Unit")]
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class GuildMessageTriggersExecutorTests : OrleansTestBase<UnitTestsSiloConfigurator, UnitTestsClientConfigurator>
{
    [Test]
    public async Task When_ExecuteTriggers_ReactionMessageTrigger_Then_TriggerExecutes()
    {
        // Arrange
        var guildId = NextUlong();
        var channelId = NextUlong();
        var messageId = NextUlong();
        var stream = Client.GetTriggersStream(guildId);
        var discordService = GetService<IDiscordService>();

        ReactionMessageTrigger trigger = new SimpleReactionMessageTrigger("test", "test", "test");
        discordService.AddReactionAsync(Arg.Is(channelId), Arg.Is(messageId), Arg.Is(trigger.Response))
            .Returns(Task.CompletedTask);

        var command = new ExecuteTriggers(new List<MessageTrigger>() { trigger }, channelId, messageId);
        // Act
        await stream.OnNextAsync(command);
        var task = stream.ReceiveAsync<TriggerEvent, TriggersExecuted>();
        var c = await task;

        // Assert
        await discordService.Received(1)
            .AddReactionAsync(Arg.Is(channelId), Arg.Is(messageId), Arg.Is(trigger.Response));
    }
}