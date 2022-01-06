namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class NotFoundException : BaseException
{
    public NotFoundException() { }

    public NotFoundException(string? message) : base(message) { }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <inheritdoc />
    public NotFoundException(string? message, object? id) : base(message, id) { }

    /// <inheritdoc />
    public NotFoundException(string? message, object? id, Exception? innerException) : base(message, id, innerException) { }
}
