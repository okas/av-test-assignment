using System.ComponentModel.DataAnnotations;
using Backend.WebApi.Model;
using Backend.WebApi.Services;
using MediatR;

namespace Backend.WebApi.ModelOperations.UserInteractionCommands;

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
