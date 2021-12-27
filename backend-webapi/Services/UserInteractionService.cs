using System.Linq.Expressions;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Backend.WebApi.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Services;

public class UserInteractionService
{
    private static readonly string _queryingErrorMessage;
    private static readonly string _createNewModelErrorMessage;
    private readonly ApiDbContext _context;
    private readonly DbSet<UserInteraction> _interactionsRepo;

    static UserInteractionService()
    {
        _queryingErrorMessage = $"{nameof(UserInteractionService)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";
        _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";
    }

    public UserInteractionService(ApiDbContext dbContext)
    {
        _context = dbContext;
        _interactionsRepo = dbContext.UserInteraction;
    }

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> GetOne(Guid id)
    {
        try
        {
            UserInteraction? model = await _interactionsRepo.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            return (errors: Array.Empty<ServiceError>(), model);
        }
        catch (Exception ex)
        {
            ServiceError[] errors = new ServiceError[] { new(ServiceErrorKind.InternalError, Exceptions: ex) };

            return (errors, model: default);
        }
    }

    /// <summary>
    /// Gets models, optionally filtered, optionally projected.
    /// </summary>
    /// <remarks>
    /// NB! Intention to take in projection is to pass it to DB! It needs to be verified, though, whether/how it is possible. If not possible, then there is no point to do projection in this service!
    /// </remarks>
    /// <typeparam name="Tout">Type of projection.</typeparam>
    /// <param name="projection"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<(IEnumerable<ServiceError> errors, IEnumerable<Tout>? models, int totalCount)> Get<Tout>(
        Expression<Func<UserInteraction, Tout>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<UserInteraction>? filteredQuery = _interactionsRepo
            .AsNoTracking()
            .AppendFiltersToQuery(filters);

        (IEnumerable<ServiceError> errors, IEnumerable<Tout>? models) = await TryGetList(filteredQuery, projection);

        int total = errors.Any()
            ? default
            : await _interactionsRepo.CountAsync();

        return (errors, models, total);
    }

    public async Task<IEnumerable<ServiceError>> SetOpenState(Guid id, bool newState)
    {
        _context.Attach(new UserInteraction
        {
            Id = id,
            IsOpen = newState
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync();
            return Enumerable.Empty<ServiceError>();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _interactionsRepo.AnyAsync(model => model.Id == id))
            {
                return new[] {
                    new ServiceError(ServiceErrorKind.NotFoundOnChange)
                };
            }
            else
            {
                // HACK
                throw;
            }
        }
        catch (Exception ex)
        {
            return new[] {
                new ServiceError(ServiceErrorKind.InternalError, Exceptions: ex)
            };
        }
    }

    /// <summary>
    /// Create new model into database.
    /// </summary>
    /// <param name="newModel"></param>
    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Create(UserInteraction newModel)
    {
        // These values setup is system's responsibility, thus doing it here.
        newModel.IsOpen = true;
        newModel.Created = DateTime.Now;

        return await TryCreate(newModel);
    }

    private static async Task<(IEnumerable<ServiceError> errors, IEnumerable<Tout> models)> TryGetList<Tout>(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection)
    {
        try
        {
            return (errors: Enumerable.Empty<ServiceError>(),
                models: await RunGetToList(query, projection));
        }
        catch (Exception ex)
        {
            // TODO Log here.
            return (errors: new[] {
                new ServiceError(ServiceErrorKind.InternalError, _queryingErrorMessage, ex)
            }, models: Enumerable.Empty<Tout>());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// It is considered "dangerous code", because it can throw variuos exceptions due to using <see cref="Expression{T}"/> types thac might not be supported by EF Core. For example if predicates (filter or projection) contains methods that cannot be translated to SQL. <see href="https://docs.microsoft.com/en-us/ef/core/querying/client-eval">Client vs. Server Evaluation</see> for more info.
    /// </remarks>
    /// <typeparam name="Tout"></typeparam>
    /// <param name="query"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    private static async Task<IEnumerable<Tout>> RunGetToList<Tout>(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection)
    {
        IQueryable<Tout> modelsQuery = projection is null
            ? query.Cast<Tout>() // required, because in case of null projection, typeof(T) is not known
            : query.Select(projection);

        return (await modelsQuery.ToListAsync()).AsReadOnly();
    }

    private async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> TryCreate(UserInteraction model)
    {
        IEnumerable<ServiceError> errors;
        try
        {
            await _context.UserInteraction.AddAsync(model);
            await _context.SaveChangesAsync();

            return (Enumerable.Empty<ServiceError>(), model);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // TODO Log it
            errors = new ServiceError[] { new(ServiceErrorKind.AlreadyExistsOnCreate) };

            return (errors, default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            errors = new ServiceError[] { new(ServiceErrorKind.InternalError, _createNewModelErrorMessage, ex) };

            return (errors, default);
        }
    }
}
