namespace Backend.WebApi.CrossCutting.Logging;

public static partial class Log
{
    [LoggerMessage(
        3001,
        LogLevel.Warning,
        "Not found: {Model}.")]
    public static partial void WarnNotFound(this ILogger logger, object Model, Exception? ex);

    [LoggerMessage(
        3002,
        LogLevel.Warning,
        "Already exists: {Model}.")]
    public static partial void WarnAlreadyExists(this ILogger logger, object Model, Exception? ex);

    [LoggerMessage(
        3003,
        LogLevel.Warning,
        "Operation detected optimistic concurrency during saving updates; now trying to resolve by {Remedy}, retry: {Retry}. Attempted changes: {Model}")]
    public static partial void WarnOptimisticConcurrencyDetection(this ILogger logger, string Remedy, int Retry, object Model, Exception? ex);

    [LoggerMessage(
        3999,
        LogLevel.Warning,
        "Operation was cancelled wtih reason {Reason}.")]
    public static partial void WarnOperationCancelledWithReason(this ILogger logger, string Reason, Exception? ex);
}
