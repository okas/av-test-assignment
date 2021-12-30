using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public class UserInteractionSetOpenStateHandler : IRequestHandler<UserInteractionSetOpenStateCommand, IEnumerable<ServiceError>>
{
    private readonly ApiDbContext _context;

    public UserInteractionSetOpenStateHandler(ApiDbContext context) => _context = context;

    public async Task<IEnumerable<ServiceError>> Handle(
        UserInteractionSetOpenStateCommand request,
        CancellationToken ct)
    {
        _context.Attach(new UserInteraction
        {
            Id = request.Id,
            IsOpen = request.IsOpen,
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync(ct);

            return Enumerable.Empty<ServiceError>();
        }
        catch (DbUpdateConcurrencyException)
        {
            // TODO Log it
            if (await _context.UserInteraction.AnyAsync(model => model.Id == request.Id, ct))
            {
                throw;
            }

            return new ServiceError[] { new(ServiceErrorKind.NotFoundOnChange) };
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            return new ServiceError[] { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };
        }
        catch (Exception ex)
        {
            // TODO Log it
            return new ServiceError[] { new(ServiceErrorKind.InternalError, Exceptions: ex) };
        }
    }
}
