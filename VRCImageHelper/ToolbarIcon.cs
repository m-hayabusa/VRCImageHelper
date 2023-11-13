namespace VRCImageHelper;
internal class ToolbarIcon
{
    private readonly ToolStripMenuItem _autostart = new();

    public void CreateToolbarIcon()
    {
        var icon = new NotifyIcon
        {
            Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico"),
            Text = "VRChat Image Helper",
            Visible = true
        };

        var strip = new ContextMenuStrip();

        var exit = new ToolStripMenuItem
        {
            Text = "Exit"
        };
        exit.Click += new EventHandler(Exit_Click);

        var settings = new ToolStripMenuItem
        {
            Text = "Settings"
        };
        settings.Click += Settings_Click;

        _autostart.Text = "Run when Logon";

        _autostart.CheckState = new Func<CheckState>(() =>
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key is not null && key.GetValue(Application.ProductName) is not null)
            {
                return CheckState.Checked;
            }
            else
            {
                return CheckState.Unchecked;
            }
        })();
        _autostart.CheckOnClick = true;
        _autostart.CheckStateChanged += new EventHandler(Autostart_Toggle);

        var label = new ToolStripLabel
        {
            Text = "VRChat Image Helper"
        };

        strip.Items.AddRange(new ToolStripItem[] { label, new ToolStripSeparator(), _autostart, settings, exit });

        icon.ContextMenuStrip = strip;
    }
    private ConfigWindow? _configWindow;
    private void Settings_Click(object? sender, EventArgs e)
    {
        if (_configWindow is null || _configWindow.IsDisposed)
        {
            _configWindow = new ConfigWindow();
        }
        if (!_configWindow.Visible)
        {
            _configWindow.ShowDialog();
        }
    }

    private void Exit_Click(object? sender, EventArgs e)
    {
        Program.CancelToken?.Cancel();
        Thread.Sleep(500);
        Program.CancelToken?.Dispose();
        Application.Exit();
    }

    private void Autostart_Toggle(object? sender, EventArgs e)
    {
        var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        if (key is null)
        {
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        if (key is not null)
        {
            if (_autostart.Checked)
            {
                key.SetValue(Application.ProductName, Application.ExecutablePath);
            }
            else
            {
                if (key.GetValue(Application.ProductName) is not null)
                {
                    key.DeleteValue(Application.ProductName);
                }
            }
            key.Close();
        }
    }
}
