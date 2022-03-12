namespace Backend.WebApi.Domain.Exceptions;

/// <inheritdoc />
public class ConcurrentUpdateException : BaseException
{
    /// <inheritdoc />
    public ConcurrentUpdateException(string? message = default, object? model = default, string category = "", Exception? innerException = default)
        : base(message, model, category, innerException)
    { }
}
