
namespace Backend.WebApi.Domain.Model;

public interface IEntity
{
    Guid Id { get; set; }

    byte[] RowVer { get; set; }
}