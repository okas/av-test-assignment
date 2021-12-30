using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Backend.WebApi.App.Services;
using Backend.WebApi.CrossCutting.Extensions.Validation;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

[StructLayout(LayoutKind.Auto)]

public record struct UserInteractionSetOpenStateCommand : IRequest<IEnumerable<ServiceError>>
{
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
