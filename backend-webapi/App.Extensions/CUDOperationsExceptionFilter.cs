using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

/// <summary>
/// Create, Update, Delete domain operations exceptions handling filter.
/// </summary>
public class CUDOperationsExceptionFilter : IExceptionFilter
{
    private readonly ILoggerFactory _loggerFactory;

    public CUDOperationsExceptionFilter(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled || ExceptionHandler(context) is not IActionResult result)
        {
            return;
        }

        context.Result = result;
        context.ExceptionHandled = true;
    }

    private IActionResult? ExceptionHandler(ExceptionContext context) => context.Exception switch
    {
        NotFoundException ex => HandleNotFoundException(ex),
        AlreadyExistsException ex => HandleAlreadyExistsException(ex),
        _ => null
    };

    private NotFoundObjectResult HandleNotFoundException(NotFoundException ex)
    {
        GetLogger(ex).WarnNotFound(ex.Data[BaseException.ModelDataKey]!, ex);

        ProblemDetails detailsObject = GenerateBaseProblemDetails(ex, "Not found.");

        return new(detailsObject);
    }

    private ConflictObjectResult HandleAlreadyExistsException(AlreadyExistsException ex)
    {
        GetLogger(ex).WarnAlreadyExists(ex.Data[BaseException.ModelDataKey]!, ex);

        ProblemDetails detailsObject = GenerateBaseProblemDetails(ex, "Already exists.");

        return new(detailsObject);
    }

    private ILogger GetLogger(BaseException ex) =>
        _loggerFactory.CreateLogger(ex.Category);

    private static ProblemDetails GenerateBaseProblemDetails(BaseException ex, string title)
    {
        ProblemDetails details = new()
        {
            Title = title,
            Detail = ex.Message,
        };

        details.Extensions[BaseException.ModelDataKey] = ex.Data[BaseException.ModelDataKey];

        return details;
    }
}
