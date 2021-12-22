using System.Linq.Expressions;
using Backend.WebApi.Model;
using Backend.WebApi.ValidationExtensions;

namespace Backend.WebApi.Dto;

/// <summary>
/// Userinteraction DTO with full data. For full detail presentation purpose of the model entity.
/// </summary>
public class UserInteractionDto
{
    [NotDefault]
    public Guid Id { get; set; }

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime Created { get; set; }

    public bool IsOpen { get; set; }

    public static Expression<Func<UserInteraction, UserInteractionDto>> Projection => (model) => new()
    {
        Id = model.Id,
        Description = model.Description,
        Deadline = model.Deadline,
        Created = model.Created,
        IsOpen = model.IsOpen
    };
}
