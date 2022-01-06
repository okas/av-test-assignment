using System.Linq.Expressions;
using Backend.WebApi.Domain.Model;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public readonly record struct UserInteractionGetQuery<Tout>(
    Expression<Func<UserInteraction, Tout>>? Projection,
    params Expression<Func<UserInteraction, bool>>?[]? Filters
    )
    : IRequest<(IEnumerable<Tout> models, int totalCount)>;