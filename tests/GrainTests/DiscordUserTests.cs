using FluentAssertions;
using GrainInterfaces;
using GrainInterfaces.Discord.Users;

namespace GrainTests;

[Category("Unit")]
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
public class DiscordUserTests : OrleansTestBase<UnitTestsSiloConfigurator, UnitTestsClientConfigurator>
{
   // when ChangeUserUsername command is sent, username is changed
   [Test]
   public async Task When_ChangeNameCommand_Then_UsernameChanged_NotificationSent()
   {
      // Arrange
      var userId      = NextUlong();
      var discordUser = Client.GetDiscordUserGrain(userId);
      var stream      = Client.GetDiscordUserStream(userId);
      var task        = stream.ReceiveAsync();
      
      var expected = "username";
      var command  = new ChangeUserUsername(expected);
      
      // Act
      await discordUser.RaiseAsync(command);
      var @event = await task;
      var actual = await discordUser.GetUsernameAsync();
      
      // Assert
      actual.Should().Be(expected);

      @event.Should().BeOfType<ChangeUserUsername>();
   }
   
   // when ChangeUserAvatar command is sent, avatar url is changed
   [Test]
   public async Task When_ChangeAvatarCommand_Then_AvatarUrlChanged_NotificationSent()
   {
      // Arrange
      var userId      = NextUlong();
      var discordUser = Client.GetDiscordUserGrain(userId);
      var stream      = Client.GetDiscordUserStream(userId);
      var task        = stream.ReceiveAsync();
      
      var expected = "avatar url";
      var command  = new ChangeUserAvatar(expected);
      
      // Act
      await discordUser.RaiseAsync(command);
      var @event = await task;
      var actual = await discordUser.GetAvatarAsync();
      
      // Assert
      actual.Should().Be(expected);

      @event.Should().BeOfType<ChangeUserAvatar>();
   }
   
   // when ChangeUserDiscriminator command is sent, discriminator is changed
   [Test]
   public async Task When_ChangeDiscriminatorCommand_Then_DiscriminatorChanged_NotificationSent()
   {
      // Arrange
      var userId      = NextUlong();
      var discordUser = Client.GetDiscordUserGrain(userId);
      var stream      = Client.GetDiscordUserStream(userId);
      var task        = stream.ReceiveAsync();
      
      var expected = 1234;
      var command  = new ChangeUserDiscriminator(expected);
      
      // Act
      await discordUser.RaiseAsync(command);
      var @event = await task;
      var actual = await discordUser.GetDiscriminatorAsync();
      
      // Assert
      actual.Should().Be(expected);

      @event.Should().BeOfType<ChangeUserDiscriminator>();
   }
   
   // when SetEphemeralResponse command is sent, ephemeral response is set
   [Test]
   public async Task When_SetEphemeralResponses_Then_EphemeralResponseSet_NotificationSent()
   {
      // Arrange
      var userId      = NextUlong();
      var discordUser = Client.GetDiscordUserGrain(userId);
      var stream      = Client.GetDiscordUserStream(userId);
      var task        = stream.ReceiveAsync();

      var expected = true;
      var command  = new SetUseEphemeralResponses(expected);
      
      // Act
      await discordUser.RaiseAsync(command);
      var @event = await task;
      var actual = await discordUser.ShouldUseEphemeralResponsesAsync();
      
      // Assert
      actual!.Value.Should().Be(expected);

      @event.Should().BeOfType<SetUseEphemeralResponses>();
   }
}