using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;

/// <summary>
/// User Interaction DTO with base properties, that use should provide. For creation purpose.
/// </summary>
public class UserInteractionNewDto
{
    [Required]
    public DateTime Deadline { get; set; }

    [Required]
    public string Description { get; set; }

    public bool IsOpen { get; set; }
}
