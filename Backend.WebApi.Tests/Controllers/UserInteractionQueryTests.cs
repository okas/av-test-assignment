using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.Controllers;

[Collection("ApiLocalDbFixture")]
public class UserInteractionQueryTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="dbFixture"></param>
    public UserInteractionQueryTests(ApiLocalDbFixture dbFixture)
    {
        // Arrange for tests
        // Generate some unique ID's and their `IsOpen` states that will be known to an guaranteed to exist in DB during test.
        _knownEntitesIdIsOpen = Enumerable.Range(0, 5).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToArray();
        _sutDbContext = dbFixture.CreateContext();
        UserInteractionUtilities.SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutController = new UserInteractionsController(new UserInteractionService(_sutDbContext));
    }

    [Fact]
    public async Task Get_CanGetAll_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange
        // Act
        var response = await _sutController.GetUserInteractions();

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeAssignableTo<IEnumerable<UserInteractionDto>>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange+
        Guid knownId = _knownEntitesIdIsOpen.First().Id;

        // Act
        var response = await _sutController.GetUserInteraction(knownId);

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeOfType<UserInteractionDto>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnDtoWithCorrectId()
    {
        // Arrange+
        Guid knownId = _knownEntitesIdIsOpen.First(known => known.IsOpen).Id;

        // Act
        var response = await _sutController.GetUserInteraction(knownId);

        // Assert
        response.Result.As<OkObjectResult>().Value.As<UserInteractionDto>()
                .Id.Should().Be(knownId);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ReturnNotFoundResult()
    {
        // Arrange
        // Act
        var response = await _sutController.GetUserInteraction(Guid.NewGuid());

        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Get_CanGetUnFiltered_ReturnSome()
    {
        // Arrange
        // Act
        var response = await _sutController.GetUserInteractions();

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenTrue_ReturnSome()
    {
        // Arrange
        // Act
        var response = await _sutController.GetUserInteractions(true);

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenFalse_ReturnSome()
    {
        // Arrange
        // Act
        var response = await _sutController.GetUserInteractions(false);

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
