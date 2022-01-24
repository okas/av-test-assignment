using Backend.WebApi.CrossCutting.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

public class OperationCancelledExceptionFilter : IExceptionFilter
{
    private readonly Logger<OperationCancelledExceptionFilter> _logger;

    public OperationCancelledExceptionFilter(Logger<OperationCancelledExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not OperationCanceledException ocEx || !ocEx.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.WarnOperationCancelledWithReason("request aborted", ocEx);

        context.ExceptionHandled = true;
        context.Result = new StatusCodeResult(499);
    }
}
