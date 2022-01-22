using System.ComponentModel.DataAnnotations;
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
    [property: Required, MinLength(2)] string? Description)
    : IRequest<UserInteraction> // TODO Respond with DTO, instead of Domain.Model.
{
    /// <summary>
    /// Handles <see cref="UserInteractionCreateCommand" /> command.
    /// </summary>
    public class Handler : IRequestHandler<UserInteractionCreateCommand, UserInteraction>
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(ApiDbContext context, ILogger<Handler> logger) => (_context, _logger) = (context, logger);

        /// <inheritdoc />
        /// <exception cref="AlreadyExistsException" />
        /// <exception cref="DbUpdateConcurrencyException" />
        public async Task<UserInteraction> Handle(UserInteractionCreateCommand rq, CancellationToken ct)
        {
            UserInteraction model = new()
            {
                IsOpen = true,
                Created = DateTime.Now,
                Description = rq.Description,
                Deadline = rq.Deadline,
            };

            try
            {
                await _context.UserInteraction.AddAsync(model, ct).ConfigureAwait(false); // TODO For single entity overhead is not justified.
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);

                _logger.InformCreated(new { model.Id });

                return model;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                // TODO Log it
                // TODO Current workflow, where entity instance is created in this handler, should exclude this situation.
                // As of now it should be thrown when Id generation outside of server fails somehow (e.g. default value is attempted).
                throw new AlreadyExistsException("Operation cancelled.", new { model.Id }, typeof(Handler).FullName!, ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO This exception is not expected in current operation, but is a reminder to implement middleware to handle this kind of exceptions.
                throw;
            }
        }
    }
}
