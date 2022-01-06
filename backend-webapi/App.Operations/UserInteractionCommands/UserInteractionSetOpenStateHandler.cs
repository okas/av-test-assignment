using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public class UserInteractionSetOpenStateHandler : IRequestHandler<UserInteractionSetOpenStateCommand>
{
    private readonly ApiDbContext _context;

    public const string NotFoundOnIsOpenChangeMessage = "User interaction not found, while attempting to set its Open state.";

    public UserInteractionSetOpenStateHandler(ApiDbContext context) => _context = context;

    public async Task<Unit> Handle(UserInteractionSetOpenStateCommand rq, CancellationToken ct)
    {
        _context.Attach(new UserInteraction
        {
            Id = rq.Id,
            IsOpen = rq.IsOpen,
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);

            return Unit.Value;
        }
        catch (DbUpdateConcurrencyException)
        {
            // TODO Log it
            // TODO Analyze https://docs.microsoft.com/en-us/ef/core/saving/concurrency to implement better handling
            if (!await _context.UserInteraction.AnyAsync(model => model.Id == rq.Id, ct).ConfigureAwait(false))
            {
                throw new NotFoundException(NotFoundOnIsOpenChangeMessage, rq.Id);
            }
            throw;
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
        }
    }
}
