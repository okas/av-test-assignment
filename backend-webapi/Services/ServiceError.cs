namespace Backend.WebApi.Services;

public record struct ServiceError(
    ServiceErrorKind Kind,
    string? Message = default,
    params Exception?[]? Exceptions
    );
