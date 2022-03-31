using Backend.WebApi.App.Dto;
using Backend.WebApi.CrossCutting.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Backend.WebApi.App.Filters;

/// <summary>
/// Action result filter, that will compare results <see cref="IETag.ETag"/> value against HTTP header <c>If-None-Match</c> value.
/// <para>If they don't match then will return <see cref="OkObjectResult"/> with data.
/// If they match then result will be of type <see cref="StatusCodeResult"/> with status code <see cref="StatusCodes.Status304NotModified"/>.
/// </para>
/// HTTP header <c>ETAG</c> will be set if there no exceptions havent thrown and action execution is not cancelled.
/// <para>NB! [currently] does not use any caching capabilities.</para>
/// </summary>
public class IfNoneMatchActionFilter : IActionFilter, IAsyncResultFilter
{

    private readonly ILogger<IfNoneMatchActionFilter> _logger;

    public IfNoneMatchActionFilter(ILogger<IfNoneMatchActionFilter> logger) => _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // TODO: compare with cache; short-circuit with HTTP304, if exists 
        _logger.LogDebug("If-None-Match: {}", context.HttpContext.Request.Headers.IfNoneMatch);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null || context.Canceled)
        {
            return;
        }

        if (context.Result is OkObjectResult r && r.Value is IETag tagged)
        {
            context.HttpContext.Response.Headers.ETag = tagged.ETag;

            StringValues ifNoneMatch = context.HttpContext.Request.Headers.IfNoneMatch;

            if (ifNoneMatch.Any() && ifNoneMatch == tagged.ETag)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            }
        }
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next();

        _logger.InformConditionalRqIfNoneMatch(
            HeaderNames.ETag,
            context.HttpContext.Response.Headers.ETag,
            context.HttpContext.Response.StatusCode);
    }
}
