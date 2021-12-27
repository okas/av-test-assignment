using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.ModelOperations.UserInteractionCommands;

public class UserInteractionSetOpenStateHandler : IRequestHandler<UserInteractionSetOpenStateCommand, IEnumerable<ServiceError>>
{
    private readonly ApiDbContext _context;

    public UserInteractionSetOpenStateHandler(ApiDbContext context) => _context = context;

    public async Task<IEnumerable<ServiceError>> Handle(
        UserInteractionSetOpenStateCommand request,
        CancellationToken cancellationToken = default)
    {
        _context.Attach(new UserInteraction
        {
            Id = request.Id,
            IsOpen = request.IsOpen
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Enumerable.Empty<ServiceError>();
        }
        catch (DbUpdateConcurrencyException)
        {
            bool isAny = await _context.UserInteraction.AnyAsync(
                model => model.Id == request.Id,
                cancellationToken
                );

            if (!isAny)
            {
                return new ServiceError[] { new(ServiceErrorKind.NotFoundOnChange) };
            }
            else
            {
                // TODO Log
                // HACK
                throw;
            }
        }
        catch (Exception ex)
        {
            return new ServiceError[] { new(ServiceErrorKind.InternalError, Exceptions: ex) };
        }
    }
}
