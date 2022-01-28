using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        /// <inheritdoc cref="GetModelAsync"/>
        /// <inheritdoc cref="TrySaveAsync"/>
        public async Task<Unit> Handle(UserInteractionUpdateCommand rq, CancellationToken ct)
        {
            UserInteraction model = await GetModelAsync(rq, ct).ConfigureAwait(false);

            EntityEntry<UserInteraction> entry = _context.Entry(model);

            entry.CurrentValues.SetValues(rq);

            await TrySaveAsync(entry, ct).ConfigureAwait(false);

            _logger.InformUpdated(new { model.Id });

            return Unit.Value;
        }

        /// <exception cref="NotFoundException" />
        private async Task<UserInteraction> GetModelAsync(UserInteractionUpdateCommand rq, CancellationToken ct) =>
            await _context.UserInteraction.FindAsync(new object[] { rq.Id }, ct).ConfigureAwait(false)
                ?? throw new NotFoundException("Update operation cancelled.", new { rq.Id }, _logCategory);

        /// <exception cref="NotFoundException" />
        /// <exception cref="ConcurrentUpdateException" />
        /// <exception cref="OperationCanceledException" />
        /// <exception cref="DbUpdateException" />
        private async Task TrySaveAsync(EntityEntry<UserInteraction> entry, CancellationToken ct)
        {
            try
            {
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                const string message = "Concurrency detected, operation cancelled.";
                var model = new { entry.Entity.Id };

                throw await _context.UserInteraction.AnyAsync(e => e.Id == entry.Entity.Id, CancellationToken.None).ConfigureAwait(false)
                    ? new ConcurrentUpdateException(message, model, _logCategory)
                    : new NotFoundException(message, model, _logCategory);
            }
        }
    }
}
