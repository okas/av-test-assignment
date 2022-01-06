namespace Backend.WebApi.Domain.Exceptions;

/// <summary>
/// Provides <see cref="Exception.Data"/> property populating when (<c>constructor</c>) <see cref="BaseException(string?, object?, Exception?)"/> is used.
/// </summary>
public abstract class BaseException : Exception
{
    protected BaseException() { }

    protected BaseException(string? message) : base(message) { }

    protected BaseException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <inheritdoc cref="BaseException(string?, object?, Exception?)"/>
    protected BaseException(string? message, object? id) : this(message, id, default) { }

    /// <summary>
    /// Populates <see cref="Exception.Data"/> property by menas of: <code>Exception.Data["Id"] = id;</code>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="id">If is not null then value to be used: <code>Exception.Data["Id"] = id;</code></param>
    /// <param name="innerException"></param>
    protected BaseException(string? message, object? id, Exception? innerException) : base(message, innerException)
    {
        if (id is not null)
        {
            InitData(id);
        }
    }

    private void InitData(object id) => Data["Id"] = id;
}