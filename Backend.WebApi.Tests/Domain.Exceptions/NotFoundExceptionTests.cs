using System;
using AutoFixture.Xunit2;
using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.Domain.Exceptions;

public class NotFoundExceptionTests
{
    [Theory]
    [AutoData]
    public void Initialized_WithKnownParameters_ThrowsCorrectInstance(
        // Arrange
        [Frozen] string? message,
        [Frozen(Matching.ParameterName)] Guid id,
        // Act
        NotFoundException sutEx)
    {
        //Assert
        using AssertionScope _ = new();
        sutEx.Message.Should().Be(message);
        sutEx.Data["Id"].Should().Be(id);
    }
}
