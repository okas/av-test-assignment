using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

[StructLayout(LayoutKind.Auto)]
public readonly record struct UserInteractionSetOpenStateCommand(
    [property: Required, NotDefault] Guid Id,
    [property: Required] bool IsOpen)
    : IRequest
{
    /// <summary>
    /// Handles <see cref="UserInteractionSetOpenStateCommand" /> command.
    /// </summary>
    public class Handler : IRequestHandler<UserInteractionSetOpenStateCommand>
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(ApiDbContext context, ILogger<Handler> logger) => (_context, _logger) = (context, logger);

        public async Task<Unit> Handle(UserInteractionSetOpenStateCommand rq, CancellationToken ct)
        {
            // TODO Need to study and work out optimal way to handle it. I wish taht either 
            byte[] rowVer = await _context.UserInteraction.AsNoTracking()
                .Where(e => e.Id == rq.Id)
                .Select(e => e.RowVer)
                .SingleOrDefaultAsync(ct).ConfigureAwait(false)
                    ?? throw new NotFoundException("Operation cancelled.", new { rq.Id }, typeof(Handler).FullName!);

            _context.Attach(new UserInteraction // TODO needs revision, because RowVersion is added to entity and it needs to be in context to update.
            {
                Id = rq.Id,
                IsOpen = rq.IsOpen,
                RowVer = rowVer,
            })
            .Property(e => e.IsOpen)
            .IsModified = true;

            try
            {
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // TODO Analyze https://docs.microsoft.com/en-us/ef/core/saving/concurrency to implement better handling
                // TODO tests fail here
                if (!await _context.UserInteraction.AnyAsync(model => model.Id == rq.Id, ct).ConfigureAwait(false))
                {
                    throw new NotFoundException("Operation cancelled.", rq, typeof(Handler).FullName!, ex);
                }
                throw;
            }

            _logger.InformChanged(rq);

            return Unit.Value;
        }
    }
}
