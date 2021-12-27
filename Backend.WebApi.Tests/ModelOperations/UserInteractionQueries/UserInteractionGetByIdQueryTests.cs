using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.ModelOperations.UserInteractionQueries;
using Backend.WebApi.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.ModelOperations.UserInteractionQueries;

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
    public async Task GetOne_ByCorrectId_ReturnModelWithNoerrors()
    {
        // Arrange
        UserInteractionGetByIdQuery correctQuery = new()
        {
            Id = _knownEntitesIdIsOpen.First().Id
        };

        // Act
        (IEnumerable<ServiceError> errors, UserInteraction? model) =
           await _sutCommandHandler.Handle(
               correctQuery
               );

        // Assert
        errors.Should().BeNullOrEmpty();

        using AssertionScope _ = new();
        model.Should().NotBeNull();
        model!.Id.Should().Be(correctQuery.Id);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
