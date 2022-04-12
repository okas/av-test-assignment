using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionQueries;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionGetByIdQueryTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitiesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionGetByIdQuery.Handler _sutCommandHandler;

    public UserInteractionGetByIdQueryTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitiesIdIsOpen = GenerateWithKnownIdIsOpen(1);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitiesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task GetOne_ByCorrectId_ReturnDto()
    {
        // Arrange
        UserInteractionGetByIdQuery correctQuery = new(_knownEntitiesIdIsOpen[0].Id);

        // Act
        UserInteractionDto? dto =
            await _sutCommandHandler.Handle(
                correctQuery,
                ct: default
                );

        // Assert
        using AssertionScope _ = new();

        dto.Should().NotBeNull();

        dto.Value.Id.Should().Be(correctQuery.Id);
    }

    [Fact]
    public async Task GetOne_ByNotExistingId_ReturnNull()
    {
        // Arrange
        UserInteractionGetByIdQuery correctQuery = new(Guid.NewGuid());

        // Act
        UserInteractionDto? dto =
            await _sutCommandHandler.Handle(
                correctQuery,
                ct: default
                );

        // Assert
        dto.Should().BeNull();
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
