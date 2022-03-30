using Backend.WebApi.App.ActionResults;
using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Operations.UserInteractionCommands;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionOpenStateChangeTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly UserInteractionKnownTestData _knownSingle;
    private readonly UserInteractionsController _sutController;

    // Arrange for tests
    public UserInteractionOpenStateChangeTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _knownSingle = SeedDataGenerateAndReturnKnown(fixture, (Guid.NewGuid(), IsOpen: true))[0];
        _sutController = fixture.ScopedServiceProvider!.GetService<UserInteractionsController>()!;
    }

    [Fact]
    public async Task Patch_CanMarkInteractionClosed_ReturnNoContentResultAndModelEntityIsClosedEqFalse()
    {
        // Arrange
        UserInteractionSetOpenStateCommand command = new(
            _knownSingle.Id,
            IsOpen: false,
            RowVer: Array.Empty<byte>()
        );

        // Act
        IActionResult response =
            await _sutController.PatchUserInteraction(
                _knownSingle.Id,
                _knownSingle.RowVer,
                command,
                ct: default
                );

        // Assert
        using AssertionScope _ = new();

        response.Should().NotBeNull().And.BeOfType<HeaderedStatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        response.As<HeaderedStatusCodeResult>()
             .Headers.Should().ContainKey(HeaderNames.ETag);
    }
}
