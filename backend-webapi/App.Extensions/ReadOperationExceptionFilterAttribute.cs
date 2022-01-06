using Backend.WebApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.WebApi.App.Extensions;

public class ReadOperationExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException ex => new NotFoundObjectResult(GenerateError(ex)),
            _ => null
        };

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