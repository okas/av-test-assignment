using Backend.WebApi.App.Cache;
using Backend.WebApi.App.Dto;
using Backend.WebApi.CrossCutting.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Backend.WebApi.App.Filters;

/// <summary>
/// Actionfilter+resultfilter, that will short-circuit by return <see cref="StatusCodeResult"/>
/// with code <see cref="StatusCodes.Status304NotModified"/> if consumer provided HTTP Header <c>If-None-Match</c> has match.
/// <para>
/// Evaluate cache contents first: if has match then uses cached data for desition making.
/// Result value that is retreived from <c>action method</c> will be cached.
/// </para>
/// <para>
/// If HTTP Header <c>If-None-Match="*"</c>, is empty or missing, then it will pass the control to <c>action method</c> for result retreival.
/// </para>
/// </summary>
public class IfNoneMatchFilter : IActionFilter, IAsyncResultFilter
{
    private readonly ICacheService<object> _cache;
    private readonly ILogger<IfNoneMatchFilter> _logger;

    public IfNoneMatchFilter(ICacheService<object> cache, ILogger<IfNoneMatchFilter> logger)
        => (_cache, _logger) = (cache, logger);

    public void OnActionExecuting(ActionExecutingContext context)
    {
        StringValues ifNoneMatchVals = context.HttpContext.Request.Headers.IfNoneMatch;

        if (StringValues.IsNullOrEmpty(ifNoneMatchVals) || ifNoneMatchVals == "*")
        {
            return;
        }

        if (_cache.Get(ifNoneMatchVals).Result is null)
        {
            return;
        }

        context.HttpContext.Response.Headers.ETag = ifNoneMatchVals;

        context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null || context.Canceled)
        {
            return;
        }

        if (context.Result is not OkObjectResult initialResult || initialResult.Value is not IETag tagged)
        {
            return;
        }

        context.HttpContext.Response.Headers.ETag = tagged.ETag;

        _cache.Set(tagged.ETag, initialResult.Value);

        if (context.HttpContext.Request.Headers.IfNoneMatch == tagged.ETag)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
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
