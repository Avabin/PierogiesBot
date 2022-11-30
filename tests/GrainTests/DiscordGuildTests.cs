using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord;
using FluentAssertions;
using GrainInterfaces;
using GrainInterfaces.Discord;
using NSubstitute;
using Orleans.Streams;

namespace GrainTests;

[Category("Unit")]
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class DiscordGuildTests : OrleansTestBase<UnitTestsSiloConfigurator, UnitTestsClientConfigurator>
{
    [Test]
    public async Task DiscordGuildsGrain_When_GetGuildsAsync_Returns_FoundGuilds()
    {
        // Arrange
        var discordService = GetService<IDiscordService>();
        discordService.StartAsync().Returns(Task.CompletedTask);
        var expectedName = "TestGuild";
        var expectedId   = NextUlong();
        discordService.GetGuildsAsync().Returns(new List<(ulong, string)> { (expectedId, expectedName) });
        var grain = Client.GetDiscordGuildsGrain();
        
        // Act
        var guilds = await grain.GetGuildsAsync();
        var actual = guilds.FirstOrDefault();
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Name.Should().Be(expectedName);
        actual.Id.Should().Be(expectedId);

    }

    [Test]
    public async Task DiscordGuildGrain_When_ChangeNameEvent_NameChanged_NotificationSent()
    {
        // Arrange
        var expectedName = "TestGuild";
        var guildId      = NextUlong();
        var grain        = Client.GetDiscordGuildGrain(guildId);
        var task         = Client.GetDiscordGuildStream(guildId).ReceiveAsync();
        
        // Act
        var command = new ChangeGuildName(expectedName);
        await grain.RaiseAsync(command);
        var @event = await task;
        var actual = await grain.GetNameAsync();
        
        // Assert
        actual.Should().Be(expectedName);
        @event.Should().BeOfType<ChangeGuildName>();
    }
    
    [Test]
    public async Task DiscordGuildGrain_When_SetTimezoneEvent_TimezoneSet_NotificationSent()
    {
        // Arrange
        var expectedTimezone = "Europe/London";
        var guildId          = NextUlong();
        var grain            = Client.GetDiscordGuildGrain(guildId);
        var task             = Client.GetDiscordGuildStream(guildId).ReceiveAsync();
        
        // Act
        var command = new SetGuildTimezone(expectedTimezone);
        await grain.RaiseAsync(command);
        var @event = await task;
        var actual = await grain.GetTimezoneAsync();
        
        // Assert
        actual.Should().Be(expectedTimezone);
        @event.Should().BeOfType<SetGuildTimezone>();
    }
}