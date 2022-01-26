using AutoFixture.Xunit2;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using static Backend.WebApi.App.Operations.UserInteractionCommands.UserInteractionUpdateCommand;
using static Backend.WebApi.Tests.UserInteractionUtilities;
using static FluentAssertions.FluentActions;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionCommands;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionUpdateCommandTests : IDisposable
{
    private readonly ApiLocalDbFixture _dbFixture;
    private readonly ApiDbContext _sutDbContext;
    private readonly Handler _sutCommandHandler;

    public UserInteractionUpdateCommandTests(ApiLocalDbFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _sutDbContext = dbFixture.CreateContext();
        _sutCommandHandler = new(_sutDbContext, new NullLogger<Handler>());
    }

    [Theory]
    [AutoData]
    public async Task Update_NonExistingInteraction_ThrowsNotFound(
        // Arrange
        [Frozen] Guid Id,
        UserInteractionUpdateCommand notExistingModelCommand)
    {
        Task<Unit> act() => _sutCommandHandler.Handle(
                notExistingModelCommand,
                ct: default);

        // Act
        var result = Invoking(act);

        // Assert
        using AssertionScope _ = new();

        (await result.Should().ThrowAsync<NotFoundException>()
             .WithMessage("Operation cancelled.")
             .Where(ex => ex.InnerException == null))
             .Which.Data[BaseException.ModelDataKey].Should().BeEquivalentTo(new { Id });
    }

    [Theory]
    [AutoData]
    public async Task Update_ExistingInteraction_Succeeds(
        // Arrange
        [Frozen(Matching.PropertyName)] Guid Id,
        [Frozen(Matching.PropertyName)] DateTime Deadline,
        [Frozen(Matching.PropertyName)] string Description,
        [Frozen(Matching.PropertyName)] bool IsOpen,
        UserInteractionUpdateCommand correctModelCommand)
    {
        SeedData(_dbFixture, Id);

        // Act
        _ = await _sutCommandHandler.Handle(
                correctModelCommand,
                ct: default
                );

        // Assert
        _sutDbContext.UserInteraction.Find(Id).Should().BeEquivalentTo(new
        {
            Deadline,
            Description,
            IsOpen,
        });
    }

    [Theory]
    [AutoData]
    public async Task Update_HandleConcurrentEntityDeletion_ThrowsNotFoundWithInnerException(
        // Arrange
        [Frozen(Matching.PropertyName)] Guid Id,
        UserInteractionUpdateCommand correctModelCommand
    )
    {
        SeedData(_dbFixture, Id);

        // The concurrent delete: just after entity should be loadad into SUT DbContext and before saving its changes.
        _sutDbContext.SavingChanges += (sender, e) =>
        {
            using ApiDbContext otherContext = _dbFixture.CreateContext();
            otherContext.Remove(otherContext.UserInteraction.Find(Id)!);
            otherContext.SaveChanges();
        };

        Task<Unit> act() => _sutCommandHandler.Handle(
                 correctModelCommand,
                 ct: default
                 );

        // Act
        Func<Task<Unit>> result = Invoking(act);

        // Assert
        using AssertionScope s = new();

        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Operation cancelled, during concurrency handling.")
            .WithInnerExceptionExactly<NotFoundException, DbUpdateConcurrencyException>();
    }

    [Theory]
    [AutoData]
    public async Task Update_HandleConcurrencyChange_ShouldSucceedByForcingCommandValue(
        // Arrange
        [Frozen(Matching.PropertyName)] Guid Id,
        [Frozen(Matching.PropertyName)] DateTime Deadline,
        [Frozen(Matching.PropertyName)] string Description,
        [Frozen(Matching.PropertyName)] bool IsOpen,
        string concurrentChangeDescription,
        UserInteractionUpdateCommand correctModelCommand
        )
    {
        SeedData(_dbFixture, Id);
        // The concurrent delete: just after entity should be loadad into SUT DbContext and before saving its changes.
        _sutDbContext.SavingChanges += (sender, e) =>
        {
            using ApiDbContext otherContext = _dbFixture.CreateContext();
            UserInteraction entity = otherContext.UserInteraction.Find(Id)!;
            entity.Deadline = Deadline.AddYears(1);
            entity.Description = concurrentChangeDescription;
            entity.IsOpen = !IsOpen;
            otherContext.SaveChanges();
        };

        // Act
        _ = await _sutCommandHandler.Handle(
                 correctModelCommand,
                 ct: default
                 );

        // Assert
        _sutDbContext.UserInteraction.Find(Id).Should().BeEquivalentTo(new
        {
            Deadline,
            Description,
            IsOpen,
        });
    }

    // TODO add cancellable operation testing: 1) "Request Abort", 2) "to many retries", for operation type internal cancellation.

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
