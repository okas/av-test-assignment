using AutoFixture.Xunit2;
using Backend.WebApi.App.Extensions;
using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Extensions;

[Collection("ActionFilterFixture")]
public class CUDOperationsExceptionFilterTests
{
    private readonly ExceptionContext _exceptionContext;

    public CUDOperationsExceptionFilterTests(ActionFilterFixture fixture)
    {
        _exceptionContext = fixture.CreateExceptionContext();
    }

    //[Fact]
    [Theory]
    [AutoMoqData]
    public void OnException_NotFoundException_ShouldSetConflictObjectResultWithCorrectValueObject(
        // Arrange
        [Frozen] string? message,
        [Frozen] object model,
        [Greedy] NotFoundException notFoundEx,
        CUDOperationsExceptionFilter sutExceptionFilter)
    {
        ProblemDetails expectedDetails = new()
        {
            Title = "Not found.",
            Detail = message,
        };
        expectedDetails.Extensions[BaseException.ModelDataKey] = model;

        _exceptionContext.Exception = notFoundEx;

        // Act
        sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();

        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>()
            .And.BeEquivalentTo(expectedDetails);

    }

    [Theory]
    [AutoMoqData]
    public void OnException_AlreadyExistsException_ShouldSetConflictObjectResultWithCorrectValueObject(
        // Arrange
        [Frozen] string? message,
        [Frozen] object model,
        [Greedy] AlreadyExistsException alreadyExistsEx,
        CUDOperationsExceptionFilter _sutExceptionFilter)
    {
        ProblemDetails expectedDetails = new()
        {
            Title = "Already exists.",
            Detail = message,
        };
        expectedDetails.Extensions[BaseException.ModelDataKey] = model;

        _exceptionContext.Exception = alreadyExistsEx;

        // Act
        _sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().BeOfType<ConflictObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>()
            .And.BeEquivalentTo(expectedDetails);
    }
}
