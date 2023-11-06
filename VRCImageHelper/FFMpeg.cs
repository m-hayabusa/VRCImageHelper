namespace VRCImageHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

internal class FFMpeg
{
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

            if (s_ffMpegPath is null)
            {
                s_ffMpegPath = CheckExists();
            }

            return s_ffMpegPath;
        }
    }

    public static string Download()
    {
        while (s_ffMpegDownloading)
        {
            Task.Delay(100).Wait();
            var exists = CheckExists();
            if (exists is not null)
                return exists;
            if (s_cancellationToken.IsCancellationRequested)
                return "";
        }
        s_ffMpegDownloading = true;

        var url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";
        var destPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg";

        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Windows NT (VRCImageHelper)");
        var request = client.GetStreamAsync(url, s_cancellationToken);
        request.Wait();

        if (Directory.Exists(destPath))
            Directory.Delete(destPath, true);

        Directory.CreateDirectory(destPath);

        var archive = new ZipArchive(request.Result);
        archive.ExtractToDirectory(destPath);

        var extracted = Directory.EnumerateDirectories(destPath).First();
        foreach (var item in Directory.EnumerateFileSystemEntries(extracted))
        {
            if (File.Exists(item))
            {
                File.Move(item, destPath + "\\" + Path.GetFileName(item));
            }
            else if (Directory.Exists(item))
            {
                Directory.Move(item, destPath + "\\" + Path.GetFileName(item));
            }
        }
        Directory.Delete(extracted);

        var readme = new StreamWriter(File.Create(destPath + "\\README.txt"));
        readme.Write($"this files are downloaded from https://github.com/BtbN/FFmpeg-Builds");
        readme.Close();

        s_supportedEncoder.Clear();

        s_ffMpegDownloading = false;

        return destPath + "\\bin\\ffmpeg.exe";
    }

    private static string? CheckExists()
    {
        var fileName = "ffmpeg.exe";

        var destPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg";

        if (File.Exists(destPath + "\\bin\\" + fileName))
            return destPath + "\\bin\\" + fileName;

        var pathes = Environment.GetEnvironmentVariable("PATH");
        if (pathes is not null)
            foreach (var path in pathes.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
        return null;
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
}
