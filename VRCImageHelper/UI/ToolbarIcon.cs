namespace VRCImageHelper.UI;

using System;
using System.Diagnostics;
using VRCImageHelper.Core;
using VRCImageHelper.Properties;
using VRCImageHelper.Utils;

internal class ToolbarMenu : ContextMenuStrip
{
    private readonly ToolStripMenuItem _autostart = new();
    private readonly ToolStripMenuItem _scanAll = new();
    private readonly ToolStripMenuItem _version = new();
    public ToolbarMenu()
    {
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

        _scanAll.Click += ScanAll_Click;

        _autostart.Text = Resources.ToolbarToggleAutoStart;

        _autostart.CheckState = AutoStart.IsRegistered() ? CheckState.Checked : CheckState.Unchecked;
        _autostart.CheckOnClick = true;
        _autostart.CheckStateChanged += new EventHandler(Autostart_Toggle);

        var label = new ToolStripLabel
        {
            Text = Resources.ToolbarTitle
        };
        label.Click += Label_Click;

        _version.Text = "v" + Application.ProductVersion.Split('+')[0];
        _version.Click += Version_Click;

        Items.AddRange(new ToolStripItem[] { label, _version, new ToolStripSeparator(), _autostart, _scanAll, settings, exit });
        Opening += (_, _) => UpdateScanAll();
    }

    private void Label_Click(object? sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "https://github.com/m-hayabusa/VRCImageHelper",
            UseShellExecute = true,
        });
    }
    private void Version_Click(object? sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "https://github.com/m-hayabusa/VRCImageHelper/releases",
            UseShellExecute = true,
        });
    }
    public delegate void ScanningUpdate();
    private void ScanAll_Click(object? sender, EventArgs e)
    {
        var dialog = MessageBox.Show(Resources.ScanAllRestartMessage, Resources.ScanAllRestartTitle, MessageBoxButtons.YesNo);
        if (dialog.Equals(DialogResult.Yes))
        {
            Program.Exit();
            Process.Start(Application.ExecutablePath, "--scanAll");
        }
    }
    private void UpdateScanAll()
    {
        if (_scanningTotal == 0)
        {
            _scanAll.Text = Resources.ToolbarScanAll;
            if (!_scanAll.Enabled)
                _scanAll.Click += ScanAll_Click;

            _scanAll.Enabled = true;
        }
        else
        {
            _scanAll.Text = Resources.ToolbarScanAllProgress
                .Replace("{Progress}", (_scanningProgress + 1).ToString())
                .Replace("{Total}", _scanningTotal.ToString());
            if (_scanAll.Enabled)
            {
                _scanAll.Click -= ScanAll_Click;
            }
            _scanAll.Enabled = false;
        }
    }
    private int _scanningProgress;
    private int _scanningTotal;
    public void Scanning(int progress, int total)
    {
        _scanningProgress = progress;
        _scanningTotal = total;

        if (Visible)
        {
            BeginInvoke(new ScanningUpdate(UpdateScanAll));
        }
    }

    public delegate void UpdateVersionString();
    public void FoundNewerVersion(string latest)
    {
        if (Visible)
        {
            BeginInvoke(new UpdateVersionString(() =>
            {
                _version.Text += " (latest: v" + latest + ")";
            }));
        }
        else
        {
            _version.Text += " (latest: v" + latest + ")";
        }
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

internal class ToolbarIcon
{
    private readonly ToolbarMenu _menu;
    public ToolbarIcon()
    {
        _menu = new ToolbarMenu();
        _ = new NotifyIcon
        {
            Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico"),
            Text = Resources.ToolbarTitle,
            Visible = true,
            ContextMenuStrip = _menu
        };
    }
    public void Scanning(object? sender, ScanAllProgressEventArgs e)
    {
        _menu.Scanning(e.Processing, e.Total);
    }
    public void FoundNewerVersion(string version)
    {
        _menu.FoundNewerVersion(version);
    }
}
