namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class AlreadyExistsException : BaseException
{
    /// <inheritdoc />
    public AlreadyExistsException(string? message = default, object? model = default, string category = "", Exception? innerException = default)
        : base(message, model, category, innerException)
    { }
}
