using AutoFixture.Xunit2;
using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.Domain.Exceptions;

public class AlreadyExistsExceptionTests
{
    [Theory]
    [AutoData]
    public void Initialized_WithKnownParameters_ThrowsCorrectInstance(
         // Arrange
         [Frozen(Matching.ParameterName)] string? message,
         [Frozen(Matching.ParameterName)] string key,
         [Frozen] object value,
         // Act
         AlreadyExistsException sutEx)
    {
        //Assert
        using AssertionScope _ = new();
        sutEx.Message.Should().Be(message);
        sutEx.Data[key].Should().Be(value);
    }
}
