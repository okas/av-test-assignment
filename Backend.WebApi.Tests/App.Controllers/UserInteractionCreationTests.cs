using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]

public sealed class UserInteractionCreationTests
{
    private readonly UserInteractionCreateCommand _knownCorrectCommand;
    private readonly UserInteractionsController _sutController;

    public UserInteractionCreationTests(IntegrationTestFixture fixture)
    {
        _knownCorrectCommand = new(DateTime.Now.AddDays(1), "Non-empty");

        _sutController = fixture.ScopedServiceProvider!.GetService<UserInteractionsController>()!;
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCreatedActionResultWithDtoInstance()
    {
        // Arrange
        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.PostUserInteraction(
                _knownCorrectCommand,
                ct: default
                );

        // Assert
        response.Result.As<CreatedAtActionResult>().Value.As<UserInteractionDto>().Should().NotBeNull();
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCorrectApiGeneratedProperties()
    {
        // Arrange
        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.PostUserInteraction(
                _knownCorrectCommand,
                ct: default
                );

        UserInteractionDto dto = response.Result.As<CreatedAtActionResult>()
                          .Value.As<UserInteractionDto>();

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Created.Should().BeOnOrAfter(DateTime.Now.AddSeconds(-5)).And.NotBeAfter(DateTime.Now);
        dto.IsOpen.Should().BeTrue();
        dto.Description.Should().BeEquivalentTo(_knownCorrectCommand.Description);
    }

}
