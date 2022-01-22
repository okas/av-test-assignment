using System;
using System.Threading.Tasks;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using static Backend.WebApi.App.Operations.UserInteractionCommands.UserInteractionCreateCommand;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionCreateCommandTests : IDisposable
{
    private readonly ApiDbContext _sutDbContext;
    private readonly Handler _sutCommandHandler;
    private static readonly UserInteractionCreateCommand _correctCommand;
    static UserInteractionCreateCommandTests() => _correctCommand = new()
    {
        Deadline = DateTime.Now.AddDays(1),
        Description = $"Long enough description created approximately at {DateTime.Now.ToLongTimeString()}",
    };

    public UserInteractionCreateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _sutDbContext = dbFixture.CreateContext();
        _sutCommandHandler = new(_sutDbContext, new NullLogger<Handler>());
    }

    [Fact]
    public async Task CreateNew_NonExisting_ReturnModelFullyInitialized()
    {
        // Arrange
        DateTime serviceQueryTime = DateTime.Now;

        // Act
        UserInteraction createdModel =
             await _sutCommandHandler.Handle(
                 _correctCommand,
                 ct: default
                 );

        // Assert
        using AssertionScope _ = new();
        createdModel.Should().NotBeNull();
        createdModel!.Deadline.Should().Be(_correctCommand.Deadline);
        createdModel.Description.Should().Be(_correctCommand.Description);
        createdModel.Created.Should().BeAfter(serviceQueryTime);
        createdModel.IsOpen.Should().BeTrue();
        createdModel.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateNew_NonExisting_ShouldNotThrowAlreadyExists()
    {
        // Arrange
        var act = () =>
            _sutCommandHandler.Handle(
                _correctCommand,
                ct: default
                );
        // Act
        // Assert
        await act.Should().NotThrowAsync<AlreadyExistsException>();
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}

