using System.ComponentModel.DataAnnotations;
using Backend.WebApi.CrossCutting.Extensions.Validation;

namespace Backend.WebApi.Domain.Model;

/// <summary>
/// User Interaction model. For storage purpose, as of now.
/// </summary>
public class UserInteraction
{
    [NotDefault]
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime Deadline { get; set; }

    [Required]
    public string? Description { get; set; }

    public bool IsOpen { get; set; }
}
