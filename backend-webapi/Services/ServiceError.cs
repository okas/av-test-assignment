namespace Backend.WebApi.Services;

public record ServiceError(
    ServiceErrorKind Kind,
    string? Message = default,
    params Exception?[]? Exceptions
    );
