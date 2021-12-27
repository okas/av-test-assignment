using Backend.WebApi.Model;
using Backend.WebApi.Services;
using Backend.WebApi.ValidationExtensions;
using MediatR;

namespace Backend.WebApi.ModelOperations.UserInteractionQueries;

public record struct UserInteractionGetByIdQuery : IRequest<(IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    public UserInteractionGetByIdQuery(Guid id) => Id = id;

    [NotDefault]
    public Guid Id { get; init; }
}
