namespace VRCImageHelper.Utils;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using VRCImageHelper.Core;

internal class VRCExifWriter
{
    private static readonly ILogger s_logger = Log.GetLogger("VRCExifWriter");

    /// <summary>
    /// VRChat Exif Writerを削除する
    /// </summary>
    /// <returns>完了するのに再起動が必要なら、True そうでなければ、False</returns>
    public static bool Remove()
    {
        var vrcExifWriterPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Programs\\VRChat-Exif-Writer";
        if (Directory.Exists(vrcExifWriterPath) && MessageBox.Show(Properties.Resources.SetupRemoveVEWMessage, Properties.Resources.SetupRemoveVEWTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            Process.Start("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", $"-ExecutionPolicy Bypass -Command \"{vrcExifWriterPath}\\utils\\stop.ps1\"").WaitForExit();
            var unregist = new ProcessStartInfo("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe") { Verb = "runAs", UseShellExecute = true, Arguments = $"-Command \"Unregister-ScheduledTask -Confirm:$false -TaskName:VRChat-Exif-Writer\"" };
            Process.Start(unregist);
            try
            {
                Directory.Delete(vrcExifWriterPath + "\\.git", true);
                Directory.Delete(vrcExifWriterPath, true);
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\VRChat-Exif-Writer", true);
                Log.VRCExifWriter_Removed(s_logger, false);
            }
            catch (Exception)
            {
                var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
                key ??= Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
                key.SetValue("Uninstall VRChat-Exif-Writer", "C:\\Windows\\System32\\cmd.exe /c rmdir /q /s \"%AppData%\\Microsoft\\Windows\\Start Menu\\Programs\\VRChat-Exif-Writer\" \"%LocalAppData%\\Programs\\VRChat-Exif-Writer\"");
                key.Dispose();
                MessageBox.Show(Properties.Resources.SetupRemoveVEWRestartRequired);
                Log.VRCExifWriter_Removed(s_logger, true);
                return true;
            }
        }
        return false;
    }
}
