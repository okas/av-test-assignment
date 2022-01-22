using System.Linq.Expressions;
using Backend.WebApi.CrossCutting.Extensions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public readonly record struct UserInteractionGetQuery<Tout>(
    Expression<Func<UserInteraction, Tout>>? Projection,
    params Expression<Func<UserInteraction, bool>>?[]? Filters
    )
    : IRequest<(IEnumerable<Tout> models, int totalCount)>
{
    /// <summary>
    /// Handles <see cref="UserInteractionGetQuery{Tout}"/> command.
    /// </summary>
    /// <param name="Context">Dependency.</param>
    public record Handler(ApiDbContext Context) : IRequestHandler<UserInteractionGetQuery<Tout>, (IEnumerable<Tout> models, int totalCount)> // TODO To class, cause no record features used
    {
        private static readonly string _queryingErrorMessage;

        static Handler() =>
            _queryingErrorMessage = $"{nameof(Handler)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";

        public async Task<(IEnumerable<Tout> models, int totalCount)> Handle(UserInteractionGetQuery<Tout> rq, CancellationToken ct)
        {
            IQueryable<Tout> query = BuildQuery(rq);

            try
            {
                List<Tout> models = await query.ToListAsync(ct).ConfigureAwait(false);
                int totalCount = await Context.UserInteraction.CountAsync(ct).ConfigureAwait(false);

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
            IQueryable<UserInteraction> filteredQuery = Context.UserInteraction
                .AsNoTracking()
                .AppendFiltersToQuery(rq.Filters);

            return rq.Projection is null
                ? filteredQuery.Cast<Tout>() // required, because in case of null projection, typeof(Tout) is not known for result.
                : filteredQuery.Select(rq.Projection);

        }
    }
}
