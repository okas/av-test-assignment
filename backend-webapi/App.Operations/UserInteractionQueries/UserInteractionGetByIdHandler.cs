using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public class UserInteractionGetByIdHandler : IRequestHandler<UserInteractionGetByIdQuery, (IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    private readonly ApiDbContext _context;

    public UserInteractionGetByIdHandler(ApiDbContext context) => _context = context;

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Handle(
        UserInteractionGetByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            UserInteraction? foundModel = await _context.UserInteraction
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, ct);

            return (Enumerable.Empty<ServiceError>(), model: foundModel);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, model: default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.InternalError, Exceptions: ex) };

            return (errors, model: default);
        }
    }
}
