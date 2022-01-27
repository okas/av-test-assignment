using System.ComponentModel.DataAnnotations;
using Backend.WebApi.App.Dto;
using Backend.WebApi.CrossCutting.Logging;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public readonly record struct UserInteractionCreateCommand(
    [property: Required] DateTime Deadline,
    [property: Required, MinLength(2)] string Description)
    : IRequest<UserInteractionDto>
{
    /// <summary>
    /// Handles <see cref="UserInteractionCreateCommand" /> command.
    /// </summary>
    public class Handler : IRequestHandler<UserInteractionCreateCommand, UserInteractionDto>
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(ApiDbContext context, ILogger<Handler> logger) => (_context, _logger) = (context, logger);

        /// <inheritdoc />
        /// <exception cref="AlreadyExistsException" />
        /// <exception cref="DbUpdateConcurrencyException" />
        public async Task<UserInteractionDto> Handle(UserInteractionCreateCommand rq, CancellationToken ct)
        {
            UserInteraction model = new()
            {
                IsOpen = true,
                Created = DateTime.Now,
                Description = rq.Description,
                Deadline = rq.Deadline,
            };
#pragma warning disable MA0042 // Do not use blocking calls in an async method
            _context.UserInteraction.Add(model);
#pragma warning restore MA0042 // Do not use blocking calls in an async method
            try
            {
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                throw new AlreadyExistsException("Operation cancelled.", new { model.Id }, typeof(Handler).FullName!, ex);
            }

            _logger.InformCreated(new { model.Id });

            return UserInteractionDto.Projection.Compile().Invoke(model);
        }
    }
}
