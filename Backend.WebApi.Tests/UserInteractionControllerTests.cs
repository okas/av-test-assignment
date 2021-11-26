using System;
using Backend.WebApi.Controllers;
using Backend.WebApi.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests;

public class UserInteractionControllerTests : IClassFixture<ApiDbContextLocalDbFixture>
{
    const string _nonEmptyDescription = "Non-empty";

    public UserInteractionControllerTests(ApiDbContextLocalDbFixture dbFixture) => DbFixture = dbFixture;

    public ApiDbContextLocalDbFixture DbFixture { get; private set; }

    private UserInteractionDto CorrectDtoOpenFixture
    {
        get => new()
        {
            Id = Guid.NewGuid(),
            Created = DateTime.Now,
            Description = _nonEmptyDescription,
            Deadline = DateTime.Now.AddDays(1),
            IsOpen = true
        };
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
    public async void Post_CanCreateInteraction_ReturnCreatedActionResultWithDtoInstance()
    {
        using var context = DbFixture.CreateContext();
        var controller = new UserInteractionsController(context);

        var responseResult = (await controller.PostUserInteraction(CorrectNewDto)).Result;

        responseResult.Should().NotBeNull()
            .And.BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeOfType<UserInteractionDto>()
            .Which.Should().NotBeNull();
    }

    [Fact]
    public async void Post_CanCreateInteraction_ReturnCorrectApiGeneratedProperties()
    {
        using var context = DbFixture.CreateContext();
        var controller = new UserInteractionsController(context);

        var dto = (await controller.PostUserInteraction(CorrectNewDto))
            .Result.As<CreatedAtActionResult>().Value.As<UserInteractionDto>();

        dto.Id.Should().NotBeEmpty();
        dto.Created.Should().BeOnOrAfter(DateTime.Now.AddSeconds(-5)).And.NotBeAfter(DateTime.Now);
        dto.IsOpen.Should().BeTrue();
        dto.Description.Should().BeEquivalentTo(CorrectNewDto.Description);
    }
}

