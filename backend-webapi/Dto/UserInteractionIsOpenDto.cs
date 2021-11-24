using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Dto;


/// <summary>
/// Userinteraction "open state" changing DTO. For PATH method purpose in API.
/// </summary>
public class UserInteractionIsOpenDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public bool IsOpen { get; set; }
}
