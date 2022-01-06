using System.Linq.Expressions;
using Backend.WebApi.CrossCutting.Extensions;
using Backend.WebApi.Domain.Exceptions;
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

    public const string NotFoundOnIsOpenChangeMessage = "User interaction not found, while attempting to set its Open state.";
    public const string NotFoundOnQueryingMessage = "User interaction not found.";
    public const string AlreadyExists = "User interaction already exists.";

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

    public async Task<UserInteraction> GetOne(Guid id, CancellationToken ct)
    {
        try
        {
            UserInteraction? model = await _interactionsRepo
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct).ConfigureAwait(false);

            return model ?? throw new NotFoundException(NotFoundOnQueryingMessage, id);
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
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
    public async Task<(IEnumerable<Tout> models, int totalCount)> Get<Tout>(
        CancellationToken ct,
        Expression<Func<UserInteraction, Tout>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<Tout> query = BuildQuery(projection, filters);

        try
        {
            List<Tout> models = await query.ToListAsync(ct).ConfigureAwait(false);
            int totalCount = await _interactionsRepo.CountAsync(ct).ConfigureAwait(false);

            return (models.AsReadOnly(), totalCount);
        }
        catch
        {
            // TODO Log domain exception
            throw;
        }
    }

    public async Task SetOpenState(Guid id, bool newState, CancellationToken ct)
    {
        _context.Attach(new UserInteraction
        {
            Id = id,
            IsOpen = newState,
        }
        ).Property(model => model.IsOpen).IsModified = true;

        try
        {
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);

            return;
        }
        catch (DbUpdateConcurrencyException)
        {
            // TODO Log it
            // TODO Analyze https://docs.microsoft.com/en-us/ef/core/saving/concurrency to implement better handling
            if (!await _context.UserInteraction.AnyAsync(model => model.Id == id, ct).ConfigureAwait(false))
            {
                throw new NotFoundException(NotFoundOnIsOpenChangeMessage, id);
            }
            throw;
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
        }
    }

    /// <summary>
    /// Create new model into database.
    /// </summary>
    /// <param name="newModel"></param>
    /// <param name="ct"></param>
    /// <exception cref="AlreadyExistsException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    public async Task<UserInteraction> Create(UserInteraction newModel, CancellationToken ct)
    {
        // These values setup is system's responsibility, thus doing it here.
        newModel.IsOpen = true;
        newModel.Created = DateTime.Now;

        try
        {
            await _interactionsRepo.AddAsync(newModel, ct).ConfigureAwait(false); // TODO For single entity overhead is not justified.
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);

            return newModel;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // TODO Log it
            // TODO Current workflow, where entity instance is created in this handler, should exclude this situation.
            // As of now it should be thrown when Id generation outside of server fails somehow (e.g. default value is attempted).
            throw new AlreadyExistsException(AlreadyExists, newModel.Id, ex);
        }
        catch (DbUpdateConcurrencyException)
        {
            // TODO Whether and what should be logged here?
            throw;
        }
        catch
        {
            // TODO Whether and what should be logged here?
            throw;
        }
    }

    private IQueryable<Tout> BuildQuery<Tout>(
        Expression<Func<UserInteraction, Tout>>? projection,
        Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        IQueryable<UserInteraction> filteredQuery = _interactionsRepo
            .AsNoTracking()
            .AppendFiltersToQuery(filters);

        return projection is null
            ? filteredQuery.Cast<Tout>() // required, because in case of null projection, typeof(Tout) is not known for result.
            : filteredQuery.Select(projection);

    }
}
