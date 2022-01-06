﻿using System;
using System.Threading.Tasks;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionCreateCommandTests : IDisposable
{
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionCreateCommand.Handler _sutCommandHandler;

    public UserInteractionCreateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _sutDbContext = dbFixture.CreateContext();
        _sutCommandHandler = new(_sutDbContext);
    }

    [Fact]
    public async Task CreateNew_NonExisting_ReturnModelFullyInitialized()
    {
        // Arrange+
        UserInteractionCreateCommand correctCommand = new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty",
        };

        DateTime serviceQueryTime = DateTime.Now;

        // Act
        UserInteraction createdModel =
             await _sutCommandHandler.Handle(
                 correctCommand,
                 ct: default
                 );

        // Assert
        using AssertionScope _ = new();
        createdModel.Should().NotBeNull();
        createdModel!.Deadline.Should().Be(correctCommand.Deadline);
        createdModel.Description.Should().Be(correctCommand.Description);
        createdModel.Created.Should().BeAfter(serviceQueryTime);
        createdModel.IsOpen.Should().BeTrue();
        createdModel.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}

