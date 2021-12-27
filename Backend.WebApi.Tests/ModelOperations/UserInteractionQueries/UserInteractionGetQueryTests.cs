using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Dto;
using Backend.WebApi.ModelOperations.UserInteractionQueries;
using Backend.WebApi.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backend.WebApi.Tests.ModelOperations.UserInteractionQueries;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionGetQueryTests : IDisposable
{
    private static readonly string _becauseKnownOrMoreEntitiesExpected;
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;

    static UserInteractionGetQueryTests()
    {
        _becauseKnownOrMoreEntitiesExpected = $"test data may contain more models and never less than {nameof(_knownEntitesIdIsOpen)}";
    }

    public UserInteractionGetQueryTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = Enumerable.Range(0, 4).Select(i => (Guid.NewGuid(), i % 2 == 0)).ToArray();
        _sutDbContext = dbFixture.CreateContext();
        UserInteractionUtilities.SeedData(dbFixture, _knownEntitesIdIsOpen);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_FilterByIsClosed_ReturnsFilteredCollection(bool isOpenTestValue)
    {
        // Arrange
        UserInteractionGetQuery<UserInteractionDto> query =
            new(UserInteractionDto.Projection, model => model.IsOpen == isOpenTestValue);

        UserInteractionGetHandler<UserInteractionDto> _sutCommandHandler = new(_sutDbContext);

        // Act
        (IEnumerable<ServiceError> errors, IEnumerable<UserInteractionDto>? dtos, int totalCount) =
            await _sutCommandHandler.Handle(
                query
                );

        // Assert
        using (new AssertionScope())
        {
            errors.Should().BeNullOrEmpty();
            dtos.Should().NotBeNullOrEmpty();
            dtos.Should().OnlyContain(d => d.IsOpen == isOpenTestValue);
        };

        totalCount.Should().BeGreaterThanOrEqualTo(
            _knownEntitesIdIsOpen.Length,
            _becauseKnownOrMoreEntitiesExpected
            );
    }

    /// <summary>
    /// Clean up test class level arrangements.
    /// </summary>
    public void Dispose() => _sutDbContext.Dispose();
}
