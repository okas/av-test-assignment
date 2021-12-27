using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.CrossCutting.Extensions.Validation;

/// <summary>
/// Validates if value is not type's default.
/// </summary>
/// <remarks>
/// Value's type non-default value or <see langword="null"/> will return <see langword="true"/>.
/// Use with <see cref="RequiredAttribute"/> to invalidate <see langword="null"/> of <see cref="Nullable{T}"/> values.
/// See <seealso href="https://andrewlock.net/creating-an-empty-guid-validation-attribute/">source</seealso>.
/// </remarks>
public class NotDefaultAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "The {0} field must not have the default value";

    public NotDefaultAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value)
    {
        //NotDefault doesn't necessarily mean required
        if (value is null)
        {
            return true;
        }

        Type? type = value.GetType();
        if (type.IsValueType)
        {
            object? defaultValue = Activator.CreateInstance(type);
            return !value.Equals(defaultValue);
        }

        // non-null ref type
        return true;
    }
}
