using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Infrastructure.Data.EF;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Operations.UserInteractionQueries;

[Collection("ApiLocalDbFixture")]
public sealed class UserInteractionGetQueryTests : IDisposable
{
    private static readonly string _becauseKnownOrMoreEntitiesExpected;
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly ApiDbContext _sutDbContext;
    private readonly UserInteractionGetQuery<UserInteractionDto>.Handler _sutCommandHandler;

    static UserInteractionGetQueryTests()
    {
        _becauseKnownOrMoreEntitiesExpected = $"test data may contain more models and never less than {nameof(_knownEntitesIdIsOpen)}";
    }

    public UserInteractionGetQueryTests(ApiLocalDbFixture dbFixture)
    {
        _knownEntitesIdIsOpen = GenerateWithKnownIdIsOpen(4);
        _sutDbContext = dbFixture.CreateContext();
        SeedData(dbFixture, _knownEntitesIdIsOpen);
        _sutCommandHandler = new(_sutDbContext);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_FilterByIsClosed_ReturnsFilteredCollection(bool isOpenTestValue)
    {
        // Arrange
        UserInteractionGetQuery<UserInteractionDto> query = new(
            UserInteractionDto.Projection,
            model => model.IsOpen == isOpenTestValue
            );

        // Act
        (IEnumerable<UserInteractionDto> dtos, int totalCount) =
            await _sutCommandHandler.Handle(
                query,
                ct: default
                );

        // Assert
        using (new AssertionScope())
        {
            dtos.Should().NotBeNullOrEmpty()
                .And.OnlyContain(d => d.IsOpen == isOpenTestValue);
        }

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
