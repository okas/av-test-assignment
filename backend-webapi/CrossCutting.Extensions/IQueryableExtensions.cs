using System.Linq.Expressions;

namespace Backend.WebApi.CrossCutting.Extensions;

public static class IQueryableExtensions
{
    /// <summary>
    /// Adds all predicates to Where() calls to given <see cref="IQueryable{T}"/> <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="T">Type in that filter is applied againts.</typeparam>
    /// <param name="filters">Predicates array that will update returned query.</param>
    /// <param name="query"></param>
    /// <returns>Updated query, chainable.</returns>
    public static IQueryable<T> AppendFiltersToQuery<T>(this IQueryable<T> query, params Expression<Func<T, bool>>?[]? filters)
    {
        if (filters is null)
        {
            return query;
        }

        return filters.Aggregate(query, (seed, predicate) => predicate is not null ? seed.Where(predicate) : seed);
    }
}
