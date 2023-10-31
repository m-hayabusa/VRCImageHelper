namespace VRCImageHelper
{
    public partial class ConfigWindow : Form
    {
        public ConfigWindow()
        {
            InitializeComponent();
            Config = new Config();

            Icon = new Icon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\icon.ico");
            comboBoxFileFormat.Items.AddRange(new object[] { "PNG", "JPEG", "AVIF" });
            comboBoxEncoder.Items.AddRange(new object[] { "libaom-av1", "av1_qsv", "av1_nvenc", "av1_amf" });
        }

        private Config Config;

        private void ConfigWindow_Load(object sender, EventArgs e)
        {
            Config = ConfigManager.Config;

            numericUpDownQuality.Value = Config.Quality;
            textBoxDir.Text = Config.DestDir;

            comboBoxFileFormat.SelectedItem = Config.Format;
            comboBoxEncoder.SelectedItem = Config.Encoder;

            textBoxFilePattern.Text = Config.FilePattern;
            textBoxEncoderOption.Text = Config.EncoderOption;

            comboBoxFileFormat_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void buttonSelectDir_Click(object sender, EventArgs e)
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
                    Config.DestDir = path;
                    textBoxDir.Text = path;
                }
            }
        }

        private string getFilePattern(string fileName)
        {
            fileName = fileName[..^Path.GetExtension(fileName).Length];
            var ext = comboBoxFileFormat.SelectedItem.ToString();
            if (ext is null) ext = "";
            fileName += "." + ext.ToLower();
            return fileName;
        }

        private void buttonResetFilePattern_Click(object sender, EventArgs e)
        {
            textBoxFilePattern.Text = getFilePattern(Config.Default.FilePattern);
        }

        private void comboBoxFileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxFilePattern.Text = getFilePattern(textBoxFilePattern.Text);
            switch (comboBoxFileFormat.SelectedItem)
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var format = comboBoxFileFormat.SelectedItem.ToString();
            if (format is not null) Config.Format = format;

            var encoder = comboBoxEncoder.SelectedItem.ToString();
            if (encoder is not null) Config.Encoder = encoder;

            Config.Quality = Convert.ToInt32(numericUpDownQuality.Value);
            Config.EncoderOption = textBoxEncoderOption.Text;
            Config.FilePattern = textBoxFilePattern.Text;

            ConfigManager.Save(Config);
            Dispose();
        }
    }
}
