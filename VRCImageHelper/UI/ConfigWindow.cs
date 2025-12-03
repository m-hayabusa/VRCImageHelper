namespace VRCImageHelper.UI;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
    private readonly Dictionary<string, string> _format = new();
    private readonly Dictionary<string, string> _selectedEncoder = new() {
        { "AVIF", "libaom-av1" }, { "WEBP", "libwebp" },
        { "AVIFAlpha", "libaom-av1" }, { "WEBPAlpha", "libwebp" }
    };
    private readonly Dictionary<string, string> _selectedEncoderOption = new();

    private string GetEncoderOptions(string encoder, bool hasAlpha)
    {
        var key = encoder + (hasAlpha ? "Alpha" : "");
        if (_selectedEncoderOption.ContainsKey(key))
            return _selectedEncoderOption[key];
        else
            return ConfigManager.DefaultEncoderOptions(encoder, hasAlpha);
    }

    private static type GetControl<type>(Control sender, string name)
    where type : Control
    {
        var root = sender.Parent.Parent.Controls;
        var typename = typeof(type).Name;
        typename = char.ToLower(typename[0]) + typename[1..];
        return (type)root.Find($"{typename}{name}", true)[0];
    }

    private void ConfigWindow_Load(object sender, EventArgs e)
    {
        _config = ConfigManager.GetConfig();

        _format.Add("", _config.Format);
        _format.Add("Alpha", _config.AlphaFormat);

        numericUpDownQuality.Value = _config.Quality;
        numericUpDownAlphaQuality.Value = _config.AlphaQuality;
        textBoxDir.Text = _config.DestDir;

        comboBoxFileFormat.SelectedItem = _config.Format;
        comboBoxEncoder.SelectedItem = _config.Encoder;

        richTextBoxFilePattern.Text = _config.FilePattern;
        HighlightMatches(richTextBoxFilePattern);
        textBoxEncoderOption.Text = _config.EncoderOption;

        comboBoxAlphaFileFormat.SelectedItem = _config.AlphaFormat;
        comboBoxAlphaEncoder.SelectedItem = _config.AlphaEncoder;

        richTextBoxAlphaFilePattern.Text = _config.AlphaFilePattern;
        HighlightMatches(richTextBoxAlphaFilePattern);
        textBoxAlphaEncoderOption.Text = _config.AlphaEncoderOption;

        checkBoxDeleteOriginal.CheckState = _config.DeleteOriginalFile ? CheckState.Checked : CheckState.Unchecked;
        checkBoxOverwriteDest.CheckState = _config.OverwriteDestinationFile ? CheckState.Checked : CheckState.Unchecked;
        numericUpDownParallel.Value = _config.ParallelCompressionProcesses;

        _selectedEncoder[_config.Format] = _config.Encoder;
        _selectedEncoder[_config.AlphaFormat + "Alpha"] = _config.AlphaEncoder;
        _selectedEncoderOption[_config.Encoder] = _config.EncoderOption;
        _selectedEncoderOption[_config.AlphaEncoder + "Alpha"] = _config.AlphaEncoderOption;

        ComboBoxFileFormat_SelectedIndexChanged(comboBoxFileFormat, EventArgs.Empty);
        ComboBoxFileFormat_SelectedIndexChanged(comboBoxAlphaFileFormat, EventArgs.Empty);
    }

    private static void HighlightMatches(RichTextBox richTextBox)
    {
        var foreColor = richTextBox.ForeColor;
        var validColor = Color.FromArgb(28, 109, 103);
        var invalidColor = Color.FromArgb(220, 50, 47);

        var currentScrollPosition = ScrollHelper.GetScrollPosition(richTextBox.Handle);

        // UI更新を一時停止 (下のSelectとかでチラつくので)
        richTextBox.SuspendLayout();
        RenderHelper.StopRender(richTextBox.Handle);

        var prevSelectionStart = richTextBox.SelectionStart;
        var prevSelectionLength = richTextBox.SelectionLength;

        var matches = Regex.Matches(richTextBox.Text, @"%\{.*?\}%");
        var keys = Placeholders.Keys();

        var lastTail = 0;
        foreach (Match match in matches)
        {
            if (!match.Success) break;
            richTextBox.Select(lastTail, match.Index - lastTail);
            richTextBox.SelectionColor = foreColor;
            richTextBox.Select(match.Index, match.Length);
            richTextBox.SelectionColor = keys.Contains(match.Value) ? validColor : invalidColor;
            lastTail = match.Index + match.Length;
        }

        richTextBox.Select(lastTail, richTextBox.TextLength - lastTail);
        richTextBox.SelectionColor = foreColor;

        ScrollHelper.SetScrollPosition(richTextBox.Handle, currentScrollPosition);

        // 元のキャレット位置を復元
        richTextBox.SelectionStart = prevSelectionStart;
        richTextBox.SelectionLength = prevSelectionLength;

        // UI更新を再開・再描画
        richTextBox.ResumeLayout();
        RenderHelper.StartRender(richTextBox.Handle);
        richTextBox.Invalidate();
    }

    private void ButtonSelectDir_Click(object sender, EventArgs e)
    {
        var selectDirectoryDialog = new SaveFileDialog()
        {
            Filter = "Directory|" + groupBoxSaveDir.Text,
            FileName = "image"
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
        var control = (Control)sender;

        var alpha = "";
        if (control.Name.Contains("Alpha")) alpha = "Alpha";

        var textBox = GetControl<RichTextBox>(control, $"{alpha}FilePattern");
        var fileFormat = GetControl<ComboBox>(control, $"{alpha}FileFormat");

        if (textBox is not null && fileFormat is not null)
        {
            var filePattern = alpha == "Alpha" ? Config.Default.AlphaFilePattern : Config.Default.FilePattern;
            var ext = fileFormat.SelectedItem.ToString()?.ToLower();
            if (ext is not null)
            {
                textBox.Text = Path.ChangeExtension(filePattern, ext);
            }
        }
    }

    private void ComboBoxFileFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        var control = (Control)sender;
        var alpha = "";
        if (control.Name.Contains("Alpha")) alpha = "Alpha";

        var textBox = GetControl<RichTextBox>(control, $"{alpha}FilePattern");
        var fileFormat = GetControl<ComboBox>(control, $"{alpha}FileFormat");
        var encoder = GetControl<ComboBox>(control, $"{alpha}Encoder");
        var encoderOption = GetControl<TextBox>(control, $"{alpha}EncoderOption");
        var quality = GetControl<NumericUpDown>(control, $"{alpha}Quality");

        var format = fileFormat.SelectedItem.ToString();

        if (format is not null && format != _format[alpha])
        {
            textBox.Text = Path.ChangeExtension(richTextBoxFilePattern.Text, format.ToLower());
            _format[alpha] = format;
        }

        if ((format == "AVIF" && FFMpeg.GetSupportedEncoder("av1").Length == 0)
            || (format == "WEBP" && FFMpeg.GetSupportedEncoder("webp").Length == 0))
        {
            var res = MessageBox.Show(Resources.FFMpegDownloadMessage, Resources.FFMpegDownloadTitle, MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                new DownloadProgressDialog().Download("ffmpeg", () =>
                {
                    try
                    {
                        FFMpeg.Download();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
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
        encoderOption.Text = GetEncoderOptions(encoder.Text, alpha == "Alpha");
    }

    private void ComboBoxEncoder_SelectedIndexChanged(object sender, EventArgs e)
    {
        var control = (Control)sender;

        var alpha = "";
        if (control.Name.Contains("Alpha")) alpha = "Alpha";

        var fileFormat = GetControl<ComboBox>(control, $"{alpha}FileFormat");
        var encoder = GetControl<ComboBox>(control, $"{alpha}Encoder");
        var encoderOption = GetControl<TextBox>(control, $"{alpha}EncoderOption");

        encoderOption.Text = GetEncoderOptions(encoder.Text, alpha == "Alpha");
        _selectedEncoder[fileFormat.Text + alpha] = encoder.Text;
    }

    private void TextBoxEncoderOption_TextChanged(object sender, EventArgs e)
    {
        var control = (Control)sender;

        var alpha = "";
        if (control.Name.Contains("Alpha")) alpha = "Alpha";

        var encoder = GetControl<ComboBox>(control, $"{alpha}Encoder");
        var encoderOption = GetControl<TextBox>(control, $"{alpha}EncoderOption");

        _selectedEncoderOption[encoder.Text + alpha] = encoderOption.Text;
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
        _config.ParallelCompressionProcesses = Convert.ToInt32(numericUpDownParallel.Value);
        _config.EncoderOption = textBoxEncoderOption.Text;
        _config.FilePattern = richTextBoxFilePattern.Text;
        _config.AlphaQuality = Convert.ToInt32(numericUpDownAlphaQuality.Value);
        _config.AlphaEncoderOption = textBoxAlphaEncoderOption.Text;
        _config.AlphaFilePattern = richTextBoxAlphaFilePattern.Text;

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

    private void RichTextBoxFilePattern_TextChanged(object sender, EventArgs e)
    {
        HighlightMatches(richTextBoxFilePattern);
    }

    private void RichTextBoxAlphaFilePattern_TextChanged(object sender, EventArgs e)
    {
        HighlightMatches(richTextBoxAlphaFilePattern);
    }
}


internal class ScrollHelper
{
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref Point lParam);

    public static Point GetScrollPosition(IntPtr hWnd)
    {
        var pos = new Point();
        SendMessage(hWnd, 0x04DD, 0, ref pos);
        return pos;
    }

    public static void SetScrollPosition(IntPtr hWnd, Point newPosition)
    {
        SendMessage(hWnd, 0x04DE, 0, ref newPosition);
    }
}

internal class RenderHelper
{
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

    public static void StopRender(IntPtr hWnd)
    {
        SendMessage(hWnd, 11, false, 0);
    }
    public static void StartRender(IntPtr hWnd)
    {
        SendMessage(hWnd, 11, true, 0);
    }
}
