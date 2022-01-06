using System;
using System.Threading.Tasks;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionQueries;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionGetByIdQueryTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionGetByIdHandler _sutCommandHandler;

    public UserInteractionGetByIdQueryTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(1);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task GetOne_ByCorrectId_ReturnModel()
    {
        // Arrange
        UserInteractionGetByIdQuery correctQuery = new()
        {
            Id = _knownEntitesIdIsOpen[0].Id,
        };

        // Act
        UserInteraction model =
            await _sutCommandHandler.Handle(
                correctQuery,
                ct: default
                );

        // Assert
        using AssertionScope _ = new();
        model.Should().NotBeNull();
        model!.Id.Should().Be(correctQuery.Id);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
