namespace VRCImageHelper.Tools;
using System;
using System.IO.Compression;
using System.Threading;

internal class Executables
{
    public static string Download(string fileName, string url, CancellationToken cancellationToken)
    {
        var destPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + Path.GetFileNameWithoutExtension(fileName);
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Windows NT (VRCImageHelper)");
        var request = client.GetStreamAsync(url, cancellationToken);
        request.Wait(cancellationToken);

        Directory.CreateDirectory(destPath);

        var archive = new ZipArchive(request.Result);
        archive.ExtractToDirectory(destPath);

        return destPath;
    }
    public static string? Find(string fileName)
    {
        var pathes = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process) + ";"
             + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) + ";"
             + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
        if (pathes is not null)
            foreach (var path in pathes.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }

        var destPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + Path.GetFileNameWithoutExtension(fileName);

        if (File.Exists(destPath + "\\" + fileName))
            return destPath + "\\" + fileName;

        return null;
    }
}
