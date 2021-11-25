using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;

/// <summary>
/// Userinteraction DTO to update model entity, for PUT HTTP method.
/// It only excludes `Created` property of the model to protect it from being updated.
/// </summary>
public class UserInteractionUpdateDto : UserInteractionNewDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public bool IsOpen { get; set; }
}
