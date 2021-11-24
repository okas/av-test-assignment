using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;

/// <summary>
/// Userinteraction DTO with base properties, that user should provide. For creation purpose.
/// </summary>
public class UserInteractionNewDto
{
    [Required]
    public DateTime Deadline { get; set; }

    [Required]
    public string Description { get; set; }
}
