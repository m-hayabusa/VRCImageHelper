namespace VRCImageHelper;

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
        }
        }
        var toolbarIcon = new ToolbarIcon();
        toolbarIcon.CreateToolbarIcon();

        CancelToken = new CancellationTokenSource();

        var bgTask = new Task(Background, CancelToken.Token);
        bgTask.Start();

        Application.Run();
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
