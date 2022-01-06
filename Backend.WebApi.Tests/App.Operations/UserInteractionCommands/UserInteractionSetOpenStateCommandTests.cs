using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionSetOpenStateCommandTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionSetOpenStateHandler _sutCommandHandler;

    public UserInteractionSetOpenStateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(2);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task SetOpenState_ExistingInteraction_Succeeds(bool existingValue, bool newValue)
    {
        // Arrange+
        (Guid id, _) = _knownEntitesIdIsOpen.First(k => k.IsOpen == existingValue);

        UserInteractionSetOpenStateCommand correctModelCommand = new()
        {
            Id = id,
            IsOpen = newValue,
        };

        // Act
        _ = await _sutCommandHandler.Handle(
                correctModelCommand,
                ct: default
                );

        // Assert
        _sutDbContext.UserInteraction.Should().ContainSingle(model => model.Id == id && model.IsOpen == newValue);
    }

    [Fact]
    public async Task SetOpenState_NonExistingInteraction_Throws()
    {
        // Arrange+
        UserInteractionSetOpenStateCommand notExistingModelCommand = new()
        {
            Id = Guid.NewGuid(),
        };

        // Act
        var act = () =>
            _sutCommandHandler.Handle(
                notExistingModelCommand,
                ct: default
                );

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User interaction not found, while attempting to set its Open state.")
            .Where(ex => (Guid?)ex.Data["Id"] == notExistingModelCommand.Id);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}

