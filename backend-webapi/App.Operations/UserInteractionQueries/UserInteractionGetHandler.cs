using Backend.WebApi.CrossCutting.Extensions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public class UserInteractionGetHandler<Tout> : IRequestHandler<UserInteractionGetQuery<Tout>, (IEnumerable<Tout> models, int totalCount)>
{
    private static readonly string _queryingErrorMessage;
    private readonly ApiDbContext _dbContext;

    static UserInteractionGetHandler() =>
        _queryingErrorMessage = $"{nameof(UserInteractionGetHandler<Tout>)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";

    public UserInteractionGetHandler(ApiDbContext dbContext) => _dbContext = dbContext;

    public async Task<(IEnumerable<Tout> models, int totalCount)> Handle(UserInteractionGetQuery<Tout> rq, CancellationToken ct)
    {
        IQueryable<Tout> query = BuildQuery(rq);

        try
        {
            List<Tout> models = await query.ToListAsync(ct).ConfigureAwait(false);
            int totalCount = await _dbContext.UserInteraction.CountAsync(ct).ConfigureAwait(false);

            return (models.AsReadOnly(), totalCount);
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
        }
    }

    private IQueryable<Tout> BuildQuery(UserInteractionGetQuery<Tout> rq)
    {
        IQueryable<UserInteraction> filteredQuery = _dbContext.UserInteraction
            .AsNoTracking()
            .AppendFiltersToQuery(rq.Filters);

        return rq.Projection is null
            ? filteredQuery.Cast<Tout>() // required, because in case of null projection, typeof(Tout) is not known for result.
            : filteredQuery.Select(rq.Projection);

    }
}
