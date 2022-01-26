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
        /// <exception cref="NotFoundException" />
        /// <exception cref="DbUpdateConcurrencyException" />
        public async Task<Unit> Handle(UserInteractionUpdateCommand rq, CancellationToken ct)
        {
            UserInteraction model = await _context.UserInteraction
                .FindAsync(new object?[] { rq.Id }, ct).ConfigureAwait(false)
                ?? throw new NotFoundException(
                    "Operation cancelled.",
                    new { rq.Id },
                    _logCategory);

            EntityEntry<UserInteraction> entry = _context.Entry(model);

            entry.CurrentValues.SetValues(rq);

            await TrySaveChanges(entry, ct).ConfigureAwait(false); // TODO Add cancellation to limit concurrency handling retries. With logging!

            _logger.InformUpdated(new { model.Id });

            return Unit.Value;
        }

        private async ValueTask TrySaveChanges(EntityEntry<UserInteraction> entry, CancellationToken ct)
        {
            int retry = 1;// TODO or from param if recursion is opted-out.

            try
            {
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.WarnOptimisticConcurrencyDetection("forcing request updates again to be saved", retry++, entry.Entity, ex);

                PropertyValues databaseValues = await entry.GetDatabaseValuesAsync(ct).ConfigureAwait(false)
                    ?? throw new NotFoundException(
                        "Operation cancelled, during concurrency handling.",
                        new { entry.Entity.Id },
                        _logCategory,
                        ex);

                entry.OriginalValues.SetValues(databaseValues);

                await TrySaveChanges(entry, ct).ConfigureAwait(false);
            }
        }
    }
}
