using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionQuerySingleTests
{
    private readonly UserInteractionKnownTestData[] _knownEntitesIdIsOpen;
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="fixture"></param>
    public UserInteractionQuerySingleTests(IntegrationTestFixture fixture)
    {
        _knownEntitesIdIsOpen = SeedDataGenerateAndReturnKnown(fixture, 3);
        _sutController = new UserInteractionsController(
            fixture.ScopedServiceProvider!.GetService<IMediator>()!);
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange
        UserInteractionGetByIdQuery query = new(_knownEntitesIdIsOpen[0].Id);

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                query,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.BeOfType<UserInteractionDto>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnDtoWithCorrectId()
    {
        // Arrange
        UserInteractionGetByIdQuery query = new(_knownEntitesIdIsOpen[0].Id);

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                query,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<UserInteractionDto>()
            .Which.Id.Should().Be(query.Id);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ReturnsNotFoundResult()
    {
        // Arrange
        UserInteractionGetByIdQuery query = new(Guid.NewGuid());

        // Act
        ActionResult<UserInteractionDto> act =
            await _sutController.GetUserInteraction(
                query,
                ct: default
                );

        // Assert
        act.Result.Should().NotBeNull()
           .And.BeOfType<NotFoundResult>();
    }
}
