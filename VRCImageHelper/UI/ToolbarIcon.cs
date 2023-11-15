namespace VRCImageHelper.UI;

using VRCImageHelper.Core;
using VRCImageHelper.Properties;

internal class ToolbarIcon
{
    private readonly ToolStripMenuItem _autostart = new();

    public void CreateToolbarIcon()
    {
        var icon = new NotifyIcon
        {
            Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico"),
            Text = Resources.ToolbarTitle,
            Visible = true
        };

        var strip = new ContextMenuStrip();

        var exit = new ToolStripMenuItem
        {
            Text = Resources.ToolbarExit
        };
        exit.Click += new EventHandler(Exit_Click);

        var settings = new ToolStripMenuItem
        {
            Text = Resources.ToolbarSettings
        };
        settings.Click += Settings_Click;

        _autostart.Text = Resources.ToolbarToggleAutoStart;

        _autostart.CheckState = AutoStart.IsRegistered() ? CheckState.Checked : CheckState.Unchecked;
        _autostart.CheckOnClick = true;
        _autostart.CheckStateChanged += new EventHandler(Autostart_Toggle);

        var label = new ToolStripLabel
        {
            Text = Resources.ToolbarTitle
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
        Program.Exit();
    }

    private void Autostart_Toggle(object? sender, EventArgs e)
    {
        AutoStart.Register(!AutoStart.IsRegistered());
    }
}
