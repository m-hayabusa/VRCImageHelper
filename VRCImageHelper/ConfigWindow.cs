namespace VRCImageHelper;
public partial class ConfigWindow : Form
{
    public ConfigWindow()
    {
        InitializeComponent();
        _config = new Config();

        Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico");
        comboBoxFileFormat.Items.AddRange(new object[] { "PNG", "JPEG", "AVIF" });
        comboBoxEncoder.Items.AddRange(new object[] { "libaom-av1", "av1_qsv", "av1_nvenc", "av1_amf" });
    }

    private Config _config;

    private void ConfigWindow_Load(object sender, EventArgs e)
    {
        _config = ConfigManager.Config;

        numericUpDownQuality.Value = _config.Quality;
        textBoxDir.Text = _config.DestDir;

        comboBoxFileFormat.SelectedItem = _config.Format;
        comboBoxEncoder.SelectedItem = _config.Encoder;

        textBoxFilePattern.Text = _config.FilePattern;
        textBoxEncoderOption.Text = _config.EncoderOption;

        ComboBoxFileFormat_SelectedIndexChanged(this, EventArgs.Empty);
    }

    private void ButtonSelectDir_Click(object sender, EventArgs e)
    {
        var selectDirectoryDialog = new SaveFileDialog()
        {
            Filter = "Directory|保存先のフォルダ",
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

    private string GetFilePattern(string fileName)
    {
        var ext = comboBoxFileFormat.SelectedItem.ToString() ?? "";
        return Path.ChangeExtension(fileName, ext.ToLower());
    }

    private void ButtonResetFilePattern_Click(object sender, EventArgs e)
    {
        textBoxFilePattern.Text = GetFilePattern(Config.Default.FilePattern);
    }

    private void ComboBoxFileFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        var format = comboBoxFileFormat.SelectedItem.ToString();
        if (format is not null && format != _config.Format)
        {
            textBoxFilePattern.Text = GetFilePattern(textBoxFilePattern.Text);
            _config.Format = format;
        }

        switch (format)
        {
            case "AVIF":
                comboBoxEncoder.Enabled = true;
                textBoxEncoderOption.Enabled = true;
                numericUpDownQuality.Enabled = true;
                break;
            case "JPEG":
                comboBoxEncoder.Enabled = false;
                textBoxEncoderOption.Enabled = false;
                numericUpDownQuality.Enabled = true;
                break;
            default:
                comboBoxEncoder.Enabled = false;
                textBoxEncoderOption.Enabled = false;
                numericUpDownQuality.Enabled = false;
                break;
        }
    }

    private void ButtonSave_Click(object sender, EventArgs e)
    {
        var format = comboBoxFileFormat.SelectedItem.ToString();
        if (format is not null) _config.Format = format;

        var encoder = comboBoxEncoder.SelectedItem.ToString();
        if (encoder is not null) _config.Encoder = encoder;

        _config.Quality = Convert.ToInt32(numericUpDownQuality.Value);
        _config.EncoderOption = textBoxEncoderOption.Text;
        _config.FilePattern = textBoxFilePattern.Text;

        ConfigManager.Save(_config);
        Dispose();
    }
}
