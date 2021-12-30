﻿using Backend.WebApi.App.Services;
using Backend.WebApi.CrossCutting.Extensions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public class UserInteractionGetHandler<Tout> : IRequestHandler<UserInteractionGetQuery<Tout>, (IEnumerable<ServiceError> errors, IEnumerable<Tout> models, int? totalCount)>
{
    private static readonly string _queryingErrorMessage;
    private readonly ApiDbContext _dbContext;

    static UserInteractionGetHandler() =>
        _queryingErrorMessage = $"{nameof(UserInteractionGetHandler<Tout>)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";

    public UserInteractionGetHandler(ApiDbContext dbContext) => _dbContext = dbContext;

    public async Task<(IEnumerable<ServiceError> errors, IEnumerable<Tout> models, int? totalCount)> Handle(UserInteractionGetQuery<Tout> rq, CancellationToken ct)
    {
        IQueryable<Tout> query = BuildQuery(rq);

        try
        {
            List<Tout> models = await query.ToListAsync(ct).ConfigureAwait(false);
            int totalCount = await _dbContext.UserInteraction.CountAsync(ct).ConfigureAwait(false);

            return (Enumerable.Empty<ServiceError>(), models.AsReadOnly(), totalCount);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, models: Enumerable.Empty<Tout>(), totalCount: default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.InternalError, _queryingErrorMessage, ex) };

            return (errors, models: Enumerable.Empty<Tout>(), totalCount: default);
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
