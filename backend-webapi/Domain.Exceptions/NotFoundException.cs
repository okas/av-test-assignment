namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class NotFoundException : BaseException
{
    /// <summary>
    /// Populates <see cref="Exception.Data"/> property by menas of: <code><see cref="Exception.Data"/>["Id"] = <paramref name="id"/>;</code>
    /// </summary>
    public NotFoundException(string message, object id, Exception? innerException = default)
        : base(message, "Id", id, innerException) { }
}
