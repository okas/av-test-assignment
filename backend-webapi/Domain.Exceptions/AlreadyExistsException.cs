namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class AlreadyExistsException : BaseException
{
    public AlreadyExistsException() { }

    public AlreadyExistsException(string? message) : base(message) { }

    public AlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <inheritdoc />
    public AlreadyExistsException(string? message, object? id) : base(message, id, default) { }

    /// <inheritdoc />
    public AlreadyExistsException(string? message, object? id, Exception? innerException) : base(message, id, innerException) { }
}
