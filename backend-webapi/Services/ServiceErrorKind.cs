namespace Backend.WebApi.Services;

/// <summary>
/// Kinds that indicate what went in Service wrong while called action.
/// </summary>
public enum ServiceErrorKind
{
    /// <summary>
    /// Entity not found result on change operation.
    /// </summary>
    /// <remarks>
    /// Not intended to use on query operations where entity is not found.
    /// </remarks>
    NotFoundOnChange,

    /// <summary>
    /// Entit already exists in database result.
    /// </summary>
    AlreadyExistsOnCreate,

    /// <summary>
    /// Something unknown and serious happened result.
    /// </summary>
    InternalError,
}
