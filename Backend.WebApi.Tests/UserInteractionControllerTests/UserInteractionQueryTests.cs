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
    private readonly IList<Guid> _knownEntityIds;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    public UserInteractionQueryTests(ApiDbContextLocalDbFixture dbFixture)
    {
        // Arrange for tests
        _knownEntityIds = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToList();
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
            _knownEntityIds.Select((id, i) =>
            new UserInteraction()
            {
                Id = id,
                Created = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                Description = $@"Test e\ntity""{i}"" for query tests ",
                IsOpen = true
            }));

        context.SaveChanges();
    }

    [Fact]
    public async Task Get_CanGetAll_ReturnOkObjectResultAndNonNullValue()
    {
        // Act
        var response = await _sutController.GetAllUserInteractions();

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeAssignableTo<IEnumerable<UserInteractionDto>>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnOkObjectResultAndNonNullValue()
    {
        // Act
        var response = await _sutController.GetUserInteraction(_knownEntityIds[0]);

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeOfType<UserInteractionDto>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnDtoWithCorrectId()
    {
        // Act
        var response = await _sutController.GetUserInteraction(_knownEntityIds[1]);

        // Assert
        response.Result.As<OkObjectResult>().Value.As<UserInteractionDto>()
                .Id.Should().Be(_knownEntityIds[1]);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ReturnNotFoundResult()
    {
        // Act
        var response = await _sutController.GetUserInteraction(Guid.NewGuid());

        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Clean up arranged resources of tests
    /// </summary>
    public void Dispose()
    {
        _sutDbContext.Dispose();
    }
}
