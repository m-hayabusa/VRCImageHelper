namespace VRCImageHelper.Tools;

using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class ExifTool
{
    public static CancellationToken s_cancellationToken;
    private static string? s_exitToolPath;
    private static bool s_exitToolDownloading;
    private static string GetExifTool()
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
    private static string CheckExistsAndDownload(string fileName, string url)
    {
        var path = Executables.Find("exiftool.exe");

        if (path is not null) return path;

        var dir = Executables.Download(fileName, url, s_cancellationToken);
        File.Move(dir + "\\exiftool(-k).exe", dir + "\\exiftool.exe");

        return dir + "\\exiftool.exe";
    }

    public static async Task<bool> Write(string path, List<string> args)
    {
        var argsFilePath = Path.GetTempFileName();
        var argsFile = new StreamWriter(argsFilePath);
        argsFile.Write(string.Join("\n", args));
        argsFile.Close();
        var exifTool = new ProcessStartInfo(GetExifTool()) { Arguments = path + " -@ " + argsFilePath, CreateNoWindow = true };

        Process? process;
        try
        {
            process = Process.Start(exifTool);
        }
        catch (Exception ex)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyError + ":\n" + ex.Message, false, (e) =>
            {
                MessageBox.Show(ToastArguments.Parse(e.Argument).Get("Message"));
            }, new Dictionary<string, string> { { "Message", ex.Message } });
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
        else
        {
            var log = process.StandardOutput.ReadToEnd();
            UI.SendNotify.Send(Properties.Resources.NotifyErrorExiftool + ":\n", false, (e) =>
            {
                MessageBox.Show(ToastArguments.Parse(e.Argument).Get("Message"));
            }, new Dictionary<string, string> { { "Message", log } });
            return false;
        }
    }
}
