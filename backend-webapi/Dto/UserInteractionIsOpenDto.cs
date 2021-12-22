using System.ComponentModel.DataAnnotations;
using Backend.WebApi.ValidationExtensions;

namespace Backend.WebApi.Dto;


/// <summary>
/// Userinteraction "open state" changing DTO. For PATH method purpose in API.
/// </summary>
public class UserInteractionIsOpenDto
{
    [NotDefault]
    public Guid Id { get; set; }

    [Required]
    public bool IsOpen { get; set; }
}
