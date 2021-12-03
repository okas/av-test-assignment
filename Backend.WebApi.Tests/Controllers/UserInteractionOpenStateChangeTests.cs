using System;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.Tests.TestInfrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.Controllers;

[Collection("ApiDbContext")]
public class UserInteractionOpenStateChangeTests : IDisposable
{
    private readonly ApiDbContextLocalDbFixture _dbFixture;
    private readonly Guid _entityId;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    // Arrange for tests
    public UserInteractionOpenStateChangeTests(ApiDbContextLocalDbFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _entityId = Guid.NewGuid();
        _sutDbContext = dbFixture.CreateContext();
        UserInteractionUtilities.SeedData(dbFixture, (_entityId, true));
        _sutController = new UserInteractionsController(_sutDbContext);
    }

    [Fact]
    public async Task Patch_CanMarkInteractionClosed_ReturnNoContentResultAndModelEntityIsClosedEqFalse()
    {
        // Arrange
        UserInteractionIsOpenDto isOpenDto = new() { Id = _entityId, IsOpen = false };
        using var newContext = _dbFixture.CreateContext();

        // Act
        var response = await _sutController.PatchUserInteraction(_entityId, isOpenDto);

        // Assert
        response.Should().BeOfType<NoContentResult>().And.NotBeNull();

        var interactionModel = newContext.UserInteraction.Find(_entityId);
        interactionModel.IsOpen.Should().BeFalse();
    }

    /// <summary>
    /// Clean up arranged resources of tests
    /// </summary>
    public void Dispose()
    {
        // Clean up test class level Arranged resources
        _sutDbContext.Dispose();
    }
}
