using System;
using System.Threading;
using Backend.WebApi.App.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Extensions;

[Collection("ActionFilterFixture")]
public class OperationCancelledExceptionFilterAttributeTests
{
    private readonly OperationCancelledExceptionFilterAttribute _sutExceptionFilter;
    private readonly ExceptionContext _exceptionContext;

    public OperationCancelledExceptionFilterAttributeTests(ActionFilterFixture fixture)
    {
        _sutExceptionFilter = new();
        _exceptionContext = fixture.CreateExceptionContext();
    }

    [Fact]
    public void OnException_TokenIsCancelled_ShouldReturnStatusCodeResultWith499()
    {
        // Arrange
        int expectedStatusCode = 499;
        CancellationToken cancelledToken = new(canceled: true);

        _exceptionContext.Exception = new OperationCanceledException(cancelledToken);

        // Act
        _sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(expectedStatusCode);

    }

    [Fact]
    public void OnException_TokenIsNotCancelled_ShouldReturnNullResultAndExceptionNotHandeled()
    {
        // Arrange
        CancellationToken notCancelledToken = new(canceled: false);

        _exceptionContext.Exception = new OperationCanceledException(notCancelledToken);

        // Act
        _sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeFalse();

        _exceptionContext.Result.Should().BeNull();
    }
}
