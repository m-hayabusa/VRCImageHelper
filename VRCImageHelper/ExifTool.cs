namespace VRCImageHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

internal class ExifTool
{

    public static CancellationToken s_cancellationToken;
    private static string? s_exitToolPath;
    private static bool s_exitToolDownloading;
    private static string ExifToolPath
    {
        get
        {
            while (s_exitToolDownloading)
            {
                Task.Delay(100).Wait();
                if (s_cancellationToken.IsCancellationRequested)
                    return "";
            }

            if (s_exitToolPath is null)
            {
                s_exitToolDownloading = true;
                s_exitToolPath = CheckExistsAndDownload("exiftool.exe", "https://sourceforge.net/projects/exiftool/files/latest/download");
                s_exitToolDownloading = false;
            }

            return s_exitToolPath;
        }
    }

    private static string CheckExistsAndDownload(string fileName, string? url)
    {
        var destPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + Path.GetFileNameWithoutExtension(fileName);

        if (File.Exists(destPath + "\\" + fileName))
            return destPath + "\\" + fileName;

        var pathes = Environment.GetEnvironmentVariable("PATH");
        if (pathes is not null)
            foreach (var path in pathes.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }

        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Windows NT (VRCImageHelper)");
        var request = client.GetStreamAsync(url, s_cancellationToken);
        request.Wait();

        Directory.CreateDirectory(destPath);

        var archive = new ZipArchive(request.Result);
        archive.ExtractToDirectory(destPath);

        if (fileName == "exiftool.exe")
        {
            File.Move(destPath + "\\exiftool(-k).exe", destPath + "\\exiftool.exe");
        }

        return destPath + "\\" + fileName;
    }

    public static async Task<bool> ProcessImage(string path, List<string> args)
    {
        var argsFilePath = Path.GetTempFileName();
        var argsFile = new StreamWriter(argsFilePath);
        argsFile.Write(string.Join("\n", args));
        argsFile.Close();
        var exifTool = new ProcessStartInfo(ExifToolPath) { Arguments = path + " -@ " + argsFilePath, CreateNoWindow = true };

        Process? process;
        try
        {
            process = Process.Start(exifTool);
        }
        catch
        {
            return false;
        }

        if (process is null) return false;

        await process.WaitForExitAsync(s_cancellationToken);

        if (s_cancellationToken.IsCancellationRequested)
            return false;

        if (process.ExitCode == 0)
        {
            argsFile.Dispose();
            File.Delete(argsFilePath);
            return true;
        }
        return false;
    }
}
