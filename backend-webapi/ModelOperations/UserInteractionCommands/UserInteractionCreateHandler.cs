using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.ModelOperations.UserInteractionCommands;

public class UserInteractionCreateHandler : IRequestHandler<UserInteractionCreateCommand, (IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    private readonly ApiDbContext _context;
    private static readonly string _createNewModelErrorMessage;

    static UserInteractionCreateHandler()
    {
        _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";
    }

    public UserInteractionCreateHandler(ApiDbContext dbContext) => _context = dbContext;

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Handle(
        UserInteractionCreateCommand request,
        CancellationToken cancellationToken = default)
    {
        UserInteraction model = new()
        {
            IsOpen = true,
            Created = DateTime.Now,
            Description = request.Description,
            Deadline = request.Deadline,
        };

        return await TryCreate(model, cancellationToken);
    }

    private async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> TryCreate(
        UserInteraction model, CancellationToken cancellationToken)
    {
        IEnumerable<ServiceError> errors;

        try
        {
            await _context.UserInteraction.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return (Enumerable.Empty<ServiceError>(), model);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // TODO Log it
            errors = new ServiceError[] { new(ServiceErrorKind.AlreadyExistsOnCreate) };

            return (errors, default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            errors = new ServiceError[] { new(ServiceErrorKind.InternalError, _createNewModelErrorMessage, ex) };

            return (errors, default);
        }
    }
}
