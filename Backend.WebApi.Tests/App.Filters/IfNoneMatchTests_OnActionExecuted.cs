using AutoFixture.Xunit2;
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
    public partial class IfNoneMatchTests_OnActionExecuted : IClassFixture<ActionExecutionFixture>
    {
        private readonly ActionExecutedContext _actionExecutedContext;

        public IfNoneMatchTests_OnActionExecuted(ActionExecutionFixture fixture) => _actionExecutedContext = fixture.CreateActionExecutedContext();


        [Theory]
        [AutoMoqData]
        public void OkObjectResultValueIsIETag_ETagHeaderSetFromResult(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            IfNoneMatchActionFilter sutActionFilter)
        {
            _actionExecutedContext.Result = result;

            // Act
            sutActionFilter.OnActionExecuted(
                _actionExecutedContext
                );

            // Assert
            _actionExecutedContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(Value.ETag);
        }

        [Theory]
        [AutoMoqData]
        public void ExistingModelButETagMisMatch_ProducesOkObjectResultWithValueAndETag(
            // Arrange
            [Frozen(Matching.PropertyName)] ETaggedStub Value,
            OkObjectResult result,
            IfNoneMatchActionFilter sutActionFilter)
        {
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
            IfNoneMatchActionFilter sutActionFilter)
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
           IfNoneMatchActionFilter sutActionFilter)
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
           IfNoneMatchActionFilter sutActionFilter)
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
