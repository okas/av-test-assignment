using System.ComponentModel.DataAnnotations;
using Backend.WebApi.Domain.Model;
using MediatR;

namespace Backend.WebApi.App.Operations.UserInteractionCommands;

public readonly record struct UserInteractionCreateCommand(
    [property: Required] DateTime Deadline,
    [property: Required, MinLength(2)] string? Description
    )
    : IRequest<UserInteraction>;
