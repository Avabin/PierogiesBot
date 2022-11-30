using System.Drawing;
using FluentAssertions;
using GrainInterfaces;
using RestEase;

namespace MathApiTests;

[TestFixture]
public class Tests
{
    [SetUp]
    public void Setup()
    {
        _mathApi = RestClient.For<IMathApi>("http://localhost:5000");
    }

    private IMathApi _mathApi;

    [Test]
    public async Task When_SimpleFormulaIsProvided_Then_ResultIsReturned()
    {
        // Arrange
        var formula = "$$ 1+1 $$";

        // Act
        var stream = await _mathApi.GetRenderAsync(formula);

        // read stream

        using var image = Image.FromStream(stream);

        // Assert
        image.Height.Should().NotBe(0);
        image.Width.Should().NotBe(0);
    }

    [Test]
    public async Task When_ComplexIntegralIsProvided_Then_ResultIsReturned()
    {
        // Arrange
        var formula = @"$$ \int_{-\infty}^{\infty} \hat f(\xi)\,e^{2 \pi i \xi x} \,d\xi $$";

        // Act
        var stream = await _mathApi.GetRenderAsync(formula);

        // read stream

        using var image = Image.FromStream(stream);

        // Assert
        image.Height.Should().NotBe(0);
        image.Width.Should().NotBe(0);
    }
}