using Backend.WebApi.Dto;
using Backend.WebApi.Model;

namespace Backend.WebApi.MapperExtensions;

public static class UserInteractionExtensions
{
    /// <summary>
    /// Map DTO to Model.
    /// </summary>
    public static UserInteraction ToModel(this UserInteractionDto source)
    {
        var model = ToModel(source as UserInteractionUpdateDto);
        model.Created = source.Created;
        return model;
    }

    /// <summary>
    /// Map DTO to Model.
    /// </summary>
    public static UserInteraction ToModel(this UserInteractionUpdateDto source)
    {
        var model = ToModel(source as UserInteractionNewDto);
        model.Id = source.Id;
        model.IsOpen = source.IsOpen;
        return model;
    }

    /// <summary>
    /// Map DTO to Model.
    /// </summary>
    public static UserInteraction ToModel(this UserInteractionNewDto source)
    {
        return new UserInteraction
        {
            Deadline = source.Deadline,
            Description = source.Description
        };
    }

    /// <summary>
    /// Map Model to full detail DTO.
    /// </summary>
    public static UserInteractionDto ToDto(this UserInteraction source)
    {
        var dto = (UserInteractionDto)ToUpdateDto(source);
        dto.Id = source.Id;
        return dto;
    }

    /// <summary>
    /// Map Model to update DTO.
    /// </summary>
    private static UserInteractionUpdateDto ToUpdateDto(this UserInteraction source)
    {
        var dto = (UserInteractionUpdateDto)ToNewDto(source);
        dto.Id = source.Id;
        dto.IsOpen = source.IsOpen;
        return dto;
    }

    /// <summary>
    /// Map Model to create DTO.
    /// </summary>
    private static UserInteractionNewDto ToNewDto(this UserInteraction source)
    {
        return new UserInteractionNewDto
        {
            Deadline = source.Deadline,
            Description = source.Description,
        };
    }
}
