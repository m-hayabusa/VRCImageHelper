namespace VRCImageHelper.Tools;

using FFMpegCore;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using VRCImageHelper.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class FFMpeg
{
    private static readonly ILogger s_logger = Log.GetLogger("FFMPEG");

    public static CancellationToken s_cancellationToken;
    private static string? s_ffMpegPath;
    private static bool s_ffMpegDownloading;
    public static string? ExecDir
    {
        get
        {
            return Path.GetDirectoryName(ExecPath);
        }
    }
    public static string? ExecPath
    {
        get
        {
            while (s_ffMpegDownloading)
            {
                Task.Delay(100).Wait();
                if (s_cancellationToken.IsCancellationRequested)
                    return "";
            }

            s_ffMpegPath ??= Executables.Find("ffmpeg.exe");

            return s_ffMpegPath;
        }
    }

    public static string Download()
    {
        while (s_ffMpegDownloading)
        {
            Task.Delay(100).Wait();
            var exists = Executables.Find("ffmpeg.exe");
            if (exists is not null)
                return exists;
            if (s_cancellationToken.IsCancellationRequested)
                return "";
        }
        s_ffMpegDownloading = true;

        var url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";
        var dir = Executables.Download("ffmpeg.exe", url, s_cancellationToken);

        var extracted = Directory.EnumerateDirectories(dir).First();
        foreach (var item in Directory.EnumerateFileSystemEntries(extracted))
        {
            if (File.Exists(item))
            {
                File.Move(item, dir + "\\" + Path.GetFileName(item));
            }
            else if (Directory.Exists(item))
            {
                Directory.Move(item, dir + "\\" + Path.GetFileName(item));
            }
        }
        Directory.Delete(extracted);

        var readme = new StreamWriter(File.Create(dir + "\\README.txt"));
        readme.Write($"this files are downloaded from https://github.com/BtbN/FFmpeg-Builds");
        readme.Close();

        s_supportedEncoder.Clear();

        s_ffMpegDownloading = false;

        return dir + "\\bin\\ffmpeg.exe";
    }

    private static readonly Dictionary<string, string[]> s_supportedEncoder = new();
    public static string[] GetSupportedEncoder(string format)
    {
        if (s_supportedEncoder.ContainsKey(format))
            return s_supportedEncoder[format];

        if (ExecPath is null)
            return Array.Empty<string>();

        var ffmpegStartInfo = new ProcessStartInfo(ExecPath) { Arguments = "-encoders", CreateNoWindow = true, RedirectStandardOutput = true };
        Process? ffmpeg;
        try
        {
            ffmpeg = Process.Start(ffmpegStartInfo);
        }
        catch
        {
            ffmpeg = null;
        }

        if (ffmpeg is null)
            return Array.Empty<string>();

        var result = new List<string>();

        while (!ffmpeg.HasExited)
        {
            if (ffmpeg.StandardOutput.BaseStream.CanRead)
            {
                var line = ffmpeg.StandardOutput.ReadLine();
                if (line is null) break;
                if (line.Contains($"(codec {format})"))
                {
                    result.Add(line[8..29].Trim());
                }
            }
        }

        var resultArray = result.ToArray();

        s_supportedEncoder.Add(format, resultArray);

        return resultArray;
    }

    public static async Task<bool> Encode(string src, string dest, string format, string encoder, int quality, string option)
    {
        Log.FFmpegEncode(s_logger, src, format);

        if (ExecDir is null)
        {
            Log.FFmpegEncodeFailed(s_logger, src, encoder, "ffmpegが存在せず、ダウンロードにも失敗しました");
            return false;
        }
        try
        {
            await FFMpegArguments
                .FromFileInput(src)
                .OutputToFile(dest, false, options =>
                {
                    options
                        .ForceFormat(format)
                        .WithVideoCodec(encoder);

                    switch (encoder)
                    {
                        case "libaom-av1":
                            options
                                .WithConstantRateFactor(quality);
                            break;
                        case "libsvtav1":
                            options
                                .WithConstantRateFactor(quality);
                            break;
                        case "av1_qsv":
                            options
                                .WithCustomArgument($"-q {quality}");
                            break;
                        case "av1_nvenc":
                            options
                                .WithCustomArgument($"-cq {quality}");
                            break;
                        case "av1_amf":
                            options
                                .WithCustomArgument($"-qp_i {quality}");
                            break;
                        case "libwebp":
                            options
                                .WithCustomArgument($"-quality {quality}");
                            break;
                    }
                    if (option != "")
                        options.WithCustomArgument(option);
                })
                .ProcessAsynchronously(true, new FFOptions() { BinaryFolder = ExecDir });
        }
        catch (Exception ex)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorFFMpeg, false, (e) =>
            {
                MessageBox.Show(ToastArguments.Parse(e.Argument).Get("Message"));
            }, new Dictionary<string, string> { { "Message", ex.Message } });
            File.Delete(dest);
            Log.FFmpegEncodeFailed(s_logger, src, encoder, ex.Message);
            return false;
        }
        Log.FFmpegEncodeEnd(s_logger, src, format);
        return true;
    }
}
