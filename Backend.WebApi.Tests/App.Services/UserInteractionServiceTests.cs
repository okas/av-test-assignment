using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Services;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionServiceTests : IDisposable
{
    private static readonly string _becauseKnownOrMoreEntitiesExpected;
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionService _sutService;

    static UserInteractionServiceTests()
    {
        _becauseKnownOrMoreEntitiesExpected = "test data may contain more models, but never less than known entities";
    }

    public UserInteractionServiceTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(4);
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutDbContext = dbFixture.CreateContext();
        _sutService = new UserInteractionService(_sutDbContext);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_FilteredByIsOpenWithProjectToDto_ReturnsFilteredProjectedCollection(bool isOpenTestValue)
    {
        // Arrange
        Expression<Func<UserInteraction, bool>> filter = model => model.IsOpen == isOpenTestValue;

        // Act
        (IEnumerable<UserInteractionDto> modelsDto, int totalCount) =
             await _sutService.Get(
                 ct: default,
                 projection: UserInteractionDto.Projection,
                 filters: filter
                 );

        // Assert
        using (new AssertionScope())
        {
            modelsDto.Should().NotBeNullOrEmpty()
                .And.OnlyContain(d => d.IsOpen == isOpenTestValue);
        }

        totalCount.Should().BeGreaterThanOrEqualTo(
            _knownEntitesIdIsOpen.Length,
            _becauseKnownOrMoreEntitiesExpected
            );
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task SetOpenState_ExistingInteraction_Succeeds(bool existingValue, bool newValue)
    {
        // Arrange
        (Guid id, _) = _knownEntitesIdIsOpen.First(k => k.IsOpen == existingValue);

        // Act
        await _sutService.SetOpenState(
                id,
                newState: newValue,
                ct: default
                );

        // Assert
        _sutDbContext.UserInteraction.Should().NotBeNullOrEmpty()
            .And.ContainSingle(model => model.Id == id && model.IsOpen == newValue);
    }

    [Fact]
    public async Task SetOpenState_NonExistingInteraction_Throws()
    {
        // Arrange
        Guid unknownId = Guid.NewGuid();

        // Act
        var act = () =>
             _sutService.SetOpenState(
                unknownId,
                newState: true,
                ct: default
                );

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User interaction not found, while attempting to set its Open state.")
            .Where(ex => (Guid?)ex.Data["Id"] == unknownId);
    }

    [Fact]
    public async Task CreateNew_NonExisting_ReturnModelFullyInitialized()
    {
        // Arrange+
        UserInteraction correctNewModel = new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty",
        };

        DateTime serviceQueryTime = DateTime.Now;

        // Act
        UserInteraction createdModel =
            await _sutService.Create(
                correctNewModel,
                ct: default
                );

        // Assert
        using AssertionScope _ = new();
        createdModel.Should().NotBeNull();
        createdModel!.Deadline.Should().Be(correctNewModel.Deadline);
        createdModel.Description.Should().Be(correctNewModel.Description);
        createdModel.Created.Should().BeAfter(serviceQueryTime);
        createdModel.IsOpen.Should().BeTrue();
        createdModel.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateNew_ModelAlreadyExists_Throws()
    {
        // Arrange
        UserInteraction attemptedModel = new()
        {
            Id = _knownEntitesIdIsOpen[0].Id,
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty",
        };

        // Act
        Func<Task<UserInteraction>> act = () =>
              _sutService.Create(
                 attemptedModel,
                 ct: default
                 );

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("User interaction already exists.")
            .Where(ex => (Guid?)ex.Data["Id"] == _knownEntitesIdIsOpen[0].Id);

    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
