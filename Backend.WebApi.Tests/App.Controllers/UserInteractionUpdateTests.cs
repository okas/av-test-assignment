using AutoFixture.Xunit2;
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
public sealed class UserInteractionUpdateTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly Guid _entityId;
    private readonly UserInteractionsController _sutController;

    // Arrange for tests
    public UserInteractionUpdateTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _entityId = Guid.NewGuid();
        SeedData(fixture, (_entityId, IsOpen: true));
        _sutController = fixture.ScopedServiceProvider!.GetService<UserInteractionsController>()!;
    }

    [Theory]
    [AutoData]
    public async Task Put_CanUpdateExisting_Succeeds(
        // Arrange
        [Frozen(Matching.PropertyName)] DateTime deadline,
        [Frozen(Matching.PropertyName)] string description,
        [Frozen(Matching.PropertyName)] bool isOpen)
    {
        UserInteractionUpdateCommand correctModelCommand = new(
            _entityId, deadline, description, isOpen
        );

        // Act
        IActionResult response =
            await _sutController.PutUserInteraction(
                _entityId,
                correctModelCommand,
                ct: default
                );

        // Assert
        response.Should().BeOfType<NoContentResult>().And.NotBeNull();

        using ApiDbContext newContext = _fixture.CreateContext();
        UserInteraction? interactionModel = await newContext.UserInteraction.FindAsync(_entityId);
        interactionModel.Should().NotBeNull();
        interactionModel!.Deadline.Should().Be(deadline);
        interactionModel.Description.Should().Be(description);
        interactionModel.IsOpen.Should().Be(isOpen);
    }
}
