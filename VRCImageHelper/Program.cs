namespace VRCImageHelper;

using Microsoft.Win32;
using System.Diagnostics;

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
                new ConfigWindow().ShowDialog();
                Process.Start(Application.ExecutablePath);
                return;
            }
            else if (args.Contains("--uninstall"))
            {
                Cleanup();
                return;
            }
        }

        var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
            .Where(p => p.Id != Environment.ProcessId);
        if (processes.Any())
        {
            return;
        }

        var toolbarIcon = new ToolbarIcon();
        toolbarIcon.CreateToolbarIcon();

        CancelToken = new CancellationTokenSource();

        var bgTask = new Task(Background, CancelToken.Token);
        bgTask.Start();

        SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        SystemEvents.SessionEnded += SystemEvents_SessionEnded;

        Application.Run();

    }

    private static void Cleanup()
    {
        var basePath = Path.GetDirectoryName(Application.ExecutablePath);
        Debug.WriteLine(basePath);
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

        var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (key is not null && key.GetValue(Application.ProductName) is not null)
        {
            key.DeleteValue(Application.ProductName);
        }

        key?.Dispose();
    }

    private static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
    {
        CancelToken?.Cancel();
        Task.Delay(500).Wait();
        CancelToken?.Dispose();
        Application.Exit();
    }

    private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
    {
        Debug.WriteLine(e.Reason.ToString());
    }

    public static CancellationTokenSource? CancelToken { get; private set; }
    private static void Background()
    {
        if (CancelToken is null)
            return;

        var logReader = new LogReader(CancelToken.Token);
        var oscServer = new OscServer(CancelToken.Token);
        ImageProcess.s_cancellationToken = CancelToken.Token;
        ExifTool.s_cancellationToken = CancelToken.Token;
        FFMpeg.s_cancellationToken = CancelToken.Token;

        logReader.NewLine += ImageProcess.Taken;
        logReader.NewLine += Info.WorldId;
        logReader.NewLine += Info.JoinRoom;
        logReader.NewLine += Info.PlayerJoin;
        logReader.NewLine += Info.PlayerLeft;
        logReader.NewLine += Info.Quit;

        oscServer.Received += Info.VL2Enable;
        oscServer.Received += Info.VL2Zoom;
        oscServer.Received += Info.VL2Aperture;
        oscServer.Received += Info.ChangeAvater;

        logReader.Enable = true;
        oscServer.Enable = true;
    }
}
