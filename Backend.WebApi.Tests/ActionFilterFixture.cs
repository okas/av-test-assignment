using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Backend.WebApi.Tests;

public class ActionFilterFixture
{
    public ActionContext ActionContext { get; init; } = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());

    public ExceptionContext CreateExceptionContext() => new(ActionContext, Array.Empty<IFilterMetadata>());
}

[CollectionDefinition("ActionFilterFixture")]
public class ActionFilterFixtureCollection : ICollectionFixture<ActionFilterFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
