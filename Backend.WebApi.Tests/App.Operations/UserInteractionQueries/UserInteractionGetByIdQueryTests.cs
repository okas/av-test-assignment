using System;
using System.Threading.Tasks;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionQueries;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionGetByIdQueryTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionGetByIdQuery.Handler _sutCommandHandler;

    public UserInteractionGetByIdQueryTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(1);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task GetOne_ByCorrectId_ReturnDtol()
    {
        // Arrange
        UserInteractionGetByIdQuery correctQuery = new(_knownEntitesIdIsOpen[0].Id);

        // Act
        UserInteractionDto? dto =
            await _sutCommandHandler.Handle(
                correctQuery,
                ct: default
                );

        // Assert
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
