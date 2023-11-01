namespace VRCImageHelper;

using Rug.Osc;
using System.Diagnostics;
using VRC.OSCQuery;

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
    private readonly CancellationToken _cancellationToken;
    private bool _enabled;
    public bool Enable
    {
        set
        {
            if (value == _enabled)
                return;

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

                _oscReceiver = new OscReceiver(_port);
                _oscReceiver.Connect();

                _oscWatcher = new Task(() => OscReceiverWatcher());
                _oscWatcher.Start();
            }
            else
            {
                // close
                _oscWatcher = null;
                _oscReceiver?.Dispose();
                _oscQuery?.Dispose();
            }

            _enabled = value;
        }
        get
        {
            return _enabled;
        }
    }

    private int _port;
    private OSCQueryService? _oscQuery;

    private void OscQuery_OnOscServiceAdded(OSCQueryServiceProfile profile)
    {
        _oscWatcher = new Task(() => OscReceiverWatcher());
        _oscWatcher.Start();
    }

    private OscReceiver? _oscReceiver;
    private Task? _oscWatcher;
    private void OscReceiverWatcher()
    {
        while (!_cancellationToken.IsCancellationRequested && _enabled)
        {
            if (_oscReceiver is not null && _oscReceiver.TryReceive(out var packet))
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
                Task.Delay(200).Wait();
            }
        }
    }
    public event OscEventHandler? Received;
    public OscServer(CancellationToken token)
    {
        _cancellationToken = token;
    }
}
