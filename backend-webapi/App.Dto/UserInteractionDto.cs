using System.Linq.Expressions;
using Backend.WebApi.Domain.Model;

namespace Backend.WebApi.App.Dto;

/// <summary>
/// Userinteraction DTO with full data. For full detail presentation purpose of the model entity.
/// </summary>
public readonly record struct UserInteractionDto(
    Guid Id,
    string Description,
    DateTime Deadline,
    DateTime Created,
    bool IsOpen
    )
{
    public static Expression<Func<UserInteraction, UserInteractionDto>> Projection => (model) => new()
    {
        Id = model.Id,
        Description = model.Description!,
        Deadline = model.Deadline,
        Created = model.Created,
        IsOpen = model.IsOpen,
    };
}
