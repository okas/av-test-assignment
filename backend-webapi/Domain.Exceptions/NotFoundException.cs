namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class NotFoundException : BaseException
{
    /// <inheritdoc />
    public NotFoundException(string? message = default, object? model = default, string category = "", Exception? innerException = default)
        : base(message, model, category, innerException)
    { }
}
