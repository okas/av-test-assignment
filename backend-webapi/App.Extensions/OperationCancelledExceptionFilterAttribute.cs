using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

public class OperationCancelledExceptionFilterAttribute : ExceptionFilterAttribute
{
    //private readonly ILogger<OperationCancelledExceptionFilterAttribute> _logger;

    //public OperationCancelledExceptionFilterAttribute(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger<OperationCancelledExceptionFilterAttribute>();

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not OperationCanceledException ocEx || !ocEx.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        //_logger.LogWarning(ocEx, "Operation cancellation was requested.");

        context.ExceptionHandled = true;
        context.Result = new StatusCodeResult(499);
    }
}


// TODO "Internal Exceptions" Should be logged in middleware
// TODO Domain level logging should take place in operations or service layer.