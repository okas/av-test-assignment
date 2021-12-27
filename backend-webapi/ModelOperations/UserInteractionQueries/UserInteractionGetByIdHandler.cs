using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.ModelOperations.UserInteractionQueries;

public class UserInteractionGetByIdHandler : IRequestHandler<UserInteractionGetByIdQuery, (IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    private readonly ApiDbContext _context;

    public UserInteractionGetByIdHandler(ApiDbContext context) => _context = context;

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Handle(
        UserInteractionGetByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            UserInteraction? foundModel = await _context.UserInteraction.AsNoTracking()
                .FirstOrDefaultAsync(
                    model => model.Id == request.Id,
                    cancellationToken
                    );

            return (Enumerable.Empty<ServiceError>(), foundModel);
        }
        catch (Exception ex)
        {
            ServiceError[] errors = { new(ServiceErrorKind.InternalError, Exceptions: ex) };

            return (errors, default);
        }
    }
}
