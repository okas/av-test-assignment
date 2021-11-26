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
        return new UserInteractionDto
        {
            Created = source.Created,
            Id = source.Id,
            IsOpen = source.IsOpen,
            Deadline = source.Deadline,
            Description = source.Description,
        };
    }
}
