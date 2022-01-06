using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using Backend.WebApi.Domain.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionQueryTests
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="fixture"></param>
    public UserInteractionQueryTests(IntegrationTestFixture fixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(5);
        SeedData(fixture, _knownEntitesIdIsOpen);
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

    [Fact]
    public async Task Get_CanGetSingleById_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange
        Guid id = _knownEntitesIdIsOpen[0].Id;

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                id,
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
        Guid id = _knownEntitesIdIsOpen[0].Id;

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                id,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<UserInteractionDto>()
            .Which.Id.Should().Be(id);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ThrowsWithExpectedMessage()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        Func<Task<ActionResult<UserInteractionDto>>> act = () =>
            _sutController.GetUserInteraction(
                id,
                ct: default
                );

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User interaction not found");
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
