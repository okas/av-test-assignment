using System.ComponentModel.DataAnnotations;
using Backend.WebApi.App.Dto;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public record UserInteractionGetByIdQuery(
    [Required, NotDefault] Guid Id
    )
    : IRequest<UserInteractionDto?>
{
    /// <summary>
    /// Handles <see cref="UserInteractionGetByIdQuery"/> command.
    /// </summary>
    /// <param name="Context">Dependency.</param>
    public record Handler(ApiDbContext Context) : IRequestHandler<UserInteractionGetByIdQuery, UserInteractionDto?> // TODO To class, cause no record features used
    {
        public async Task<UserInteractionDto?> Handle(UserInteractionGetByIdQuery rq, CancellationToken ct)
        {
            try
            {
                UserInteraction? model = await Context.UserInteraction.AsNoTracking()
                    .SingleOrDefaultAsync(m => m.Id == rq.Id, ct).ConfigureAwait(false);

                return model is not null
                    ? UserInteractionDto.Projection.Compile().Invoke(model)
                    : null;
            }
            catch
            {
                // TODO Whether and what should be logged here?
                throw;
            }
        }
    }
}
