using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public record UserInteractionGetByIdQuery(
    [Required, NotDefault] Guid Id
    )
    : IRequest<UserInteraction?>
{
    /// <summary>
    /// Handles <see cref="UserInteractionGetByIdQuery"/> command.
    /// </summary>
    /// <param name="Context">Dependency.</param>
    public record Handler(ApiDbContext Context) : IRequestHandler<UserInteractionGetByIdQuery, UserInteraction?>
    {
        public async Task<UserInteraction?> Handle(UserInteractionGetByIdQuery rq, CancellationToken ct)
        {
            try
            {
                return await Context.UserInteraction
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == rq.Id, ct).ConfigureAwait(false);
            }
            catch
            {
                // TODO Whether and what should be logged here?
                throw;
            }
        }
    }
}
