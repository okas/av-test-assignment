using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Backend.WebApi.Tests;

public class ActionExecutionFixture
{
    public ActionContext CreateActionContext() => new(
        new DefaultHttpContext(),
        new RouteData(),
        new ActionDescriptor());

    public ExceptionContext CreateExceptionContext() => new(
        CreateActionContext(),
        Array.Empty<IFilterMetadata>());

    public ActionExecutedContext CreateActionExecutedContext() => new(
        CreateActionContext(),
        Array.Empty<IFilterMetadata>(), controller: default);

}

[CollectionDefinition("ActionExecutionFixture")]
public class ActionFilterFixtureCollection : ICollectionFixture<ActionExecutionFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
