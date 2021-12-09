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
        _queryingErrorMessage = $@"{nameof(UserInteractionService)} encountered error while querying database. Probbably caused by bad WebApi code. Operation was stopped.";

        _createNewModelErrorMessage = $"Attempted to create new `{nameof(UserInteraction)}`, but operation was cancelled unexpectedly. See excpetion details.";
    }

    public UserInteractionService(ApiDbContext dbContext)
    {
        _context = dbContext;
        _interactionsRepo = dbContext.UserInteraction;
    }

    public async Task<(IEnumerable<ServiceError>? errors, UserInteraction? model)> GetOne(Guid id)
    {
        try
        {
            return (default, await _interactionsRepo.AsNoTracking().FirstOrDefaultAsync(model => model.Id == id));
        }
        catch (Exception ex)
        {
            return (new ServiceError[] {
                new(ServiceResultType.InternalError, Exceptions: ex)
            }, default);
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
    public async Task<(IEnumerable<ServiceError>? errors, IList<Tout>? models, int totalCount)> GetSome<Tout>(
        Expression<Func<UserInteraction, Tout>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        var filteredQuery = _interactionsRepo
            .AsNoTracking()
            .AppendFiltersToQuery(filters);

        (IEnumerable<ServiceError>? errors, IList<Tout>? models) = await TryGetList(filteredQuery, projection);

        int total = errors?.Any() ?? false
            ? 0
            : await _interactionsRepo.CountAsync();

        return (errors, models, total);
    }

    public async Task<(bool succeed, IEnumerable<ServiceError>? errors)> SetOpenState(Guid id, bool newState)
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
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _interactionsRepo.AnyAsync(model => model.Id == id))
            {
                return (false, new[] {
                    new ServiceError(ServiceResultType.NotFoundOnChange)
                });
            }
            else
            {
                // HACK
                throw;
            }
        }
        catch (Exception ex)
        {
            return (false, new[] {
                new ServiceError(ServiceResultType.InternalError, Exceptions: ex)
            });
        }

        return (true, default);
    }

    /// <summary>
    /// Create new model into database.
    /// </summary>
    /// <param name="newModel"></param>
    public async Task<(IEnumerable<ServiceError>? errors, UserInteraction? model)> Create(UserInteraction newModel)
    {
        // These values setup is system's responsibility, thus doing it here.
        newModel.IsOpen = true;
        newModel.Created = DateTime.Now;

        return await TryCreate(newModel);
    }

    private static async Task<(IEnumerable<ServiceError>?, IList<Tout>?)> TryGetList<Tout>(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection)
    {
        ServiceError[]? errors = default;
        IList<Tout>? models = default;

        try
        {
            models = await RunGetSomeToList(query, projection);
        }
        catch (Exception ex)
        {
            // TODO Log here.
            errors = new[] {
                new ServiceError(ServiceResultType.InternalError, _queryingErrorMessage, ex)
            };
        }

        return (errors, models);
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
    private static async Task<IList<Tout>?> RunGetSomeToList<Tout>(
        IQueryable<UserInteraction> query,
        Expression<Func<UserInteraction, Tout>>? projection)
    {
        if (projection == null)
        {
            // Cast<>() is required, because in case of null projection, typeof(T) is not known
            return await query.Cast<Tout>().ToListAsync();
        }
        else
        {
            return await query.Select(projection).ToListAsync();
        }
    }

    private async Task<(IEnumerable<ServiceError>? errors, UserInteraction? model)> TryCreate(UserInteraction newModel)
    {
        try
        {
            _interactionsRepo.Add(newModel);
            await _context.SaveChangesAsync();
            return (default, newModel);
        }
        catch (DbUpdateException ex)
        {
            return HandleCreateModelException(ex);
        }
        catch (Exception ex)
        {
            return (new[] {
                new ServiceError(ServiceResultType.InternalError, _createNewModelErrorMessage, ex)
            }, default);
        }
    }

    private (IEnumerable<ServiceError>? errors, UserInteraction? model) HandleCreateModelException(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException)
        {

        }
        return default;
    }
}

public record ServiceError(ServiceResultType ResultType, string? Message = default, params Exception?[]? Exceptions);

/// <summary>
/// Typest that indicate what went in Service wrong while called action.
/// </summary>
public enum ServiceResultType
{
    /// <summary>
    /// Entity not found result on change operation.
    /// </summary>
    /// <remarks>Not intended to use on query operations where entity is not found.</remarks>
    NotFoundOnChange,

    /// <summary>
    /// Entit already exists in database result.
    /// </summary>
    AlreadyExistsOnCreate,

    /// <summary>
    /// Something unknown and serious happened result.
    /// </summary>
    InternalError,
}
