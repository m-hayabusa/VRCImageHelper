namespace VRCImageHelper.UI;
using System.Windows.Forms;
using VRCImageHelper.Properties;

public partial class DownloadProgressDialog : Form
{
    public DownloadProgressDialog()
    {
        InitializeComponent();
    }

    public delegate void DownloadEnd();
    public void Download(string target, Func<bool> download)
    {
        Text = "Downloading " + target + "...";
        var downloading = true;
        FormClosing += (sender, e) =>
        {
            if (downloading)
                e.Cancel = true;
        };
        Load += (sender, e) =>
        {
            new Task(() =>
            {
                download();
                BeginInvoke(new DownloadEnd(() =>
                {
                    downloading = false;
                    Close();
                }));
            }).Start();
        };
        ShowDialog();
    }
}
