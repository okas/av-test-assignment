using AutoFixture.Xunit2;
using Backend.WebApi.App.Cache;
using Backend.WebApi.App.Dto;
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
    IfNoneMatchFilter sutActionFilter)
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
        IfNoneMatchFilter sutActionFilter)
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
    [AutoMoqDataConditionalHttpRequest(false)]
    public void HeaderETagIsInCache_ShouldShortCircuit(
        // Arrange
        [Frozen(Matching.PropertyName)] string ETag,
        [Frozen] UserInteractionDto dto,
        [Frozen] ICacheService<object> cache,
        IfNoneMatchFilter sutActionFilter)
    {
        cache.Set(ETag, dto);
        _actionExecutingContext.HttpContext.Request.Headers.IfNoneMatch = ETag;

        // Act
        sutActionFilter.OnActionExecuting(
            _actionExecutingContext
            );

        // Assert
        _actionExecutingContext.Should().BeShortCircuited(ETag);
    }

    [Theory]
    [AutoMoqDataConditionalHttpRequest(false)]
    public void HeaderETagNotInCache_ShouldNotShortCircuit(
        // Arrange
        [Frozen(Matching.PropertyName)] string ETag,
        [Frozen] UserInteractionDto dto,
        [Frozen] ICacheService<object> cache,
        IfNoneMatchFilter sutActionFilter,
        string nonExistingETag)
    {
        cache.Set(ETag, dto);
        _actionExecutingContext.HttpContext.Request.Headers.IfNoneMatch = nonExistingETag;

        // Act
        sutActionFilter.OnActionExecuting(
            _actionExecutingContext
            );

        // Assert
        _actionExecutingContext.Should().NotBeShortCircuited();
    }
}
