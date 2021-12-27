using System.Linq.Expressions;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using Backend.WebApi.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.ModelOperations.UserInteractionQueries;

public class UserInteractionGetHandler<Tout> : IRequestHandler<UserInteractionGetQuery<Tout>, (IEnumerable<ServiceError> errors, IEnumerable<Tout>? models, int totalCount)>
{
    private static readonly string _queryingErrorMessage;
    private readonly ApiDbContext _dbContext;

    static UserInteractionGetHandler()
    {
        _queryingErrorMessage = $"{nameof(UserInteractionService)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";
    }

    public UserInteractionGetHandler(ApiDbContext dbContext) => _dbContext = dbContext;

    public async Task<(IEnumerable<ServiceError> errors, IEnumerable<Tout>? models, int totalCount)> Handle(
        UserInteractionGetQuery<Tout> request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UserInteraction> filteredQuery = _dbContext.UserInteraction
            .AsNoTracking()
            .AppendFiltersToQuery(request.Filters);

        (IEnumerable<ServiceError> errors, IEnumerable<Tout>? models) =
            await TryGetList(filteredQuery, request.Projection, cancellationToken);

        int total = errors.Any()
            ? 0
            : await _dbContext.UserInteraction.CountAsync(cancellationToken);

        return (errors, models, total);
    }

    private static async Task<(IEnumerable<ServiceError>, IEnumerable<Tout>?)> TryGetList(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection,
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Tout> models = await RetreiveListFrom(query, projection, cancellationToken);

            return (Enumerable.Empty<ServiceError>(), models);
        }
        catch (Exception ex)
        {
            // TODO Log here.
            ServiceError[] errors = { new(ServiceErrorKind.InternalError, _queryingErrorMessage, ex) };

            return (errors, default);
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// It is considered "dangerous code", because it can throw variuos exceptions due to using <see cref="Expression{T}"/> types that might not be supported by EF Core. For example if predicates (filter or projection) contains methods that cannot be translated to SQL. <see href="https://docs.microsoft.com/en-us/ef/core/querying/client-eval">Client vs. Server Evaluation</see> for more info.
    /// </remarks>
    /// <param name="query"></param>
    /// <param name="projection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<IEnumerable<Tout>> RetreiveListFrom(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection,
        CancellationToken cancellationToken)
    {
        IQueryable<Tout> modelsQuery = projection is null
                    ? query.Cast<Tout>() // required, because in case of null projection, typeof(T) is not known
                    : query.Select(projection);

        return (await modelsQuery.ToListAsync(cancellationToken: cancellationToken)).AsReadOnly();
    }
}
