using System;
using System.Threading.Tasks;
using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Services;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionCreationTests : IDisposable
{
    private readonly UserInteractionNewDto _knownCorrectDto;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    public UserInteractionCreationTests(ApiLocalDbFixture dbFixture)
    {
        _knownCorrectDto = new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty",
        };
        _sutDbContext = dbFixture.CreateContext();
        _sutController = new UserInteractionsController(new UserInteractionService(_sutDbContext));
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCreatedActionResultWithDtoInstance()
    {
        // Arrange
        // Act
        var response = await _sutController.PostUserInteraction(_knownCorrectDto);

        // Assert
        response.Result.As<CreatedAtActionResult>().Value.As<UserInteractionDto>().Should().NotBeNull();
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCorrectApiGeneratedProperties()
    {
        // Arrange
        // Act
        var response = await _sutController.PostUserInteraction(_knownCorrectDto);
        var dto = response.Result.As<CreatedAtActionResult>()
                          .Value.As<UserInteractionDto>();

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Created.Should().BeOnOrAfter(DateTime.Now.AddSeconds(-5)).And.NotBeAfter(DateTime.Now);
        dto.IsOpen.Should().BeTrue();
        dto.Description.Should().BeEquivalentTo(_knownCorrectDto.Description);
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
