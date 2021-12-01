using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.UserInteractionControllerTests;

[Collection("ApiDbContext")]
public class UserInteractionQueryTests : IDisposable
{
    private readonly IList<(Guid, bool)> _knownEntityIds;
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
        _knownEntityIds = Enumerable.Range(0, 5).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToList();
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

        context.UserInteraction.AddRange(
            _knownEntityIds.Select(tuple =>
            new UserInteraction()
            {
                Id = tuple.Item1,// known guid
                Created = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                Description = $@"Test entity to test {tuple.Item1.ToString()[..8]}",// take first 8 chars for visual uniqueness
                IsOpen = tuple.Item2,// known state.
            }));

        context.SaveChanges();
    }

    [Fact]
    public async Task Get_CanGetAll_ReturnOkObjectResultAndNonNullValue()
    {
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
        // Act
        var response = await _sutController.GetUserInteraction(_knownEntityIds[0].Item1);

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeOfType<UserInteractionDto>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnDtoWithCorrectId()
    {
        // Act
        var response = await _sutController.GetUserInteraction(_knownEntityIds[1].Item1);

        // Assert
        response.Result.As<OkObjectResult>().Value.As<UserInteractionDto>()
                .Id.Should().Be(_knownEntityIds[1].Item1);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ReturnNotFoundResult()
    {
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
