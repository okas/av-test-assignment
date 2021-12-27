using System.Linq.Expressions;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;

namespace Backend.WebApi.ModelOperations.UserInteractionQueries;

public record struct UserInteractionGetQuery<Tout> : IRequest<(IEnumerable<ServiceError> errors, IEnumerable<Tout>? models, int totalCount)>
{
    public UserInteractionGetQuery(Expression<Func<UserInteraction, Tout>>? projection = default,
        params Expression<Func<UserInteraction, bool>>?[]? filters)
    {
        Projection = projection;
        Filters = filters;
    }

    public Expression<Func<UserInteraction, bool>>?[]? Filters { get; init; }

    public Expression<Func<UserInteraction, Tout>>? Projection { get; init; }
}
