namespace VRCImageHelper.Core;

using Microsoft.Win32;

internal class AutoStart
{
    public static bool IsRegistered()
    {
        var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", false);

        if (key is null) return false;

        var val = (string?)key.GetValue(Application.ProductName);

        key.Dispose();

        return val is not null && val == Application.ExecutablePath;
    }

    public static void Register(bool value)
    {
        var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        key ??= Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (value)
        {
            key.SetValue(Application.ProductName, Application.ExecutablePath);
        }
        else if (key.GetValue(Application.ProductName) is not null)
        {
            key.DeleteValue(Application.ProductName);
        }
        key.Dispose();
    }
}
