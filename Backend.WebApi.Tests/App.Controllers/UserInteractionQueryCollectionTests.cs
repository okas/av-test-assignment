using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionQueryCollectionTests
{
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="fixture"></param>
    public UserInteractionQueryCollectionTests(IntegrationTestFixture fixture)
    {
        SeedData(fixture, GenerateWithKnownIdIsOpen(3));
        _sutController = new UserInteractionsController(
            fixture.ScopedServiceProvider!.GetService<IMediator>()!);
    }

    [Fact]
    public async Task Get_CanGetAll_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: default,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.BeAssignableTo<IEnumerable<UserInteractionDto>>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Get_CanGetUnFiltered_ReturnSome(bool? filterValue)
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: filterValue,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
            .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenTrue_ReturnSome()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: true,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
            .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenFalse_ReturnSome()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: false,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().NotBeNull()
            .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
            .Which.Should().HaveCountGreaterThan(0);
    }
}
