using AutoFixture.Xunit2;
using Backend.WebApi.App.ActionResults;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Backend.WebApi.Tests.App.ActionResults;

[Collection("ActionExecutionFixture")]
public sealed class HeaderedStatusCodeResultTests
{
    private readonly ActionContext _actionContext;

    public HeaderedStatusCodeResultTests(ActionExecutionFixture fixture) =>
        _actionContext = fixture.CreateActionContext();

    [Theory]
    [AutoMoqData(true)] // NB! OmitAutoProperty filling of Header auto-prop.
    public void ExecuteResult_StateUnderTest_ExpectedBehavior(
        // Arrange
        [Frozen(Matching.ParameterName)] int statusCode,
        [Frozen(Matching.ParameterName)] KeyValuePair<string, StringValues> header,
        [Frozen(Matching.ParameterName)] KeyValuePair<string, StringValues>[] headers,
        HeaderedStatusCodeResult sutActionResult)
    {
        IEnumerable<KeyValuePair<string, StringValues>>? allHeaders = headers.Prepend(header);

        // Act
        sutActionResult.ExecuteResult(
            _actionContext
            );

        // Assert
        using AssertionScope _ = new();

        _actionContext.HttpContext.Response.StatusCode.Should().Be(statusCode);

        _actionContext.HttpContext.Response.Headers.Should().BeEquivalentTo(allHeaders);
    }
}
