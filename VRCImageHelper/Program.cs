﻿namespace VRCImageHelper;

using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using VRCImageHelper.Core;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        if (Environment.GetCommandLineArgs().ToList() is var args && args is not null && args.Any())
        {
            if (args.Contains("--setup"))
            {
                new UI.ConfigWindow().ShowDialog();

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
                    }
                    catch (Exception)
                    {
                        var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
                        key ??= Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
                        key.SetValue("Uninstall VRChat-Exif-Writer", "C:\\Windows\\System32\\cmd.exe /c rmdir /q /s \"%AppData%\\Microsoft\\Windows\\Start Menu\\Programs\\VRChat-Exif-Writer\" \"%LocalAppData%\\Programs\\VRChat-Exif-Writer\"");
                        key.Dispose();
                        MessageBox.Show(Properties.Resources.SetupRemoveVEWRestartRequired);
                        return;
                    }
                }

                Process.Start(Application.ExecutablePath);
                return;
            }
            else if (args.Contains("--uninstall"))
            {
                Cleanup();
                return;
            }
            if (args.Contains("--scanAll"))
            {
                s_scanAll = true;
            }
        }
        var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
            .Where(p => p.Id != Environment.ProcessId);
        if (processes.Any())
        {
            return;
        }

        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            UI.SendNotify.Cleanup();
            return;
        }

        s_toolbarIcon = new UI.ToolbarIcon();

        CancelToken = new CancellationTokenSource();

        var bgTask = new Task(Background, CancelToken.Token);
        bgTask.Start();

        SystemEvents.SessionEnded += (_, _) => Exit();

        Application.Run();
    }

    private static UI.ToolbarIcon? s_toolbarIcon;
    public static void Exit()
    {
        CancelToken?.Cancel();
        Thread.Sleep(500);
        CancelToken?.Dispose();
        UI.SendNotify.Cleanup();
        Application.Exit();
    }

    private static void Cleanup()
    {
        var basePath = Path.GetDirectoryName(Application.ExecutablePath);
        if (Directory.Exists(basePath + "\\exiftool"))
        {
            Directory.Delete(basePath + "\\exiftool", true);
        }
        if (Directory.Exists(basePath + "\\ffmpeg"))
        {
            Directory.Delete(basePath + "\\ffmpeg", true);
        }
        if (File.Exists(basePath + "\\config.json"))
        {
            File.Delete(basePath + "\\config.json");
        }
        UI.SendNotify.Cleanup();
        AutoStart.Register(false);
    }
    private static bool s_scanAll;
    public static CancellationTokenSource? CancelToken { get; private set; }
    private static void Background()
    {
        if (CancelToken is null)
            return;

        var logReader = new LogReader(CancelToken.Token);
        var oscServer = new OscServer(CancelToken.Token);
        ImageProcess.s_cancellationToken = CancelToken.Token;
        Tools.ExifTool.s_cancellationToken = CancelToken.Token;
        Tools.FFMpeg.s_cancellationToken = CancelToken.Token;

        if (s_toolbarIcon is not null)
            logReader.ScanAllProgress += s_toolbarIcon.Scanning;

        logReader.NewLine += StateChecker.Taken;
        logReader.NewLine += StateChecker.WorldId;
        logReader.NewLine += StateChecker.JoinRoom;
        logReader.NewLine += StateChecker.PlayerJoin;
        logReader.NewLine += StateChecker.PlayerLeft;
        logReader.NewLine += StateChecker.Quit;

        oscServer.Received += StateChecker.VL2Enable;
        oscServer.Received += StateChecker.VL2Zoom;
        oscServer.Received += StateChecker.VL2Aperture;
        oscServer.Received += StateChecker.ChangeAvater;

        oscServer.Received += StateChecker.IntegralEnable;
        oscServer.Received += StateChecker.IntegralMode;
        oscServer.Received += StateChecker.IntegralZoom;
        oscServer.Received += StateChecker.IntegralAperture;
        oscServer.Received += StateChecker.IntegralShutterSpeed;
        oscServer.Received += StateChecker.IntegralExposure;
        oscServer.Received += StateChecker.IntegralBokehShape;

        logReader.Enable(ConfigManager.ScanAll || s_scanAll);
        oscServer.Enable();
    }
}
