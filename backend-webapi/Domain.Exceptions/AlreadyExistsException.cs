namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class AlreadyExistsException : BaseException
{
    /// <inheritdoc />
    public AlreadyExistsException(string? message, string key, object value, Exception? innerException = default)
        : base(message, key, value, innerException) { }
}
