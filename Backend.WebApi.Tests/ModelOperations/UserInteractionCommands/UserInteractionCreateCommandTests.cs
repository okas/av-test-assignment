using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.ModelOperations.UserInteractionCommands;
using Backend.WebApi.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.ModelOperations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public class UserInteractionCreateCommandTests : IDisposable
{
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionCreateHandler _sutCommandHandler;

    public UserInteractionCreateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _sutDbContext = dbFixture.CreateContext();
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task CreateNew_NonExisting_ReturnModelFullyInitialized()
    {
        // Arrange+
        UserInteractionCreateCommand correctCommand = new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty"
        };

        DateTime serviceQueryTime = DateTime.Now;

        // Act
        (IEnumerable<ServiceError>? errors, Model.UserInteraction? createdModel) =
            await _sutCommandHandler.Handle(
                correctCommand
                );

        // Assert
        errors.Should().BeNullOrEmpty();

        using AssertionScope _ = new();
        createdModel.Should().NotBeNull();
        createdModel.Deadline.Should().Be(correctCommand.Deadline);
        createdModel.Description.Should().Be(correctCommand.Description);
        createdModel.Created.Should().BeAfter(serviceQueryTime);
        createdModel.IsOpen.Should().BeTrue();
        createdModel.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}

