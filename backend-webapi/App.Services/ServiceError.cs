namespace Backend.WebApi.App.Services;

public record struct ServiceError(
    ServiceErrorKind Kind,
    string? Message = default,
    params Exception?[]? Exceptions
    );
