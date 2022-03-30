using System.Runtime.Serialization;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public readonly record struct UserInteractionSetOpenStateCommand(
    [property: NotDefault] Guid Id,
    bool IsOpen,
    [property: /* not working on record */ BindNever, /* not working on record */ValidateNever, IgnoreDataMember] byte[] RowVer)
    : IRequest<byte[]>
{
    /// <summary>
    /// Handles <see cref="UserInteractionSetOpenStateCommand" /> command.
    /// </summary>
    public class Handler : ConcurrencyHandlerBase, IRequestHandler<UserInteractionSetOpenStateCommand, byte[]>
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
        public async Task<byte[]> Handle(UserInteractionSetOpenStateCommand rq, CancellationToken ct)
        {
            UserInteraction entity = new()
            {
                Id = rq.Id,
                IsOpen = rq.IsOpen,
                RowVer = rq.RowVer,
            };

            base._context.Attach(entity).Property(e => e.IsOpen).IsModified = true;

            await base.SaveAndHandleExceptions(entity, _logCategory, ct).ConfigureAwait(false);

            _logger.InformChanged(rq);

            return entity.RowVer;
        }
    }
}
