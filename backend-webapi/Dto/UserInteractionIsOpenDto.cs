using System.ComponentModel.DataAnnotations;
using Backend.WebApi.ValidationExtensions;

namespace Backend.WebApi.Dto;

/// <summary>
/// Userinteraction "open state" changing DTO. For PATH method purpose in API.
/// </summary>
public record struct UserInteractionIsOpenDto
{
    [NotDefault]
    public Guid Id { get; init; }

    [Required]
    public bool IsOpen { get; init; }
}
