using Backend.WebApi.Domain.Exceptions;
using Backend.WebApi.Domain.Model;
using Backend.WebApi.Infrastructure.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public abstract class ConcurrencyHandlerBase
{
    const string MESSAGE = "Concurrency detected, operation cancelled.";

    protected readonly ApiDbContext _context;

    protected ConcurrencyHandlerBase(ApiDbContext context) => _context = context;

    /// <exception cref="NotFoundException" />
    /// <exception cref="ConcurrentUpdateException" />
    /// <exception cref="OperationCanceledException" />
    /// <exception cref="DbUpdateException" />
    public async Task<int> SaveAndHandleExceptions<TEntity>(TEntity model, string loggerCategory, CancellationToken ct)
        where TEntity : class, IEntity
    {
        try
        {
            return await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException)
        {
            bool entityExists = await _context.Set<TEntity>()
                .AnyAsync(e => e.Id == model.Id, CancellationToken.None)
                .ConfigureAwait(false);

            throw entityExists
                ? new ConcurrentUpdateException(MESSAGE, model, loggerCategory)
                : new NotFoundException(MESSAGE, model, loggerCategory);
        }
    }
}
