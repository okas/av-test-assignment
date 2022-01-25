using Backend.WebApi.Domain.Exceptions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Backend.WebApi.Tests.Domain.Exceptions;

public class AlreadyExistsExceptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Initialized_TryInitInvalidCategoryValue_ThrowsArgumentOutOfRange(
    // Arrange
    string? testValue)
    {
        // Act
        Func<AlreadyExistsException> act = Invoking(
            () => new AlreadyExistsException() { Category = testValue! });

        // Assert
        act.Should().ThrowOnBadCategory();
    }
}
