namespace VRCImageHelper.UI;

using System.Diagnostics;
using VRCImageHelper.Core;
using VRCImageHelper.Properties;
using VRCImageHelper.Tools;

public partial class ConfigWindow : Form
{
    public ConfigWindow()
    {
        InitializeComponent();
        _config = new Config();

        Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico");
        comboBoxFileFormat.Items.AddRange(new object[] { "PNG", "JPEG", "AVIF", "WEBP" });
        comboBoxAlphaFileFormat.Items.AddRange(new object[] { "PNG", "AVIF", "WEBP" });
    }

    private Config _config;
    private string _format = "";
    private readonly Dictionary<string, string> _selectedEncoder = new() {
        { "AVIF", "libaom-av1" }, { "WEBP", "libwebp" },
        { "AVIFAlpha", "libaom-av1" }, { "WEBPAlpha", "libwebp" }
    };
    private readonly Dictionary<string, string> _selectedEncoderOption = new();

    private void ConfigWindow_Load(object sender, EventArgs e)
    {
        _config = ConfigManager.GetConfig();

        _format = _config.Format;

        numericUpDownQuality.Value = _config.Quality;
        textBoxDir.Text = _config.DestDir;

        comboBoxFileFormat.SelectedItem = _config.Format;
        comboBoxEncoder.SelectedItem = _config.Encoder;

        textBoxFilePattern.Text = _config.FilePattern;
        textBoxEncoderOption.Text = _config.EncoderOption;

        comboBoxAlphaFileFormat.SelectedItem = _config.AlphaFormat;
        comboBoxAlphaEncoder.SelectedItem = _config.AlphaEncoder;

        textBoxAlphaFilePattern.Text = _config.AlphaFilePattern;
        textBoxAlphaEncoderOption.Text = _config.AlphaEncoderOption;

        checkBoxDeleteOriginal.CheckState = _config.DeleteOriginalFile ? CheckState.Checked : CheckState.Unchecked;
        checkBoxOverwriteDest.CheckState = _config.OverwriteDestinationFile ? CheckState.Checked : CheckState.Unchecked;

        _selectedEncoder[_config.Format] = _config.Encoder;
        _selectedEncoder[_config.AlphaFormat + "Alpha"] = _config.AlphaEncoder;
        _selectedEncoderOption[_config.Format] = _config.EncoderOption;
        _selectedEncoderOption[_config.Format + "Alpha"] = _config.AlphaEncoderOption;

        ComboBoxFileFormat_SelectedIndexChanged(comboBoxFileFormat, EventArgs.Empty);
        ComboBoxFileFormat_SelectedIndexChanged(comboBoxAlphaFileFormat, EventArgs.Empty);
    }

    private void ButtonSelectDir_Click(object sender, EventArgs e)
    {
        var selectDirectoryDialog = new SaveFileDialog()
        {
            Filter = "Directory|" + groupBoxSaveDir.Text,
            FileName = Path.GetFileName(textBoxFilePattern.Text)
        };
        if (selectDirectoryDialog is not null && selectDirectoryDialog.ShowDialog() == DialogResult.OK)
        {
            var path = Path.GetDirectoryName(selectDirectoryDialog.FileName);
            if (path is not null)
            {
                _config.DestDir = path;
                textBoxDir.Text = path;
            }
        }
    }


    private void ButtonResetFilePattern_Click(object sender, EventArgs e)
    {
        var alpha = false;
        if (((Control)sender).Name.Contains("Alpha")) alpha = true;

        var controls = ((Control)sender).Parent.Parent.Controls;
        var textBox = (TextBox)controls.Find("textBoxFilePattern", true)[0];
        var comboBox = (ComboBox)controls.Find("fileFormat", true)[0];

        if (textBox is not null && comboBox is not null)
        {
            var filePattern = alpha ? Config.Default.AlphaFilePattern : Config.Default.FilePattern;
            var ext = comboBox.SelectedItem.ToString()?.ToLower();
            if (ext is not null)
            {
                textBox.Text = Path.ChangeExtension(filePattern, ext);
            }
        }
    }
    public delegate void FFMpegDownloadEnd();
    private void ComboBoxFileFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        var alpha = "";
        if (((Control)sender).Name.Contains("Alpha")) alpha = "Alpha";
        var controls = ((Control)sender).Parent.Parent.Controls;
        var textBox = (TextBox)controls.Find($"textBox{alpha}FilePattern", true)[0];
        var fileFormat = (ComboBox)controls.Find($"comboBox{alpha}FileFormat", true)[0];
        var encoder = (ComboBox)controls.Find($"comboBox{alpha}Encoder", true)[0];
        var encoderOption = (TextBox)controls.Find($"textBox{alpha}EncoderOption", true)[0];
        var quality = (NumericUpDown)controls.Find($"numericUpDown{alpha}Quality", true)[0];

        var format = fileFormat.SelectedItem.ToString();

        if (format is not null && format != _format)
        {
            textBox.Text = Path.ChangeExtension(textBoxFilePattern.Text, format.ToLower());
            _format = format;
        }

