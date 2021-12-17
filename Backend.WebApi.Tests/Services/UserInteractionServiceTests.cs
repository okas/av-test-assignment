using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
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
    private static readonly string _becauseKnownOrMoreEntitiesExpected;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionService _sutService;
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContextLocalDbFixture _dbFixture;

    static UserInteractionServiceTests()
    {
        _becauseKnownOrMoreEntitiesExpected = "test data may contain more models, but never less than known entities";
    }

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
    public async Task GetSome_FilterOpenWithoutProjection_ReturnsCollectionFilteredByCriterion()
    {
        // Arrange
        Expression<Func<UserInteraction, bool>> filters = model => model.IsOpen;

        // Act
        (IEnumerable<ServiceError>? errors, IList<UserInteraction>? models, int totalCount) =
            await _sutService.GetSome<UserInteraction>(
                filters: filters
                );

        // Assert
        using (AssertionScope _ = new())
        {
            errors.Should().BeNullOrEmpty();
            models.Should().NotBeNullOrEmpty();
            models.Should().Match(model => model.All(m => m.IsOpen));
        }
        totalCount.Should().BeGreaterThanOrEqualTo(
            _knownEntitesIdIsOpen.Length,
            _becauseKnownOrMoreEntitiesExpected
            );
    }

    [Fact]
    public async Task GetSome_FilterClosedWithoutProjection_ReturnsCollectionFilteredByCriterion()
    {
        // Arrange
        Expression<Func<UserInteraction, UserInteractionDto>> projection = model => model.ToDto();
        Expression<Func<UserInteraction, bool>> filters = model => !model.IsOpen;

        // Act
        (IEnumerable<ServiceError>? errors, IList<UserInteractionDto>? models, int totalCount) =
            await _sutService.GetSome(
                projection: projection,
                filters: filters
                );

        // Assert
        using (AssertionScope _ = new())
        {
            errors.Should().BeNullOrEmpty();
            models.Should().NotBeNullOrEmpty();
            models.Should().Match(list => list.All(m => !m.IsOpen));
        };
        totalCount.Should().BeGreaterThanOrEqualTo(
            _knownEntitesIdIsOpen.Length,
            _becauseKnownOrMoreEntitiesExpected
            );
    }

    [Fact]
    public async Task GetSome_ProjectToDto_ReturnsCollectionProjectedToDtoType()
    {
        // Arrange+
        Expression<Func<UserInteraction, Dto.UserInteractionDto>> projection = model => model.ToDto();

        // Act
        (IEnumerable<ServiceError>? errors, IList<UserInteractionDto>? modelsDto, int totalCount) =
            await _sutService.GetSome(
                projection: projection
                );

        // Assert
        using (AssertionScope _ = new())
        {
            errors.Should().BeNullOrEmpty();
            modelsDto.Should().NotBeNullOrEmpty();
        }
        totalCount.Should().BeGreaterThanOrEqualTo(
            _knownEntitesIdIsOpen.Length,
            _becauseKnownOrMoreEntitiesExpected
            );
    }

    [Fact]
    public async Task SetOpenState_CurrentlyOpenInteraction_WasSetToClosed()
    {
        // Arrange+
        var (id, _) = _knownEntitesIdIsOpen.First(k => k.IsOpen);

        // Act
        var errors =
            await _sutService.SetOpenState(
                id,
                false
                );

        // Assert
        errors.Should().BeNullOrEmpty();

        using var context = _dbFixture.CreateContext();
        context.UserInteraction.Should().Contain(model => !model.IsOpen);
    }

    [Fact]
    public async Task SetOpenState_NonExistingInteraction_ReturnNotFoundResultWithCorrectError()
    {
        // Arrange+
        var unknownId = Guid.NewGuid();

        // Act
        var errors =
            await _sutService.SetOpenState(
                unknownId,
                true
                );

        // Assert
        errors.Should().NotBeNullOrEmpty();
        errors.Select(error => error.ResultType).Should().Contain(ServiceResultType.NotFoundOnChange);
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
        var (errors, createdModel) =
            await _sutService.Create(
                correctNewModel
                );

        // Assert
        errors.Should().BeNullOrEmpty();
        using AssertionScope _ = new();
        createdModel.Should().NotBeNull();
        createdModel.Deadline.Should().Be(correctNewModel.Deadline);
        createdModel.Description.Should().Be(correctNewModel.Description);
        createdModel.Created.Should().BeAfter(serviceQueryTime);
        createdModel.IsOpen.Should().BeTrue();
        createdModel.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateNew_ModelAlreadyExists_ReturnsErrorOfAlreadyExistingAndNullModel()
    {
        // Arrange+
        UserInteraction attemptedModel = new()
        {
            Id = _knownEntitesIdIsOpen.First().Id,
            Deadline = DateTime.Now.AddDays(1),
            Description = "Non-empty"
        };

        // Act
        var (errors, existingModel) =
            await _sutService.Create(
                attemptedModel
                );

        // Assert
        errors.Should().ContainSingle(error => error.ResultType == ServiceResultType.AlreadyExistsOnCreate);
        existingModel.Should().BeNull();
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
