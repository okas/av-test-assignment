namespace Backend.WebApi.CrossCutting.Logging;

public static partial class Log
{
    [LoggerMessage(
        1001,
        LogLevel.Information,
        "Created: {Model}")]
    public static partial void InformCreated(this ILogger logger, object Model);

    [LoggerMessage(
        1002,
        LogLevel.Information,
        "State changed: {Model}")]
    public static partial void InformChanged(this ILogger logger, object Model);
}
