using System;
using System.Collections.Generic;
using System.Linq;
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
public class UserInteractionQueryTests : IDisposable
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntityIds;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="dbFixture"></param>
    public UserInteractionQueryTests(ApiDbContextLocalDbFixture dbFixture)
    {
        // Arrange for tests
        // Generate some unique ID's and their `IsOpen` states that will be known to an guaranteed to exist in DB during test.
        _knownEntityIds = Enumerable.Range(0, 5).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToArray();
        _sutDbContext = dbFixture.CreateContext();
        UserInteractionUtilities.SeedData(dbFixture, _knownEntityIds);
        _sutController = new UserInteractionsController(_sutDbContext);
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
        Guid knownId = _knownEntityIds.First().Id;

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
        Guid knownId = _knownEntityIds.First(known => known.IsOpen).Id;

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
    /// Clean up arranged resources of tests
    /// </summary>
    public void Dispose()
    {
        // Clean up test class level Arranged resources
        _sutDbContext.Dispose();
    }
}
