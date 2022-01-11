using System;
using Backend.WebApi.App.Extensions;
using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Extensions;

[Collection("ActionFilterFixture")]
public class CRUDOperationsExceptionFilterTests
{
    private readonly CUDOperationsExceptionFilter _sutExceptionFilter;
    private readonly ExceptionContext _exceptionContext;

    public CRUDOperationsExceptionFilterTests(ActionFilterFixture fixture)
    {
        _sutExceptionFilter = new();
        _exceptionContext = fixture.CreateExceptionContext();
    }

    [Fact]
    public void OnException_NotFoundException_ShouldSetConflictObjectResultWithCorrectValueObject()
    {
        // Arrange
        string errorMessage = "error message";
        Guid id = Guid.NewGuid();

        object resultValueObject = new
        {
            Id = new
            {
                Value = id,
                Error = errorMessage,
            },
        };

        _exceptionContext.Exception = new NotFoundException(errorMessage, id);

        // Act
        _sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().NotBeNull()
            .And.BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.Subject.Should().BeEquivalentTo(resultValueObject);

    }

    [Fact]
    public void OnException_AlreadyExistsException_ShouldSetConflictObjectResultWithCorrectValueObject()
    {
        // Arrange
        string errorMessage = "error message";
        Guid id = Guid.NewGuid();

        object resultValueObject = new
        {
            Id = new
            {
                Value = id,
                Error = errorMessage,
            },
        };

        _exceptionContext.Exception = new AlreadyExistsException(errorMessage, "Id", id);

        // Act
        _sutExceptionFilter.OnException(
            _exceptionContext
            );

        // Assert
        using AssertionScope _ = new();
        _exceptionContext.ExceptionHandled.Should().BeTrue();

        _exceptionContext.Result.Should().NotBeNull()
            .And.BeOfType<ConflictObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.Subject.Should().BeEquivalentTo(resultValueObject);
    }
}
