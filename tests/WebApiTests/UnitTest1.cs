using FluentAssertions;
using Newtonsoft.Json;
using RestEase;
using Shared;
using Shared.MessageTriggers;

namespace WebApiTests;

public class Tests
{
    private IPierogiesBotApi _pierogiesBotApi;

    [SetUp]
    public void Setup()
    {
        var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";
        _pierogiesBotApi = new RestClient(apiUrl)
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            }
        }.For<IPierogiesBotApi>();
    }

    [Test]
    public async Task When_AddMessageTrigger_Expect_MessageTriggerAdded()
    {
        // Arrange
        var expected = new SimpleResponseMessageTrigger("test", "test", "test");
        var guildId = 1ul;

        // Act
        await _pierogiesBotApi.AddMessageTriggerAsync(guildId, expected);
        var actual = await _pierogiesBotApi.GetMessageTriggerAsync(guildId, expected.Name);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}