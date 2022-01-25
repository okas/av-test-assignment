using AutoFixture.Xunit2;
using Backend.WebApi.Domain.Exceptions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Backend.WebApi.Tests.Domain.Exceptions;

public class BaseExceptionTests
{
    [Theory]
    [AutoMoqData]
    public void Initialized_WithAllKnownParameters_Succeeds(
        // Arrange
        [Frozen] string? message,
        [Frozen] object model,
        [Frozen] string category,
        // Act
        BaseException sutEx)
    {
        //Assert
        sutEx.Should().BeCorrectlyInitializedViaConstructor(message, model, category);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Initialized_TryInitInvalidCategoryValue_ThrowsArgumentOutOfRange(
        // Arrange
        string category)
    {
        // Act
        Func<BaseException> act = Invoking(
            () => new NotFoundException() { Category = category! });

        // Assert
        act.Should().ThrowOnBadCategory();
    }
}
