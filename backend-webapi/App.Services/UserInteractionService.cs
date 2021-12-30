using System.Linq.Expressions;
using Backend.WebApi.CrossCutting.Extensions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Services;

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

    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> GetOne(Guid id, CancellationToken ct)
    {
        try
        {
            UserInteraction? model = await _interactionsRepo
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            return (errors: Array.Empty<ServiceError>(), model);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, default);
        }
        catch (Exception ex)
        {
            // TODO Log it
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
    /// <param name="ct"></param>
    /// <param name="projection"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<(IEnumerable<ServiceError> errors, IEnumerable<Tout>? models, int totalCount)> Get<Tout>(
        CancellationToken ct,
        Expression<Func<UserInteraction, Tout>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<Tout> query = BuildQuery(projection, filters);

        try
        {
            List<Tout> models = await query.ToListAsync(ct);
            int totalCount = await _interactionsRepo.CountAsync(ct);

            return (Enumerable.Empty<ServiceError>(), models.AsReadOnly(), totalCount);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, models: Enumerable.Empty<Tout>(), totalCount: default);
        }
        catch (Exception ex)
        {
            ServiceError[] errors = { new(ServiceErrorKind.InternalError, _queryingErrorMessage, ex) };

            return (errors, models: Enumerable.Empty<Tout>(), totalCount: default);
        }
    }

    public async Task<IEnumerable<ServiceError>> SetOpenState(Guid id, bool newState, CancellationToken ct)
    {
        _context.Attach(new UserInteraction
        {
            Id = id,
            IsOpen = newState,
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync(ct);

            return Enumerable.Empty<ServiceError>();
        }
        catch (DbUpdateConcurrencyException)
        {
            // TODO Log it
            if (await _context.UserInteraction.AnyAsync(model => model.Id == id, ct))
            {
                throw;
            }

            return new ServiceError[] { new(ServiceErrorKind.NotFoundOnChange) };
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            return new ServiceError[] { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };
        }
        catch (Exception ex)
        {
            // TODO Log it
            return new ServiceError[] { new(ServiceErrorKind.InternalError, Exceptions: ex) };
        }
    }

    /// <summary>
    /// Create new model into database.
    /// </summary>
    /// <param name="newModel"></param>
    /// <param name="ct"></param>
    public async Task<(IEnumerable<ServiceError> errors, UserInteraction? model)> Create(UserInteraction newModel, CancellationToken ct)
    {
        // These values setup is system's responsibility, thus doing it here.
        newModel.IsOpen = true;
        newModel.Created = DateTime.Now;

        try
        {
            await _interactionsRepo.AddAsync(newModel, ct);
            await _context.SaveChangesAsync(ct);

            return (Enumerable.Empty<ServiceError>(), newModel);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.AlreadyExistsOnCreate) };

            return (errors, model: default);
        }
        catch (OperationCanceledException ocex) when (ocex.CancellationToken.IsCancellationRequested)
        {
            // TODO Log it
            ServiceError[] errors = { new(ServiceErrorKind.OperationCancellationRequested, Exceptions: ocex) };

            return (errors, model: default);
        }
        catch (Exception ex)
        {
            // TODO Log it
            ServiceError[] errors = new ServiceError[] { new(ServiceErrorKind.InternalError, _createNewModelErrorMessage, ex) };

            return (errors, model: default);
        }
    }

    private IQueryable<Tout> BuildQuery<Tout>(
        Expression<Func<UserInteraction, Tout>>? projection,
        Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<UserInteraction> filteredQuery = _interactionsRepo
            .AsNoTracking()
            .AppendFiltersToQuery(filters);

        IQueryable<Tout> filteredAndProjectedQuery = projection is null
            ? filteredQuery.Cast<Tout>() // required, because in case of null projection, typeof(Tout) is not known for result.
            : filteredQuery.Select(projection);

        return filteredAndProjectedQuery;
    }
}
