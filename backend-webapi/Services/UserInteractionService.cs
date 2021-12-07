using System.Linq.Expressions;
using Backend.WebApi.Data.EF;
using Backend.WebApi.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Services;

public class UserInteractionService
{
    private readonly ApiDbContext _context;
    readonly DbSet<UserInteraction> _interactionsRepo;

    public UserInteractionService(ApiDbContext dbContext)
    {
        _context = dbContext;
        _interactionsRepo = dbContext.UserInteraction;
    }

    public async Task<UserInteraction?> GetOne(Guid id)
    {
        return await _interactionsRepo.AsNoTracking().FirstOrDefaultAsync(model => model.Id == id);
    }

    public async Task<IEnumerable<T>> GetSome<T>(
        Expression<Func<UserInteraction, T>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<UserInteraction>? query = _interactionsRepo.AsNoTracking();

        if (filters?.Any() ?? false)
        {
            foreach (var predicate in filters)
            {
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
            }
        }
        // TODO Count gathering in here
        if (projection != null)
        {
            return await query.Select(projection).Cast<T>().ToListAsync();
        }
        else
        {
            return await query.Cast<T>().ToListAsync();
        }
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
                return (false, new[] { new ServiceError(ServiceResultType.NotFound) });
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            return (false, new[] { new ServiceError(ServiceResultType.InternalError, Exception: ex) });
        }
        return (true, default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newModel"></param>
    public async Task<(bool succeed, UserInteraction? model, IEnumerable<ServiceError>? errors)> Create(UserInteraction newModel)
    {
        // These values setup is system's responsibility, thus doing it here.
        newModel.IsOpen = true;
        newModel.Created = DateTime.Now;

        return await TryCreate(newModel);
    }

    private async Task<(bool succeed, UserInteraction? model, IEnumerable<ServiceError>? errors)> TryCreate(UserInteraction newModel)
    {
        try
        {
            _interactionsRepo.Add(newModel);
            await _context.SaveChangesAsync();
            return (true, newModel, default);
        }
        catch (DbUpdateException ex)
        {
            return await TryGenerateAlreadyExistingEntityError(ex, newModel);
        }
        catch (Exception ex)
        {
            return (false, default, new[] { CreateNewModelInternalErrorResult(ex) });
        }
    }

    /// <summary>
    /// If duplicate is detected, then tries to create error type of <see cref="ServiceResultType.AlreadyExists" /> error result. Otherwise more general type of <see cref="ServiceResultType.InternalError" /> error result is created.
    /// </summary>
    /// <param name="ex">Exception that will be analyzed.</param>
    /// <param name="attemptedModel">Model object that has attempted to ad to database as new.</param>
    /// <returns></returns>
    private async Task<(bool succeed, UserInteraction? model, IEnumerable<ServiceError>? errors)> TryGenerateAlreadyExistingEntityError(DbUpdateException ex, UserInteraction attemptedModel)
    {
        ServiceError error;

        var existingModel = await GetOne(attemptedModel.Id);

        if (ex.InnerException is SqlException && existingModel != null)
        {
            error = new ServiceError(ServiceResultType.AlreadyExists,
                $"Attempted to insert duplicate entity of {nameof(UserInteraction)}.");
        }
        else
        {
            error = new ServiceError(ServiceResultType.InternalError,
                ex.InnerException?.Message ?? ex.Message);
        }

        return (false, existingModel, new[] { error });
    }

    private static ServiceError CreateNewModelInternalErrorResult(Exception ex) => new(
            ServiceResultType.InternalError,
            "Some unexpected error occured. Sorry.",
            ex
     );
}

public record ServiceError(ServiceResultType ResultType, string? Message = default, params Exception?[]? Exception);

/// <summary>
/// Typest that indicate what went in Service wrong while called action.
/// </summary>
public enum ServiceResultType
{
    /// <summary>
    /// Entity not found result.
    /// </summary>
    NotFound,

    /// <summary>
    /// Entit already exists in database result.
    /// </summary>
    AlreadyExists,

    /// <summary>
    /// Something unknown and serious happened result.
    /// </summary>
    InternalError,
}
