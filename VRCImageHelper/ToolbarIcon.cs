using System.Reflection;

namespace VRCImageHelper
{
    internal class ToolbarIcon
    {

        private readonly ToolStripMenuItem autostart = new();

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

            autostart.Text = "Run when Logon";

            autostart.CheckState = new Func<CheckState>(() =>
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key != null && key.GetValue(Application.ProductName) != null)
                    return CheckState.Checked;
                else
                    return CheckState.Unchecked;
            })();
            autostart.CheckOnClick = true;
            autostart.CheckStateChanged += new EventHandler(Autostart_Toggle);

            var label = new ToolStripLabel
            {
                Text = "VRChat Image Helper"
            };

            strip.Items.AddRange(new ToolStripItem[] { label, new ToolStripSeparator(), autostart, exit });

            icon.ContextMenuStrip = strip;
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            Program.Cts?.Cancel();
            Thread.Sleep(500);
            Program.Cts?.Dispose();
            Application.Exit();
        }

        private void Autostart_Toggle(object? sender, EventArgs e)
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key == null)
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (key != null)
            {
                if (autostart.Checked)
                {
                    key.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    if (key.GetValue(Application.ProductName) != null)
                    {
                        key.DeleteValue(Application.ProductName);
                    }
                }
                key.Close();
            }
        }
    }
}
