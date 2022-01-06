using System.ComponentModel.DataAnnotations;
using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public readonly record struct UserInteractionCreateCommand(
    [property: Required] DateTime Deadline,
    [property: Required, MinLength(2)] string? Description
    )
    : IRequest<UserInteraction>
{
    public class Handler : IRequestHandler<UserInteractionCreateCommand, UserInteraction>
    {
        private readonly ApiDbContext _context;
        private static readonly string _createNewModelErrorMessage;

        public const string AlreadyExists = "User interaction already exists.";

        static Handler() =>
            _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";

        public Handler(ApiDbContext dbContext) => _context = dbContext;

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

                return model;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                // TODO Log it
                // TODO Current workflow, where entity instance is created in this handler, should exclude this situation.
                // As of now it should be thrown when Id generation outside of server fails somehow (e.g. default value is attempted).
                throw new AlreadyExistsException(AlreadyExists, model.Id, ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO Whether and what should be logged here?
                throw;
            }
            catch
            {
                // TODO Whether and what should be logged here?
                throw;
            }
        }
    }
}