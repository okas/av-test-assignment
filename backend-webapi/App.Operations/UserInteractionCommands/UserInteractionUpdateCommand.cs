using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public readonly record struct UserInteractionUpdateCommand(
    [property: NotDefault] Guid Id,
    [property: Required, NotDefault] DateTime Deadline,
    [property: Required, MinLength(2)] string? Description,
    bool IsOpen)
    : IRequest
{
    /// <summary>
    /// Handles <see cref="UserInteractionUpdateCommand" /> command.
    /// </summary>
    public class Handler : IRequestHandler<UserInteractionUpdateCommand>
    {
        private static readonly string _logCategory;
        private readonly ApiDbContext _context;
        private readonly ILogger<Handler> _logger;

        static Handler() => _logCategory = typeof(Handler).FullName!;

        public Handler(ApiDbContext context, ILogger<Handler> logger) => (_context, _logger) = (context, logger);

        /// <inheritdoc />
        /// <exception cref="NotFoundException" />
        /// <exception cref="ConcurrentUpdateException" />
        /// <exception cref="OperationCanceledException" />
        /// <exception cref="DbUpdateException" />
        public async Task<Unit> Handle(UserInteractionUpdateCommand rq, CancellationToken ct)
        {
            UserInteraction entity = await _context.UserInteraction.FindAsync(new object[] { rq.Id }, ct).ConfigureAwait(false)
                ?? throw new NotFoundException("Update operation cancelled.", new { rq.Id }, _logCategory);

            _context.Entry(entity).CurrentValues.SetValues(rq);

            try
            {
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                const string message = "Concurrency detected, operation cancelled.";
                var model = new { entity.Id };

                throw await _context.UserInteraction.AnyAsync(e => e.Id == entity.Id, CancellationToken.None).ConfigureAwait(false)
                    ? new ConcurrentUpdateException(message, model, _logCategory)
                    : new NotFoundException(message, model, _logCategory);
            }

            _logger.InformUpdated(new { entity.Id });

            return Unit.Value;
        }
    }
}
