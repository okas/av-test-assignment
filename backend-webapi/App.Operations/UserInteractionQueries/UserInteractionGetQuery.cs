﻿using System.Linq.Expressions;
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
    public record Handler : IRequestHandler<UserInteractionGetQuery<Tout>, (IEnumerable<Tout> models, int totalCount)>
    {
        private readonly ApiDbContext _context;
        private static readonly string _queryingErrorMessage;

        public Handler(ApiDbContext context) => _context = context;

        static Handler() =>
            _queryingErrorMessage = $"{nameof(Handler)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";

        public async Task<(IEnumerable<Tout> models, int totalCount)> Handle(UserInteractionGetQuery<Tout> rq, CancellationToken ct)
        {
            IQueryable<Tout> query = BuildQuery(rq);

            try
            {
                List<Tout> models = await query.ToListAsync(ct).ConfigureAwait(false);
                int totalCount = await _context.UserInteraction.CountAsync(ct).ConfigureAwait(false);

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
            IQueryable<UserInteraction> filteredQuery = _context.UserInteraction
                .AsNoTracking()
                .AppendFiltersToQuery(rq.Filters);

            return rq.Projection is null
                ? filteredQuery.Cast<Tout>() // required, because in case of null projection, typeof(Tout) is not known for result.
                : filteredQuery.Select(rq.Projection);

        }
    }
}
