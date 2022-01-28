namespace Backend.WebApi.Domain.Exceptions;

/// <summary>
/// Populates <see cref="Exception.Data"/> property, see (<c>constructor</c>) <see cref="BaseException(string?, object?, string?, Exception?)"/>.
/// </summary>
public abstract class BaseException : Exception
{
    private string _category = "default";

    public const string ModelDataKey = "model";

    /// <summary>
    /// Populates <see cref="Category"/>; and <see cref="Exception.Data"/> properties by menas of: <code><see cref="Exception.Data"/>[<see cref="ModelDataKey"/>] = <paramref name="model"/>;</code>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="model"></param>
    /// <param name="category">Initializes <see cref="Category"/>, but if omited, then value <c>"default" will be used.</c></param>
    /// <param name="innerException"></param>
    protected BaseException(string? message, object? model, string category = "", Exception? innerException = default)
        : base(message, innerException)
    {
        Category = category;

        if (model is null)
        {
            return;
        }

        InitData(ModelDataKey, model);
    }

    public string Category // TODO Why not use ILogger<> instead?
    {
        get => _category;

        init => _category = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentOutOfRangeException(nameof(Category), value, "Should not be null, empty, or whitespace string.")
            : value;
    }

    private void InitData(string key, object? value) => Data.Add(key, value);
}
