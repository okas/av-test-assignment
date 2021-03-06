using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionOpenStateChangeTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly Guid _entityId;
    private readonly UserInteractionsController _sutController;

    // Arrange for tests
    public UserInteractionOpenStateChangeTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _entityId = Guid.NewGuid();
        SeedData(fixture, (_entityId, IsOpen: true));
        _sutController = fixture.ScopedServiceProvider!.GetService<UserInteractionsController>()!;
    }

    [Fact]
    public async Task Patch_CanMarkInteractionClosed_ReturnNoContentResultAndModelEntityIsClosedEqFalse()
    {
        // Arrange
        UserInteractionSetOpenStateCommand command = new()
        {
            Id = _entityId,
            IsOpen = false,
        };

        // Act
        IActionResult response =
            await _sutController.PatchUserInteraction(
                _entityId,
                command,
                ct: default
                );

        // Assert
        response.Should().BeOfType<NoContentResult>().And.NotBeNull();

        using ApiDbContext newContext = _fixture.CreateContext();
        UserInteraction? interactionModel = await newContext.UserInteraction.FindAsync(_entityId);
        interactionModel.Should().NotBeNull();
        interactionModel!.IsOpen.Should().BeFalse();
    }
}
