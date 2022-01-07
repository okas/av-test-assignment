using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Backend.WebApi.Tests;

public class ActionFilterFixture
{
    public ActionContext ActionContext => new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());

    public ExceptionContext CreateExceptionContext() => new(ActionContext, Array.Empty<IFilterMetadata>());
}
