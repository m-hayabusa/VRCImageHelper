using System.Diagnostics;

namespace VRCImageHelper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var toolbarIcon = new ToolbarIcon();
            toolbarIcon.CreateToolbarIcon();

            Cts = new CancellationTokenSource();

            var bgTask = new Task(background, Cts.Token);
            bgTask.Start();

            Application.Run();
        }

        public static CancellationTokenSource? Cts { get; private set; }
        static void background()
        {
            if (Cts == null) return;

            var logReader = new LogReader(Cts.Token);
            var oscServer = new OscServer(Cts.Token);

            logReader.NewLine += ImageProcess.Taken;
            logReader.NewLine += Info.WorldId;
            logReader.NewLine += Info.JoinRoom;
            logReader.NewLine += Info.PlayerJoin;
            logReader.NewLine += Info.PlayerLeft;
            logReader.NewLine += Info.Quit;

            oscServer.Received += Info.VL2Control;
            oscServer.Received += Info.VL2Zoom;
            oscServer.Received += Info.VL2Aperture;
            oscServer.Received += Info.ChangeAvater;

            logReader.Enable = true;
            oscServer.Enable = true;
        }
    }
}