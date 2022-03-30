using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Backend.WebApi.App.ActionResults;

/// <summary>
/// Takes in HTTP status code 1..* headers, but will not set the HTTP content.
/// Renders <see cref="HttpResponse"/> without setting the <see cref="HttpResponse.Body"/>.
/// </summary>
public class HeaderedStatusCodeResult : ActionResult, IStatusCodeActionResult
{
    /// <param name="statusCode">Recommended: use <see cref="StatusCodes"/>.</param>
    /// <param name="header">Recommended: use <see cref="HeaderNames"/>.</param>
    /// <param name="headers">Recommended: use <see cref="HeaderNames"/>.</param>
    public HeaderedStatusCodeResult([ActionResultStatusCode] int statusCode,
        KeyValuePair<string, StringValues> header,
        params KeyValuePair<string, StringValues>[] headers)
    {
        StatusCode = statusCode;

        IEnumerable<KeyValuePair<string, StringValues>> mergedHeaders = headers.Prepend(header);

        Headers = new HeaderDictionary(new Dictionary<string, StringValues>(mergedHeaders, StringComparer.Ordinal));
    }

    public int? StatusCode { get; init; }

    public IHeaderDictionary Headers { get; init; }

    public override void ExecuteResult(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.HttpContext.Response.StatusCode = StatusCode!.Value;

        foreach (KeyValuePair<string, StringValues> header in Headers)
        {
            context.HttpContext.Response.Headers.Add(header);
        }
    }
}
