using AutoFixture.Xunit2;
using Backend.WebApi.App.Cache;
using Backend.WebApi.App.Filters;
using Backend.WebApi.Tests.App.Extensions;
using Backend.WebApi.Tests.App.Filters;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Filters
{
    public partial class HttpConditionalRequestFilterTests_OnActionExecuted : IClassFixture<ActionExecutionFixture>
    {
        private readonly ActionExecutedContext _actionExecutedContext;

        public HttpConditionalRequestFilterTests_OnActionExecuted(ActionExecutionFixture fixture) => _actionExecutedContext = fixture.CreateActionExecutedContext();


        [Theory]
        [AutoMoqDataConditionalHttpRequest]
        public void OkObjectResultValueIs_IETag_ShouldSetETagHeaderAndCacheValue(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            [Frozen] ICacheService<object> cache,
            HttpConditionalRequestFilter sutActionFilter)
        {
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionExecutedContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(Value.ETag);

            cache.Get(Value.ETag).Result.Should().BeEquivalentTo(result.Value);
        }

        [Theory]
        [AutoMoqData]
        public void ExistingModelButETagMisMatch_ProducesOkObjectResultWithValueAndCorrectETag(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            HttpConditionalRequestFilter sutActionFilter,
            string mismatchedETag)
        {
            _actionExecutedContext.HttpContext.Request.Headers.IfNoneMatch = mismatchedETag;
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionExecutedContext.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(Value);

            _actionExecutedContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(Value.ETag);
        }

        [Theory]
        [AutoMoqData]
        public void ExistingETagInRequest_ProducesStatusCodeResult304(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            HttpConditionalRequestFilter sutActionFilter)
        {
            _actionExecutedContext.HttpContext.Request.Headers.IfNoneMatch = Value.ETag;
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            _actionExecutedContext.Result.Should().BeStatusCodeResultHttp304();
        }

        [Theory]
        [AutoMoqData]
        public void ExeptionThrownOnActionExecution_ResultIsUnChangedAndETagNotSet(
           // Arrange
           IActionResult result,
           Exception ex,
           HttpConditionalRequestFilter sutActionFilter)
        {
            _actionExecutedContext.Exception = ex;
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionExecutedContext.Result.Should().BeSameAs(result);

            _actionExecutedContext.HttpContext.Response.Headers.ETag.Should().BeEmpty();
        }

        [Theory]
        [AutoMoqData]
        public void ShortCircuited_ResultIsUnChangedAndETagNotSet(
           // Arrange
           IActionResult result,
           HttpConditionalRequestFilter sutActionFilter)
        {
            _actionExecutedContext.Canceled = true;
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionExecutedContext.Result.Should().BeSameAs(result);

            _actionExecutedContext.HttpContext.Response.Headers.ETag.Should().BeEmpty();
        }
    }
}
