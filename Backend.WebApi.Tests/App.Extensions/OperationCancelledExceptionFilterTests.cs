using Backend.WebApi.App.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Extensions;

[Collection("ActionFilterFixture")]
public class OperationCancelledExceptionFilterTests
{
    private readonly ExceptionContext _exceptionContext;

    public OperationCancelledExceptionFilterTests(ActionFilterFixture fixture)
    {
        _exceptionContext = fixture.CreateExceptionContext();
    }

    [Theory]
    [AutoMoqData]
    public void OnException_TokenIsCancelled_ShouldReturnStatusCodeResultWith499(
         // Arrange
         OperationCancelledExceptionFilter sutExceptionFilter)
    {

        int expectedStatusCode = 499;
        CancellationToken cancelledToken = new(canceled: true);

        _exceptionContext.Exception = new OperationCanceledException(cancelledToken);

        // Act
        sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(expectedStatusCode);

    }

    [Theory]
    [AutoMoqData]
    public void OnException_TokenIsNotCancelled_ShouldReturnNullResultAndExceptionNotHandeled(
         // Arrange
         OperationCancelledExceptionFilter sutExceptionFilter)
    {
        CancellationToken notCancelledToken = new(canceled: false);

        _exceptionContext.Exception = new OperationCanceledException(notCancelledToken);

        // Act
        sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeFalse();

        _exceptionContext.Result.Should().BeNull();
    }
}
