using AutoFixture.Xunit2;
using Backend.WebApi.App.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Filters;
public partial class IfNoneMatchTests_OnActionExecuting : IClassFixture<ActionExecutionFixture>
{
    private readonly ActionExecutingContext _actionExecutingContext;

    public IfNoneMatchTests_OnActionExecuting(ActionExecutionFixture fixture) => _actionExecutingContext = fixture.CreateActionExecutingContext();

    [Theory]
    [AutoMoqData]
    public void HeaderIsNotProvided_ShouldNotShortCircuit(
    // Arrange
    IfNoneMatchActionFilter sutActionFilter)
    {
        // Act
        sutActionFilter.OnActionExecuting(
            _actionExecutingContext
            );

        // Assert
        _actionExecutingContext.Should().NotBeShortCircuited();
    }

    [Theory]
    [AutoMoqData]
    public void HeaderIsAsterisk_ShouldNotShortCircuit(
        // Arrange
        IfNoneMatchActionFilter sutActionFilter)
    {
        _actionExecutingContext.HttpContext.Request.Headers.IfNoneMatch = "*";

        // Act
        sutActionFilter.OnActionExecuting(
            _actionExecutingContext
            );

        // Assert
        _actionExecutingContext.Should().NotBeShortCircuited();
    }

    [Theory]
    [AutoMoqData(false)]
    public void HeaderETagIsInCache_ShouldShortCircuit(
        // Arrange
        [Frozen(Matching.PropertyName)] string ETag,
        IfNoneMatchActionFilter sutActionFilter)
    {

        _actionExecutingContext.HttpContext.Request.Headers.IfNoneMatch = ETag;

        // Act
        sutActionFilter.OnActionExecuting(
            _actionExecutingContext
            );

        // Assert
        _actionExecutingContext.Should().BeShortCircuited(ETag);
    }
}
