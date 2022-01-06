using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public class UserInteractionGetByIdHandler : IRequestHandler<UserInteractionGetByIdQuery, UserInteraction>
{
    private readonly ApiDbContext _context;

    public UserInteractionGetByIdHandler(ApiDbContext context) => _context = context;

    public async Task<UserInteraction> Handle(UserInteractionGetByIdQuery rq, CancellationToken ct)
    {
        try
        {
            UserInteraction? model = await _context.UserInteraction
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == rq.Id, ct).ConfigureAwait(false);

            return model ?? throw new NotFoundException("User interaction not found", rq.Id);
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
        }
    }
}
