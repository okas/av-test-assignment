using System;
using System.Threading.Tasks;
using Backend.WebApi.Controllers;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.Controllers;

[Collection("ApiDbContext")]
public class UserInteractionCreationTests : IDisposable
{
    const string _nonEmptyDescription = "Non-empty";
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionsController _sutController;

    public UserInteractionCreationTests(ApiDbContextLocalDbFixture dbFixture)
    {
        //  //Arrange common code!
        // I think it is OK, because this class is instantiated for every test(method). 
        _sutDbContext = dbFixture.CreateContext();
        _sutController = new UserInteractionsController(_sutDbContext);
    }

    public UserInteractionNewDto CorrectNewDto
    {
        get => new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = _nonEmptyDescription,
        };
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCreatedActionResultWithDtoInstance()
    {
        // Arrange
        // Act
        var response = await _sutController.PostUserInteraction(CorrectNewDto);

        // Assert
        response.Result.As<CreatedAtActionResult>().Value.As<UserInteractionDto>().Should().NotBeNull();
    }

    [Fact]
    public async Task Post_CanCreateInteraction_ReturnCorrectApiGeneratedProperties()
    {
        // Arrange
        // Act
        var response = await _sutController.PostUserInteraction(CorrectNewDto);
        var dto = response.Result.As<CreatedAtActionResult>()
                          .Value.As<UserInteractionDto>();

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Created.Should().BeOnOrAfter(DateTime.Now.AddSeconds(-5)).And.NotBeAfter(DateTime.Now);
        dto.IsOpen.Should().BeTrue();
        dto.Description.Should().BeEquivalentTo(CorrectNewDto.Description);
    }

    public void Dispose()
    {
        // Clean up //Arranged resources
        _sutDbContext.Dispose();
    }
}
