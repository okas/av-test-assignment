using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.MapperExtensions;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using Backend.WebApi.Tests.TestInfrastructure;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.Services;

[Collection("ApiDbContext")]
public class UserInteractionServiceTests : IDisposable
{
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionService _sutService;
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContextLocalDbFixture _dbFixture;

    public UserInteractionServiceTests(ApiDbContextLocalDbFixture dbFixture)
    {
        _dbFixture = dbFixture;
        // Arrange for tests
        // Generate some unique ID's and their `IsOpen` states that will be known to an guaranteed to exist in DB during test.
        _knownEntitesIdIsOpen = Enumerable.Range(0, 4).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToArray();
        UserInteractionUtilities.SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutDbContext = dbFixture.CreateContext();
        _sutService = new UserInteractionService(_sutDbContext);
    }

    [Fact]
    public async Task GetSome_FilterOpen_ReturnsCollectionFilteredByCriterion()
    {
        // Arrange+
        // Act
        var result = await _sutService.GetSome<UserInteraction>(
            filters: model => model.IsOpen
        );

        // Assert
        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Match(model => model.All(m => m.IsOpen));
    }

    [Fact]
    public async Task GetSome_FilterClosedAndProjectToDto_ReturnsCollectionFilteredByCriterion()
    {
        // Arrange+
        // Act
        IEnumerable<Dto.UserInteractionDto>? result = await _sutService.GetSome(
            projection: model => model.ToDto(),
            filters: model => !model.IsOpen
        );

        // Assert
        using var _ = new AssertionScope();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Match(list => list.All(m => !m.IsOpen));
    }

    [Fact]
    public async Task SetOpenState_CurrentlyOpenInteraction_WasSetToClosed()
    {
        // Arrange+
        var known = _knownEntitesIdIsOpen.Single(k => k.IsOpen);

        // Act
        var (succeed, errors) = await _sutService.SetOpenState(
            known.Id,
            false);

        // Assert
        succeed.Should().BeTrue();
        errors.Should().BeNullOrEmpty();

        using var context = _dbFixture.CreateContext();
        context.UserInteraction.Should().
    }

    [Fact]
    public async Task SetOpenState_NonExistingInteraction_ReturnNotFoundResultWithCorrectError()
    {
        // Arrange
        var unknownId = Guid.NewGuid();

        // Act
        var (succeed, errors) = await _sutService.SetOpenState(unknownId, true);

        // Assert
        using (new AssertionScope())
        {
            succeed.Should().BeFalse();
            errors.Should().NotBeNullOrEmpty();
            errors.Select(error => error.ResultType).Should().Contain(ServiceResultType.NotFound);
        }
    }

    [Fact]
    public async Task CreateNew_NonExisting_ReturnModelFullyInitialized()
    {
        // Arrange+
        UserInteraction correctNewModel = new()
        {
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty"
        };
        var serviceQueryTime = DateTime.Now;

        // Act
        var (succeed, createdModel, errors) = await _sutService.Create(correctNewModel);

        // Assert
        using (new AssertionScope())
        {
            succeed.Should().BeTrue();
            errors.Should().BeNullOrEmpty();
        }
        using (new AssertionScope())
        {
            createdModel.Should().NotBeNull();
            createdModel.Deadline.Should().Be(correctNewModel.Deadline);
            createdModel.Description.Should().Be(correctNewModel.Description);
            createdModel.Created.Should().BeAfter(serviceQueryTime);
            createdModel.IsOpen.Should().BeTrue();
            createdModel.Id.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task CreateNew_ModelAlreadyExists_ShouldNotSucceedAndReturnExistingModelAndError()
    {
        // Arrange+
        UserInteraction attemptedModel = new()
        {
            Id = _knownEntitesIdIsOpen.First().Id,
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty"
        };

        // Act
        var (succeed, existingModel, errors) = await _sutService.Create(attemptedModel);

        // Assert
        using (new AssertionScope())
        {
            succeed.Should().BeFalse();
            errors.Should().ContainSingle(error => error.ResultType == ServiceResultType.AlreadyExists);
        }
        using (new AssertionScope())
        {
            existingModel.Should().NotBeNull();
            existingModel.Deadline.Should().BeAfter(DateTime.MinValue);
            existingModel.Description.Should().NotBeNullOrEmpty();
            existingModel.Created.Should().BeAfter(DateTime.MinValue);
            existingModel.Id.Should().Be(attemptedModel.Id);
        }
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
