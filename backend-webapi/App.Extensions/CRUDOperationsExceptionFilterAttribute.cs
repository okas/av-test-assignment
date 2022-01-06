using Backend.WebApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

public class CRUDOperationsExceptionFilterAttribute : ReadOperationExceptionFilterAttribute
{
    public new void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            AlreadyExistsException ex => new ConflictObjectResult(GenerateError(ex)),// TODO As long as ID generation is by ORM/DB this isn't appropiate error for user to show.
            _ => null
        };

        if (context.Result is null)
        {
            base.OnException(context);
        }

        if (context.ExceptionHandled)
        {
            return;
        }

        context.ExceptionHandled = context.Result is not null;
    }
}

// TODO "Internal Exceptions" Should be logged in middleware
// TODO Domain level logging should take place in operations or service layer.