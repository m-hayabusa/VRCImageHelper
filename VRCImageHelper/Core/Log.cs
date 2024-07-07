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

    [LoggerMessage(Level = LogLevel.Information, Message = "Exit (Restart)")]
    public static partial void Restart(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "初期設定")]
    public static partial void Setup(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "VRChat Exif Writerを削除 / 再起動: {restart}")]
    public static partial void VRCExifWriter_Removed(ILogger logger, bool restart);


    [LoggerMessage(Level = LogLevel.Information, Message = "開始: {fileName}")]
    public static partial void Start(ILogger logger, string fileName);
    [LoggerMessage(Level = LogLevel.Information, Message = "処理: {type}; {fileName}")]
    public static partial void Process(ILogger logger, string fileName, string type);
    [LoggerMessage(Level = LogLevel.Information, Message = "完了: {fileName}")]
    public static partial void Done(ILogger logger, string fileName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "失敗: {log}; {fileName}")]
    public static partial void Failed(ILogger logger, string fileName, string log);


    [LoggerMessage(Level = LogLevel.Information, Message = "({encoder}): 開始: {fileName}")]
    public static partial void FFmpegEncode(ILogger logger, string fileName, string encoder);
    [LoggerMessage(Level = LogLevel.Information, Message = "({encoder}): 完了: {fileName}")]
    public static partial void FFmpegEncodeEnd(ILogger logger, string fileName, string encoder);

    [LoggerMessage(Level = LogLevel.Warning, Message = "(ffmpeg {encoder}): 失敗: {log}; {fileName}")]
    public static partial void FFmpegEncodeFailed(ILogger logger, string fileName, string encoder, string log);


    [LoggerMessage(Level = LogLevel.Information, Message = "発見: {fileName}")]
    public static partial void ImageProcess(ILogger logger, string fileName);

    [LoggerMessage(Level = LogLevel.Information, Message = "開始: {fileName}")]
    public static partial void ImageProcessStart(ILogger logger, string fileName);

    [LoggerMessage(Level = LogLevel.Information, Message = "圧縮: 開始: {format}, {quality}: {fileName}")]
    public static partial void ImageProcessCompressionStart(ILogger logger, string fileName, string format, int quality);
    [LoggerMessage(Level = LogLevel.Information, Message = "圧縮: 終了: {sourceFile} -> {destFile}")]
    public static partial void ImageProcessCompressionEnd(ILogger logger, string sourceFile, string destFile);
}
