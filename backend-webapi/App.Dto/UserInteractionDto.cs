using System.Linq.Expressions;
using Backend.WebApi.Domain.Model;

namespace Backend.WebApi.App.Dto;

/// <summary>
/// Userinteraction DTO with full data. For full detail presentation purpose of the model entity.
/// </summary>
public record struct UserInteractionDto
{
    public Guid Id { get; init; }

    public string Description { get; init; }

    public DateTime Deadline { get; init; }

    public DateTime Created { get; init; }

    public bool IsOpen { get; init; }

    public static Expression<Func<UserInteraction, UserInteractionDto>> Projection => (model) => new()
    {
        Id = model.Id,
        Description = model.Description!,
        Deadline = model.Deadline,
        Created = model.Created,
        IsOpen = model.IsOpen
    };
}
