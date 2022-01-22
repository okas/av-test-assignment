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
}
