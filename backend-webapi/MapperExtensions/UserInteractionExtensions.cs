using Backend.WebApi.Dto;
using Backend.WebApi.Model;

namespace Backend.WebApi.MapperExtensions;

public static class UserInteractionExtensions
{
    /// <summary>
    /// Map DTO to Model.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static UserInteraction ToModel(this UserInteractionDto source)
    {
        var model = ToModel(source);
        model.Id = source.Id;
        model.Created = source.Created;
        model.IsOpen = source.IsOpen;
        return model;
    }

    /// <summary>
    /// Map Model to non-create DTO.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static UserInteractionDto ToDto(this UserInteraction source)
    {
        var dto = (UserInteractionDto)ToNewDto(source);
        dto.Id = source.Id;
        return dto;
    }

    /// <summary>
    /// Map DTO to Model.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static UserInteraction ToModel(this UserInteractionNewDto source)
    {
        return new UserInteraction
        {
            Deadline = source.Deadline,
            Description = source.Description
        };
    }

    /// <summary>
    /// Map Model to create DTO.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private static UserInteractionNewDto ToNewDto(this UserInteraction source)
    {
        return new UserInteractionDto
        {
            Created = source.Created,
            Deadline = source.Deadline,
            Description = source.Description,
            IsOpen = source.IsOpen
        };
    }
}

