using System.ComponentModel.DataAnnotations;
using Backend.WebApi.App.Services;
using Backend.WebApi.Domain.Model;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public record struct UserInteractionCreateCommand : IRequest<(IEnumerable<ServiceError> errors, UserInteraction? model)>
{
    public UserInteractionCreateCommand(DateTime deadline, string description)
    {
        Deadline = deadline;
        Description = description;
    }

    [Required]
    public DateTime Deadline { get; init; }

    [Required, MinLength(2)]
    public string? Description { get; init; }
}
