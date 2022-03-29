using System.Linq.Expressions;
using Backend.WebApi.Domain.Model;

namespace Backend.WebApi.App.Dto;

/// <summary>
/// Userinteraction DTO with full data. For full detail presentation purpose of the model entity.
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
/// <param name="Deadline"></param>
/// <param name="Created"></param>
/// <param name="IsOpen"></param>
/// <param name="ETag">Base64 string made from <see cref="T:System.Byte[]"/> array.</param>
public readonly record struct UserInteractionDto(
    Guid Id,
    string Description,
    DateTime Deadline,
    DateTime Created,
    bool IsOpen,
    string ETag) : IETag
{
    public static Expression<Func<UserInteraction, UserInteractionDto>> Projection => (model) => new()
    {
        Id = model.Id,
        Description = model.Description!,
        Deadline = model.Deadline,
        Created = model.Created,
        IsOpen = model.IsOpen,
        ETag = Convert.ToBase64String(model.RowVer),
    };
}
