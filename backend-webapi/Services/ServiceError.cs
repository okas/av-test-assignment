namespace Backend.WebApi.Services;

public record ServiceError(
    ServiceResultType ResultType,
    string? Message = default,
    params Exception?[]? Exceptions
    );
