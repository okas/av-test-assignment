using Backend.WebApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

/// <summary>
/// Create, Update, Delete domain operations exceptions handling filter.
/// </summary>
public class CUDOperationsExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException ex => new NotFoundObjectResult(GenerateError(ex)),
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

    protected static object GenerateError(BaseException ex) =>
       new
       {
           Id = new
           {
               Value = ex.Data["Id"],
               Error = ex.Message,
           },
       };
}

// TODO "Internal Exceptions" Should be logged in middleware
// TODO Domain level logging should take place in operations or service layer.