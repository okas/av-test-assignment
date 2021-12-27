using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.App.Dto;

/// <summary>
/// Userinteraction DTO with base properties, that user should provide. For creation purpose.
/// </summary>
public record struct UserInteractionNewDto
{
    [Required]
    public DateTime Deadline { get; init; }

    [Required]
    public string Description { get; init; }
}
