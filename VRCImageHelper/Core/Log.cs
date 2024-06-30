namespace VRCImageHelper.Core;

using Microsoft.Extensions.Logging;

internal static partial class Log
{
    private static readonly ILoggerFactory s_loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddDebug();
    });

    public static ILogger GetLogger(string categoryName)
    {
        return s_loggerFactory.CreateLogger(categoryName);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting up...")]
    public static partial void Startup(ILogger logger);
}
