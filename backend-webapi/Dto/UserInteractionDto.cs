using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;

/// <summary>
/// User Interaction DTO with full data. For full detail presentation purpose of the model entity.
/// </summary>
public class UserInteractionDto : UserInteractionUpdateDto
{
    [Required]
    public DateTime Created { get; set; }
}
