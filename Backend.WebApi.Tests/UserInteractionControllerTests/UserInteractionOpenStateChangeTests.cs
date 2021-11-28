using System;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.UserInteractionControllerTests;

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
        SeedData(dbFixture);
        _sutController = new UserInteractionsController(_sutDbContext);
    }

    /// <summary>
    /// Generate test data to database, that can be requested from API tests.
    /// </summary>
    /// <remarks>
    /// Will use one-time DbContext to not to conflict with context used for testing.
    /// </remarks>
    /// <param name="dbFixture"></param>
    private void SeedData(ApiDbContextLocalDbFixture dbFixture)
    {
        using var context = dbFixture.CreateContext();
        context.UserInteraction.Add(new()
        {
            Id = _entityId,
            Created = DateTime.Now,
            Deadline = DateTime.Now.AddDays(1),
            Description = "Test \"IsOpen\" = false",
            IsOpen = true
        });
        context.SaveChanges();
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
        // Clean up //Arranged resources
        _sutDbContext.Dispose();
    }
}
