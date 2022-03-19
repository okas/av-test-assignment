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
    public class Handler : ConcurrencyHandlerBase, IRequestHandler<UserInteractionSetOpenStateCommand>
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
        public async Task<Unit> Handle(UserInteractionSetOpenStateCommand rq, CancellationToken ct)
        {
            // TODO Need to study and work out optimal way to handle it. I wish taht either 
            byte[] rowVer = await _context.UserInteraction.AsNoTracking()
                .Where(e => e.Id == rq.Id)
                .Select(e => e.RowVer)
                .SingleOrDefaultAsync(ct).ConfigureAwait(false)
                    ?? throw new NotFoundException("Operation cancelled.", new { rq.Id }, _logCategory);

            UserInteraction entity = new() // TODO needs revision, because RowVersion is added to entity and it needs to be in context to update.
            {
                Id = rq.Id,
                IsOpen = rq.IsOpen,
                RowVer = rowVer,
            };

            _context.Attach(entity).Property(e => e.IsOpen).IsModified = true;

            await base.SaveAndHandleExceptions(entity, _logCategory, ct).ConfigureAwait(false);

            _logger.InformChanged(rq);

            return Unit.Value;
        }
    }
}