        if ((format == "AVIF" && FFMpeg.GetSupportedEncoder("av1").Length == 0)
            || (format == "WEBP" && FFMpeg.GetSupportedEncoder("webp").Length == 0))
        {
            var res = MessageBox.Show(Resources.FFMpegDownloadMessage, Resources.FFMpegDownloadTitle, MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                var downloadingDialog = new DownloadProgressDialog()
                {
                    Text = "Downloading...",
                };
                var downloading = true;
                downloadingDialog.FormClosing += (sender, e) =>
                {
                    if (downloading)
                        e.Cancel = true;
                };
                downloadingDialog.Load += (sender, e) =>
                {
                    new Task(() =>
                    {
                        FFMpeg.Download();
                        BeginInvoke(new FFMpegDownloadEnd(() =>
                        {
                            downloading = false;
                            downloadingDialog.Close();
                        }));
                    }).Start();
                };
                downloadingDialog.ShowDialog();
            }
        }

        encoder.Items.Clear();
        switch (format)
        {
            case "AVIF":
                encoder.Enabled = true;
                encoderOption.Enabled = true;
                quality.Enabled = true;
                encoder.Items.AddRange(FFMpeg.GetSupportedEncoder("av1"));
                encoder.SelectedItem = _selectedEncoder["AVIF" + alpha];
                break;
            case "WEBP":
                encoder.Enabled = true;
                encoderOption.Enabled = true;
                quality.Enabled = true;
                encoder.Items.AddRange(FFMpeg.GetSupportedEncoder("webp"));
                encoder.SelectedItem = _selectedEncoder["WEBP" + alpha];
                break;
            case "JPEG":
                encoder.Enabled = false;
                encoderOption.Enabled = false;
                quality.Enabled = true;
                break;
            default:
                encoder.Enabled = false;
                encoderOption.Enabled = false;
                quality.Enabled = false;
                break;
        }
        encoderOption.Text = ConfigManager.DefaultEncoderOptions(encoder.Text, alpha != "");
    }

    private void ButtonSave_Click(object sender, EventArgs e)
    {
        var format = comboBoxFileFormat.SelectedItem.ToString();
        if (format is not null) _config.Format = format;

        var encoder = comboBoxEncoder.SelectedItem?.ToString();
        if (encoder is not null) _config.Encoder = encoder;

        var alphaFormat = comboBoxAlphaFileFormat.SelectedItem.ToString();
        if (alphaFormat is not null) _config.AlphaFormat = alphaFormat;

        var alphaEncoder = comboBoxAlphaEncoder.SelectedItem?.ToString();
        if (alphaEncoder is not null) _config.AlphaEncoder = alphaEncoder;

        _config.Quality = Convert.ToInt32(numericUpDownQuality.Value);
        _config.EncoderOption = textBoxEncoderOption.Text;
        _config.FilePattern = textBoxFilePattern.Text;
        _config.AlphaQuality = Convert.ToInt32(numericUpDownAlphaQuality.Value);
        _config.AlphaEncoderOption = textBoxAlphaEncoderOption.Text;
        _config.AlphaFilePattern = textBoxAlphaFilePattern.Text;

        _config.DeleteOriginalFile = checkBoxDeleteOriginal.CheckState == CheckState.Checked;
        _config.OverwriteDestinationFile = checkBoxOverwriteDest.CheckState == CheckState.Checked;

        if (_config.DestDir == "" && !_config.OverwriteDestinationFile &&
            ((_config.FilePattern == Config.Default.FilePattern) || (_config.AlphaFilePattern == Config.Default.AlphaFilePattern)))
        {
            MessageBox.Show(Resources.ConfigWindowOverwriteNotAllowedMessage, Resources.ConfigWindowOverwriteNotAllowedTitle);
            return;
        }

        ConfigManager.Save(_config);
        Dispose();
    }

    private void ButtonCancel_Click(object sender, EventArgs e)
    {
        Dispose();
    }

    private void ComboBoxEncoder_SelectedIndexChanged(object sender, EventArgs e)
    {
        Debug.WriteLine("ComboBoxEncoder_SelectedIndexChanged");
        var alpha = "";
        if (((Control)sender).Name.Contains("Alpha")) alpha = "Alpha";
        var controls = ((Control)sender).Parent.Parent.Controls;
        var fileFormat = (ComboBox)controls.Find($"comboBox{alpha}FileFormat", true)[0];
        var encoder = (ComboBox)controls.Find($"comboBox{alpha}Encoder", true)[0];
        var encoderOption = (TextBox)controls.Find($"textBox{alpha}EncoderOption", true)[0];

        encoderOption.Text = _selectedEncoderOption.ContainsKey(encoder.Text + alpha) ?
            _selectedEncoderOption[encoder.Text + alpha] :
            ConfigManager.DefaultEncoderOptions(encoder.Text, alpha != "");
        _selectedEncoder[fileFormat.Text + alpha] = encoder.Text;
    }

    private void TextBoxEncoderOption_TextChanged(object sender, EventArgs e)
    {
        Debug.WriteLine("TextBoxEncoderOption_TextChanged");
        var alpha = "";
        if (((Control)sender).Name.Contains("Alpha")) alpha = "Alpha";
        var controls = ((Control)sender).Parent.Parent.Controls;
        var encoder = (ComboBox)controls.Find($"comboBox{alpha}Encoder", true)[0];
        var encoderOption = (TextBox)controls.Find($"textBox{alpha}EncoderOption", true)[0];
        _selectedEncoderOption[encoder.Text + alpha] = encoderOption.Text;
    }
}
