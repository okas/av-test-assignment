using System;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.Controllers;

[Collection("ApiLocalDbFixture")]
public class UserInteractionOpenStateChangeTests : IDisposable
{
    private readonly ApiLocalDbFixture _dbFixture;
    private readonly Guid _entityId;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    // Arrange for tests
    public UserInteractionOpenStateChangeTests(ApiLocalDbFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _entityId = Guid.NewGuid();
        _sutDbContext = dbFixture.CreateContext();
        UserInteractionUtilities.SeedData(dbFixture, (_entityId, true));
        _sutController = new UserInteractionsController(new UserInteractionService(_sutDbContext));
    }

    [Fact]
    public async Task Patch_CanMarkInteractionClosed_ReturnNoContentResultAndModelEntityIsClosedEqFalse()
    {
        // Arrange
        UserInteractionIsOpenDto isOpenDto = new() { Id = _entityId, IsOpen = false };

        // Act
        var response = await _sutController.PatchUserInteraction(_entityId, isOpenDto);

        // Assert
        response.Should().BeOfType<NoContentResult>().And.NotBeNull();

        using var newContext = _dbFixture.CreateContext();
        UserInteraction? interactionModel = newContext.UserInteraction.Find(_entityId);
        interactionModel.Should().NotBeNull();
        interactionModel.IsOpen.Should().BeFalse();
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
