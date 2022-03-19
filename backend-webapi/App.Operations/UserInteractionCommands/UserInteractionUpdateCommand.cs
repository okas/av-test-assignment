using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;

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
    public class Handler : ConcurrencyHandlerBase, IRequestHandler<UserInteractionUpdateCommand, Unit>
    {
        private static readonly string _logCategory;
        private readonly ILogger<Handler> _logger;

        static Handler() => _logCategory = typeof(Handler).FullName!;

        public Handler(ApiDbContext context, ILogger<Handler> logger) : base(context)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        /// <inheritdoc cref="ConcurrencyHandlerBase.SaveAndHandleExceptions" />
        public async Task<Unit> Handle(UserInteractionUpdateCommand rq, CancellationToken ct)
        {
            UserInteraction entity = await _context.UserInteraction.FindAsync(new object[] { rq.Id }, ct)
                .ConfigureAwait(false)
                ?? throw new NotFoundException("Update operation cancelled.", new { rq.Id }, _logCategory);

            _context.Entry(entity).CurrentValues.SetValues(rq);

            await base.SaveAndHandleExceptions(entity, _logCategory, ct).ConfigureAwait(false);

            _logger.InformUpdated(new { entity.Id });

            return Unit.Value;
        }

    }
}
