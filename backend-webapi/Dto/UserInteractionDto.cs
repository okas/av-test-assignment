using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;

/// <summary>
/// User Interaction DTO with full data. For presentation (and update purposes).
/// </summary>
public class UserInteractionDto : UserInteractionNewDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public DateTime Created { get; set; }

    [Required]
    public bool IsOpen { get; set; }
}
