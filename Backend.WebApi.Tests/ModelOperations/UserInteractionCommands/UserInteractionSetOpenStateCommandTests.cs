using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.ModelOperations.UserInteractionCommands;
using Backend.WebApi.Services;
using FluentAssertions;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.ModelOperations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionSetOpenStateCommandTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionSetOpenStateHandler _sutCommandHandler;

    public UserInteractionSetOpenStateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(4);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task SetOpenState_CurrentlyOpenInteraction_WasSetToClosed()
    {
        // Arrange+
        (Guid id, _) = _knownEntitesIdIsOpen.First(k => k.IsOpen);

        UserInteractionSetOpenStateCommand correctModelCommand = new()
        {
            Id = id,
            IsOpen = false,
        };

        // Act
        IEnumerable<ServiceError> errors =
            await _sutCommandHandler.Handle(
                correctModelCommand
                );

        // Assert
        errors.Should().BeNullOrEmpty();

        _sutDbContext.UserInteraction.Should().Contain(model => !model.IsOpen);
    }

    [Fact]
    public async Task SetOpenState_NonExistingInteraction_ReturnNotFoundResultWithCorrectError()
    {
        // Arrange+
        UserInteractionSetOpenStateCommand notExistingModelCommand = new()
        {
            Id = Guid.NewGuid(),
            IsOpen = true,
        };

        // Act
        IEnumerable<ServiceError> errors =
            await _sutCommandHandler.Handle(
                notExistingModelCommand
                );

        // Assert
        errors.Should().NotBeNullOrEmpty();
        errors!.Select(error => error.Kind).Should().Contain(ServiceErrorKind.NotFoundOnChange);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}

