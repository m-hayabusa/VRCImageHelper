using Rug.Osc;
using System.Diagnostics;
using VRC.OSCQuery;

namespace VRCImageHelper
{
    public class OscEventArgs : EventArgs
    {
        public OscEventArgs(string path, string data)
        {
            Path = path;
            Data = data;
        }

        public string Path { get; }
        public string Data { get; }
    }
    public delegate void OscEventHandler(object sender, OscEventArgs e);

    internal class OscServer
    {
        private readonly CancellationToken cancellationToken;
        private bool _enabled = false;
        public bool Enable
        {
            set
            {
                if (value == _enabled) return;

                if (value)
                {
                    _port = Extensions.GetAvailableUdpPort();

                    _oscQuery = new OSCQueryServiceBuilder()
                        .WithTcpPort(Extensions.GetAvailableTcpPort())
                        .WithUdpPort(_port)
                        .WithServiceName("VRCImageHelper")
                        .AdvertiseOSC()
                        .AdvertiseOSCQuery()
                        .StartHttpServer()
                        .Build();

                    _oscQuery.AddEndpoint<string>("/avatar/change", Attributes.AccessValues.WriteOnly);
                    _oscQuery.AddEndpoint<int>("/avatar/parameters/VirtualLens2_Enable", Attributes.AccessValues.WriteOnly);
                    _oscQuery.AddEndpoint<float>("/avatar/parameters/VirtualLens2_Zoom", Attributes.AccessValues.WriteOnly);
                    _oscQuery.AddEndpoint<float>("/avatar/parameters/VirtualLens2_Aperture", Attributes.AccessValues.WriteOnly);

                    _oscQuery.OnOscServiceAdded += OscQuery_OnOscServiceAdded;

                    _oscQuery.RefreshServices();
                    Debug.WriteLine(_oscQuery.HostInfo);
                    Debug.WriteLine(_oscQuery.OscPort);

                    oscReceiver = new OscReceiver(_port);
                    oscReceiver.Connect();

                    watcher = new Task(() => oscReceiverWatcher());
                    watcher.Start();
                }
                else
                {
                    // close
                    watcher = null;
                    oscReceiver.Dispose();
                    _oscQuery.Dispose();
                }

                _enabled = value;
            }
            get
            {
                return _enabled;
            }
        }

        private int _port;
        private OSCQueryService _oscQuery;

        private void OscQuery_OnOscServiceAdded(OSCQueryServiceProfile profile)
        {
            watcher = new Task(() => oscReceiverWatcher());
            watcher.Start();
        }

        private OscReceiver oscReceiver;
        private Task? watcher;
        private void oscReceiverWatcher()
        {
            while (!cancellationToken.IsCancellationRequested && _enabled)
            {
                if (oscReceiver.TryReceive(out var packet))
                {
                    var a = packet.ToString()?.Split(',');
                    if (a is not null && a.Length >= 2)
                    {
                        var e = new OscEventArgs(a[0], a[1]);
                        Received?.Invoke(this, e);
                    }
                }
                else
                {
                    Task.Delay(200);
                }
            }
        }
        public event OscEventHandler? Received;
        public OscServer(CancellationToken token)
        {
            cancellationToken = token;
        }
    }
}
