using AutoFixture.Xunit2;
using Backend.WebApi.App.Filters;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace Backend.WebApi.Tests.App.Extensions
{
    [Collection("ActionFilterFixture")]
    public class IfNoneMatchTests
    {
        private readonly ActionExecutedContext _actionFilterContext;

        public IfNoneMatchTests(ActionFilterFixture fixture) =>
            _actionFilterContext = fixture.CreateActionExecutedContext();

        [Theory]
        [AutoMoqData]
        public void OnActionExecuting_OkObjectResultValueIsIETag_ETagHeaderSetFromResult(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            IfNoneMatchActionFilter sutActionFilter)
        {
            _actionFilterContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionFilterContext
                );

            // Assert
            _actionFilterContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(Value.ETag);
        }

        [Theory]
        [AutoMoqData]
        public void OnActionExecuting_ExistingModelButETagMisMatch_ProducesOkObjectResultWithValueAndETag(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            IfNoneMatchActionFilter sutActionFilter)
        {
            _actionFilterContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionFilterContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionFilterContext.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(Value);

            _actionFilterContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(Value.ETag);
        }

        [Theory]
        [AutoMoqData]
        public void OnActionExecuting_ExistingETagInRequest_ProducesStatusCodeResult304(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            IfNoneMatchActionFilter sutActionFilter)
        {
            _actionFilterContext.HttpContext.Request.Headers.IfNoneMatch = Value.ETag;
            _actionFilterContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionFilterContext
                );

            // Assert
            _actionFilterContext.Result.Should().BeOfType<StatusCodeResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
        }

        [Theory]
        [AutoMoqData]
        public void OnActionExecuting_ExeptionThrownOnActionExecution_ResultIsUnChangedAndETagNotSet(
           // Arrange
           IActionResult result,
           Exception ex,
           IfNoneMatchActionFilter sutActionFilter)
        {
            _actionFilterContext.Exception = ex;
            _actionFilterContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionFilterContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionFilterContext.Result.Should().BeSameAs(result);

            _actionFilterContext.HttpContext.Response.Headers.ETag.Should().BeEmpty();
        }

        [Theory]
        [AutoMoqData]
        public void OnActionExecuting_ShortCircuited_ResultIsUnChangedAndETagNotSet(
           // Arrange
           IActionResult result,
           IfNoneMatchActionFilter sutActionFilter)
        {
            _actionFilterContext.Canceled = true;
            _actionFilterContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionFilterContext
                );

            // Assert
            using AssertionScope _ = new();

            _actionFilterContext.Result.Should().BeSameAs(result);

            _actionFilterContext.HttpContext.Response.Headers.ETag.Should().BeEmpty();
        }
    }
}
