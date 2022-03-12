namespace Backend.WebApi.Domain.Model;

/// <summary>
/// User Interaction model. For storage purpose, as of now.
/// </summary>
public class UserInteraction
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime Deadline { get; set; } // TODO add NotDefault property and it must be gt than created

    public string Description { get; set; } = "";

    public bool IsOpen { get; set; }

    public byte[]? RowVer { get; set; }
}
