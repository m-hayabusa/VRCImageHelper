namespace VRCImageHelper;

using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using VRCImageHelper.Core;
using VRCImageHelper.Core.StateChecker;
using VRCImageHelper.Utils;
using VRCImageHelper.Tools;

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
                new UI.DownloadProgressDialog().Download("exiftool", () =>
                {
                    ExifTool.GetExifTool();
                    return true;
                });
                new UI.ConfigWindow().ShowDialog();
                if (VRCExifWriter.Remove())
                    return;
                var logFile = LogReader.FindLogFile();
                if (logFile != null && logFile.Exists && logFile.CreationTime == logFile.LastWriteTime)
                {
                    UI.SendNotify.Send(Properties.Resources.NotifyErrorLogFileSeemsEmptyOnSetup, false);
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

    private static bool s_scanAll;
    public static CancellationTokenSource? CancelToken { get; private set; }
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

    private static void Background()
    {
        if (CancelToken is null)
            return;

        GitHub.GetLatestRelease().ContinueWith(task =>
        {
            if (task.Result.HasValue)
            {
                var latestVersion = task.Result.Value.tag_name;
                var version = Application.ProductVersion.Split('+')[0];
                if (latestVersion != version)
                {
                    s_toolbarIcon?.FoundNewerVersion(latestVersion);
                }
            }
        });

        var logReader = new LogReader(CancelToken.Token);
        var fileWatcher = new FileWatcher(CancelToken.Token);
        var oscServer = new OscServer(CancelToken.Token);
        ImageProcess.s_cancellationToken = CancelToken.Token;
        Tools.ExifTool.s_cancellationToken = CancelToken.Token;
        Tools.FFMpeg.s_cancellationToken = CancelToken.Token;

        if (s_toolbarIcon is not null)
            logReader.ScanAllProgress += s_toolbarIcon.Scanning;

        logReader.NewLine += LogReader.UpdateCurrentHead;

        logReader.NewLine += ImageProcess.Taken;

        logReader.NewLine += VRChat.WorldId;
        logReader.NewLine += VRChat.JoinRoom;
        logReader.NewLine += VRChat.PlayerJoin;
        logReader.NewLine += VRChat.PlayerLeft;
        logReader.NewLine += VRChat.Quit;

        oscServer.Received += VRChat.ChangeAvater;

        oscServer.Received += VirtualLens2.Initialize;
        oscServer.Received += VirtualLens2.Enable;
        oscServer.Received += VirtualLens2.Zoom;
        oscServer.Received += VirtualLens2.Aperture;
        oscServer.Received += VirtualLens2.Exposure;

        oscServer.Received += Integral.Enable;
        oscServer.Received += Integral.Mode;
        oscServer.Received += Integral.Zoom;
        oscServer.Received += Integral.Aperture;
        oscServer.Received += Integral.ShutterSpeed;
        oscServer.Received += Integral.Exposure;
        oscServer.Received += Integral.BokehShape;

        logReader.Enable(ConfigManager.ScanAll || s_scanAll);
        oscServer.Enable();
    }
}
