using System.ComponentModel.DataAnnotations;
using Backend.WebApi.Services;
using Backend.WebApi.ValidationExtensions;
using MediatR;

namespace Backend.WebApi.ModelOperations.UserInteractionCommands;

public class UserInteractionSetOpenStateCommand : IRequest<IEnumerable<ServiceError>?>
{
    public UserInteractionSetOpenStateCommand() { }

    public UserInteractionSetOpenStateCommand(Guid id, bool isOpen)
    {
        Id = id;
        IsOpen = isOpen;
    }

    [NotDefault]
    public Guid Id { get; init; }

    [Required]
    public bool IsOpen { get; init; }
}
