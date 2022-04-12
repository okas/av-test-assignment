using System.ComponentModel.DataAnnotations;
using Backend.WebApi.App.Dto;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public record UserInteractionGetByIdQuery(
    [Required, NotDefault] Guid Id)
    : IRequest<UserInteractionDto?>
{
    /// <summary>
    /// Handles <see cref="UserInteractionGetByIdQuery"/> command.
    /// </summary>
    public class Handler : IRequestHandler<UserInteractionGetByIdQuery, UserInteractionDto?>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context) => _context = context;

        public async Task<UserInteractionDto?> Handle(UserInteractionGetByIdQuery rq, CancellationToken ct)
        {

            UserInteraction? model = await _context.UserInteraction.AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == rq.Id, ct).ConfigureAwait(false);

            return model is not null
                ? UserInteractionDto.Projection.Compile().Invoke(model)
                : null;
        }
    }
}
