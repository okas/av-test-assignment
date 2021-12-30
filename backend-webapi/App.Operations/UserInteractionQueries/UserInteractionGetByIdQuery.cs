using Backend.WebApi.App.Services;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using Backend.WebApi.Domain.Model;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionQueries;

public record struct UserInteractionGetByIdQuery : IRequest<(IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    [NotDefault]
    public Guid Id { get; init; }
}
