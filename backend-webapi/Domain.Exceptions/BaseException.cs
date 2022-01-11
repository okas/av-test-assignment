namespace Backend.WebApi.Domain.Exceptions;

/// <summary>
/// Populates <see cref="Exception.Data"/> property, see (<c>constructor</c>) <see cref="BaseException(string?, string?, object?, Exception?)"/>.
/// </summary>
public abstract class BaseException : Exception
{
    /// <summary>
    /// Populates <see cref="Exception.Data"/> property by menas of: <code><see cref="Exception.Data"/>[<paramref name="key"/>] = <paramref name="value"/>;</code>
    /// </summary>
    protected BaseException(string? message, string key, object value, Exception? innerException)
        : base(message, innerException) =>
        InitData(key, value);

    private void InitData(string key, object value) => Data.Add(key, value);
}