using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.ModelOperations.UserInteractionCommands;

public class UserInteractionCreateHandler : IRequestHandler<UserInteractionCreateCommand, (IEnumerable<ServiceError>? errors, UserInteraction? model)>
{
    private readonly ApiDbContext _context;
    private static readonly string _createNewModelErrorMessage;

    static UserInteractionCreateHandler()
    {
        _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";
    }

    public UserInteractionCreateHandler(ApiDbContext dbContext) => _context = dbContext;

    public async Task<(IEnumerable<ServiceError>? errors, UserInteraction? model)> Handle(
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

    private async Task<(IEnumerable<ServiceError>? errors, UserInteraction? model)> TryCreate(
        UserInteraction newModel, CancellationToken cancellationToken)
    {
        try
        {
            await _context.UserInteraction.AddAsync(newModel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return (default, newModel);
        }
        catch (DbUpdateException ex)
        {
            // TODO Log it
            ServiceError[] serviceErrors = new[] { HandleDbUpdateException(ex) };

            return (serviceErrors, default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            ServiceError[] serviceErrors = new[] { GenerateInternaError(ex) };

            return (serviceErrors, default);
        }
    }

    private static ServiceError HandleDbUpdateException(DbUpdateException dbException)
    {
        switch (dbException.InnerException)
        {
            case SqlException ex when ex.Number == 2627:
                return new(ServiceErrorKind.AlreadyExistsOnCreate);

            default:
                return GenerateInternaError(dbException);
        }
    }

    private static ServiceError GenerateInternaError(Exception ex)
    {
        return new(ServiceErrorKind.InternalError, _createNewModelErrorMessage, ex);
    }
}
