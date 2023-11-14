namespace VRCImageHelper.UI;

partial class ConfigWindow
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        buttonSave = new Button();
        buttonCancel = new Button();
        bottomPanel = new FlowLayoutPanel();
        groupBoxSaveDir = new GroupBox();
        panelSaveDir = new Panel();
        labelSaveDir = new Label();
        buttonSelectDir = new Button();
        textBoxDir = new TextBox();
        groupBoxFileFormat = new GroupBox();
        panelFilePattern = new Panel();
        labelFilePattern = new Label();
        textBoxFilePattern = new TextBox();
        buttonResetFilePattern = new Button();
        panelFileFormat = new Panel();
        labelFileFormat = new Label();
        comboBoxFileFormat = new ComboBox();
        comboBoxEncoder = new ComboBox();
        labelQuality = new Label();
        numericUpDownQuality = new NumericUpDown();
        labelEncoderOption = new Label();
        textBoxEncoderOption = new TextBox();
        groupBoxAlphaFileFormat = new GroupBox();
        panelAlphaFilePattern = new Panel();
        labelAlphaFilePattern = new Label();
        textBoxAlphaFilePattern = new TextBox();
        buttonResetAlphaFilePattern = new Button();
        panelAlphaFormat = new Panel();
        labelAlphaFormat = new Label();
        comboBoxAlphaFileFormat = new ComboBox();
        comboBoxAlphaEncoder = new ComboBox();
        labelAlphaQuality = new Label();
        numericUpDownAlphaQuality = new NumericUpDown();
        labelAlphaEncoderOption = new Label();
        textBoxAlphaEncoderOption = new TextBox();
        mainTableLayoutPanel = new TableLayoutPanel();
        buttonQualityTest = new Button();
        bottomPanel.SuspendLayout();
        groupBoxSaveDir.SuspendLayout();
        panelSaveDir.SuspendLayout();
        groupBoxFileFormat.SuspendLayout();
        panelFilePattern.SuspendLayout();
        panelFileFormat.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownQuality).BeginInit();
        groupBoxAlphaFileFormat.SuspendLayout();
        panelAlphaFilePattern.SuspendLayout();
        panelAlphaFormat.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownAlphaQuality).BeginInit();
        mainTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // buttonSave
        // 
        buttonSave.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonSave.AutoSize = true;
        buttonSave.Location = new Point(449, 6);
        buttonSave.Margin = new Padding(5, 6, 5, 6);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(75, 25);
        buttonSave.TabIndex = 100;
        buttonSave.Text = "保存";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += ButtonSave_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonCancel.AutoSize = true;
        buttonCancel.Location = new Point(534, 6);
        buttonCancel.Margin = new Padding(5, 6, 5, 6);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(75, 25);
        buttonCancel.TabIndex = 110;
        buttonCancel.Text = "キャンセル";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // bottomPanel
        // 
        bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        bottomPanel.Controls.Add(buttonCancel);
        bottomPanel.Controls.Add(buttonSave);
        bottomPanel.Dock = DockStyle.Bottom;
        bottomPanel.FlowDirection = FlowDirection.RightToLeft;
        bottomPanel.Location = new Point(0, 281);
        bottomPanel.Name = "bottomPanel";
        bottomPanel.Padding = new Padding(0, 0, 10, 0);
        bottomPanel.Size = new Size(624, 40);
        bottomPanel.TabIndex = 4;
        // 
        // groupBoxSaveDir
        // 
        groupBoxSaveDir.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        groupBoxSaveDir.AutoSize = true;
        groupBoxSaveDir.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        groupBoxSaveDir.Controls.Add(panelSaveDir);
        groupBoxSaveDir.Location = new Point(5, 5);
        groupBoxSaveDir.Margin = new Padding(5);
        groupBoxSaveDir.Name = "groupBoxSaveDir";
        groupBoxSaveDir.Padding = new Padding(3, 3, 3, 30);
        groupBoxSaveDir.Size = new Size(614, 55);
        groupBoxSaveDir.TabIndex = 2;
        groupBoxSaveDir.TabStop = false;
        groupBoxSaveDir.Text = "保存先";
        // 
        // panelSaveDir
        // 
        panelSaveDir.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panelSaveDir.AutoSize = true;
        panelSaveDir.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelSaveDir.BackColor = SystemColors.Control;
        panelSaveDir.Controls.Add(labelSaveDir);
        panelSaveDir.Controls.Add(buttonSelectDir);
        panelSaveDir.Controls.Add(textBoxDir);
        panelSaveDir.Location = new Point(5, 20);
        panelSaveDir.Margin = new Padding(0);
        panelSaveDir.MinimumSize = new Size(0, 30);
        panelSaveDir.Name = "panelSaveDir";
        panelSaveDir.Size = new Size(600, 30);
        panelSaveDir.TabIndex = 0;
        // 
        // labelSaveDir
        // 
        labelSaveDir.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelSaveDir.AutoSize = true;
        labelSaveDir.Location = new Point(6, 7);
        labelSaveDir.Name = "labelSaveDir";
        labelSaveDir.Size = new Size(43, 15);
        labelSaveDir.TabIndex = 0;
        labelSaveDir.Text = "保存先";
        // 
        // buttonSelectDir
        // 
        buttonSelectDir.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonSelectDir.Location = new Point(520, 3);
        buttonSelectDir.Name = "buttonSelectDir";
        buttonSelectDir.Size = new Size(75, 23);
        buttonSelectDir.TabIndex = 0;
        buttonSelectDir.Text = "フォルダ選択";
        buttonSelectDir.UseVisualStyleBackColor = true;
        buttonSelectDir.Click += ButtonSelectDir_Click;
        // 
        // textBoxDir
        // 
        textBoxDir.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxDir.Location = new Point(65, 3);
        textBoxDir.Name = "textBoxDir";
        textBoxDir.ReadOnly = true;
        textBoxDir.Size = new Size(450, 23);
        textBoxDir.TabIndex = 1;
        textBoxDir.TabStop = false;
        // 
        // groupBoxFileFormat
        // 
        groupBoxFileFormat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        groupBoxFileFormat.AutoSize = true;
        groupBoxFileFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        groupBoxFileFormat.Controls.Add(panelFilePattern);
        groupBoxFileFormat.Controls.Add(panelFileFormat);
        groupBoxFileFormat.Location = new Point(5, 70);
        groupBoxFileFormat.Margin = new Padding(5);
        groupBoxFileFormat.Name = "groupBoxFileFormat";
        groupBoxFileFormat.Padding = new Padding(3, 3, 3, 30);
        groupBoxFileFormat.Size = new Size(614, 85);
        groupBoxFileFormat.TabIndex = 2;
        groupBoxFileFormat.TabStop = false;
        groupBoxFileFormat.Text = "保存形式";
        // 
        // panelFilePattern
        // 
        panelFilePattern.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panelFilePattern.AutoSize = true;
        panelFilePattern.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelFilePattern.BackColor = SystemColors.Control;
        panelFilePattern.Controls.Add(labelFilePattern);
        panelFilePattern.Controls.Add(textBoxFilePattern);
        panelFilePattern.Controls.Add(buttonResetFilePattern);
        panelFilePattern.Location = new Point(5, 20);
        panelFilePattern.Margin = new Padding(0);
        panelFilePattern.MinimumSize = new Size(0, 30);
        panelFilePattern.Name = "panelFilePattern";
        panelFilePattern.Size = new Size(600, 30);
        panelFilePattern.TabIndex = 1;
        // 
        // labelFilePattern
        // 
        labelFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelFilePattern.AutoSize = true;
        labelFilePattern.Location = new Point(6, 7);
        labelFilePattern.Name = "labelFilePattern";
        labelFilePattern.Size = new Size(53, 15);
        labelFilePattern.TabIndex = 0;
        labelFilePattern.Text = "ファイル名";
        // 
        // textBoxFilePattern
        // 
        textBoxFilePattern.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxFilePattern.Location = new Point(65, 3);
        textBoxFilePattern.Name = "textBoxFilePattern";
        textBoxFilePattern.Size = new Size(450, 23);
        textBoxFilePattern.TabIndex = 10;
        // 
        // buttonResetFilePattern
        // 
        buttonResetFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonResetFilePattern.Location = new Point(520, 3);
        buttonResetFilePattern.Name = "buttonResetFilePattern";
        buttonResetFilePattern.Size = new Size(75, 23);
        buttonResetFilePattern.TabIndex = 15;
        buttonResetFilePattern.Text = "リセット";
        buttonResetFilePattern.UseVisualStyleBackColor = true;
        buttonResetFilePattern.Click += ButtonResetFilePattern_Click;
        // 
        // panelFileFormat
        // 
        panelFileFormat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panelFileFormat.AutoSize = true;
        panelFileFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelFileFormat.BackColor = SystemColors.Control;
        panelFileFormat.Controls.Add(labelFileFormat);
        panelFileFormat.Controls.Add(comboBoxFileFormat);
        panelFileFormat.Controls.Add(comboBoxEncoder);
        panelFileFormat.Controls.Add(labelQuality);
        panelFileFormat.Controls.Add(numericUpDownQuality);
        panelFileFormat.Controls.Add(labelEncoderOption);
        panelFileFormat.Controls.Add(textBoxEncoderOption);
        panelFileFormat.Location = new Point(5, 50);
        panelFileFormat.Margin = new Padding(0);
        panelFileFormat.MinimumSize = new Size(0, 30);
        panelFileFormat.Name = "panelFileFormat";
        panelFileFormat.Size = new Size(600, 30);
        panelFileFormat.TabIndex = 2;
        // 
        // labelFileFormat
        // 
        labelFileFormat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelFileFormat.AutoSize = true;
        labelFileFormat.Location = new Point(6, 7);
        labelFileFormat.Name = "labelFileFormat";
        labelFileFormat.Size = new Size(31, 15);
        labelFileFormat.TabIndex = 0;
        labelFileFormat.Text = "形式";
        // 
        // comboBoxFileFormat
        // 
        comboBoxFileFormat.Anchor = AnchorStyles.Left;
        comboBoxFileFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxFileFormat.FormattingEnabled = true;
        comboBoxFileFormat.Location = new Point(65, 3);
        comboBoxFileFormat.Name = "comboBoxFileFormat";
        comboBoxFileFormat.Size = new Size(50, 23);
        comboBoxFileFormat.TabIndex = 20;
        comboBoxFileFormat.SelectedIndexChanged += ComboBoxFileFormat_SelectedIndexChanged;
        // 
        // comboBoxEncoder
        // 
        comboBoxEncoder.Anchor = AnchorStyles.Left;
        comboBoxEncoder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxEncoder.FormattingEnabled = true;
        comboBoxEncoder.Location = new Point(120, 3);
        comboBoxEncoder.Name = "comboBoxEncoder";
        comboBoxEncoder.Size = new Size(90, 23);
        comboBoxEncoder.TabIndex = 25;
        // 
        // labelQuality
        // 
        labelQuality.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelQuality.AutoSize = true;
        labelQuality.Location = new Point(215, 7);
        labelQuality.Name = "labelQuality";
        labelQuality.Size = new Size(31, 15);
        labelQuality.TabIndex = 26;
        labelQuality.Text = "品質";
        // 
        // numericUpDownQuality
        // 
        numericUpDownQuality.Anchor = AnchorStyles.Left;
        numericUpDownQuality.Location = new Point(250, 3);
        numericUpDownQuality.Name = "numericUpDownQuality";
        numericUpDownQuality.Size = new Size(40, 23);
        numericUpDownQuality.TabIndex = 30;
        numericUpDownQuality.TextAlign = HorizontalAlignment.Center;
        // 
        // labelEncoderOption
        // 
        labelEncoderOption.Anchor = AnchorStyles.Left;
        labelEncoderOption.AutoSize = true;
        labelEncoderOption.Location = new Point(295, 7);
        labelEncoderOption.Name = "labelEncoderOption";
        labelEncoderOption.Size = new Size(50, 15);
        labelEncoderOption.TabIndex = 31;
        labelEncoderOption.Text = "オプション";
        // 
        // textBoxEncoderOption
        // 
        textBoxEncoderOption.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxEncoderOption.Location = new Point(350, 3);
        textBoxEncoderOption.Name = "textBoxEncoderOption";
        textBoxEncoderOption.Size = new Size(244, 23);
        textBoxEncoderOption.TabIndex = 40;
        // 
        // groupBoxAlphaFileFormat
        // 
        groupBoxAlphaFileFormat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        groupBoxAlphaFileFormat.AutoSize = true;
        groupBoxAlphaFileFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        groupBoxAlphaFileFormat.Controls.Add(panelAlphaFilePattern);
        groupBoxAlphaFileFormat.Controls.Add(panelAlphaFormat);
        groupBoxAlphaFileFormat.Location = new Point(5, 165);
        groupBoxAlphaFileFormat.Margin = new Padding(5);
        groupBoxAlphaFileFormat.Name = "groupBoxAlphaFileFormat";
        groupBoxAlphaFileFormat.Padding = new Padding(3, 3, 3, 30);
        groupBoxAlphaFileFormat.Size = new Size(614, 85);
        groupBoxAlphaFileFormat.TabIndex = 2;
        groupBoxAlphaFileFormat.TabStop = false;
        groupBoxAlphaFileFormat.Text = "保存形式 (透過)";
        // 
        // panelAlphaFilePattern
        // 
        panelAlphaFilePattern.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panelAlphaFilePattern.AutoSize = true;
        panelAlphaFilePattern.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelAlphaFilePattern.BackColor = SystemColors.Control;
        panelAlphaFilePattern.Controls.Add(labelAlphaFilePattern);
        panelAlphaFilePattern.Controls.Add(textBoxAlphaFilePattern);
        panelAlphaFilePattern.Controls.Add(buttonResetAlphaFilePattern);
        panelAlphaFilePattern.Location = new Point(5, 20);
        panelAlphaFilePattern.Margin = new Padding(0);
        panelAlphaFilePattern.MinimumSize = new Size(0, 30);
        panelAlphaFilePattern.Name = "panelAlphaFilePattern";
        panelAlphaFilePattern.Size = new Size(600, 30);
        panelAlphaFilePattern.TabIndex = 1;
        // 
        // labelAlphaFilePattern
        // 
        labelAlphaFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelAlphaFilePattern.AutoSize = true;
        labelAlphaFilePattern.Location = new Point(6, 7);
        labelAlphaFilePattern.Name = "labelAlphaFilePattern";
        labelAlphaFilePattern.Size = new Size(53, 15);
        labelAlphaFilePattern.TabIndex = 0;
        labelAlphaFilePattern.Text = "ファイル名";
        // 
        // textBoxAlphaFilePattern
        // 
        textBoxAlphaFilePattern.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxAlphaFilePattern.Location = new Point(65, 3);
        textBoxAlphaFilePattern.Name = "textBoxAlphaFilePattern";
        textBoxAlphaFilePattern.Size = new Size(450, 23);
        textBoxAlphaFilePattern.TabIndex = 10;
        // 
        // buttonResetAlphaFilePattern
        // 
        buttonResetAlphaFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonResetAlphaFilePattern.Location = new Point(520, 3);
        buttonResetAlphaFilePattern.Name = "buttonResetAlphaFilePattern";
        buttonResetAlphaFilePattern.Size = new Size(75, 23);
        buttonResetAlphaFilePattern.TabIndex = 15;
        buttonResetAlphaFilePattern.Text = "リセット";
        buttonResetAlphaFilePattern.UseVisualStyleBackColor = true;
        buttonResetAlphaFilePattern.Click += ButtonResetFilePattern_Click;
        // 
        // panelAlphaFormat
        // 
        panelAlphaFormat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panelAlphaFormat.AutoSize = true;
        panelAlphaFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelAlphaFormat.BackColor = SystemColors.Control;
        panelAlphaFormat.Controls.Add(labelAlphaFormat);
        panelAlphaFormat.Controls.Add(comboBoxAlphaFileFormat);
        panelAlphaFormat.Controls.Add(comboBoxAlphaEncoder);
        panelAlphaFormat.Controls.Add(labelAlphaQuality);
        panelAlphaFormat.Controls.Add(numericUpDownAlphaQuality);
        panelAlphaFormat.Controls.Add(labelAlphaEncoderOption);
        panelAlphaFormat.Controls.Add(textBoxAlphaEncoderOption);
        panelAlphaFormat.Location = new Point(5, 50);
        panelAlphaFormat.Margin = new Padding(0);
        panelAlphaFormat.MinimumSize = new Size(0, 30);
        panelAlphaFormat.Name = "panelAlphaFormat";
        panelAlphaFormat.Size = new Size(600, 30);
        panelAlphaFormat.TabIndex = 4;
        // 
        // labelAlphaFormat
        // 
        labelAlphaFormat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelAlphaFormat.AutoSize = true;
        labelAlphaFormat.Location = new Point(6, 7);
        labelAlphaFormat.Name = "labelAlphaFormat";
        labelAlphaFormat.Size = new Size(31, 15);
        labelAlphaFormat.TabIndex = 0;
        labelAlphaFormat.Text = "形式";
        // 
        // comboBoxAlphaFileFormat
        // 
        comboBoxAlphaFileFormat.Anchor = AnchorStyles.Left;
        comboBoxAlphaFileFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxAlphaFileFormat.FormattingEnabled = true;
        comboBoxAlphaFileFormat.Location = new Point(65, 3);
        comboBoxAlphaFileFormat.Name = "comboBoxAlphaFileFormat";
        comboBoxAlphaFileFormat.Size = new Size(50, 23);
        comboBoxAlphaFileFormat.TabIndex = 20;
        comboBoxAlphaFileFormat.SelectedIndexChanged += ComboBoxFileFormat_SelectedIndexChanged;
        // 
        // comboBoxAlphaEncoder
        // 
        comboBoxAlphaEncoder.Anchor = AnchorStyles.Left;
        comboBoxAlphaEncoder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxAlphaEncoder.FormattingEnabled = true;
        comboBoxAlphaEncoder.Location = new Point(120, 3);
        comboBoxAlphaEncoder.Name = "comboBoxAlphaEncoder";
        comboBoxAlphaEncoder.Size = new Size(90, 23);
        comboBoxAlphaEncoder.TabIndex = 25;
        // 
        // labelAlphaQuality
        // 
        labelAlphaQuality.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        labelAlphaQuality.AutoSize = true;
        labelAlphaQuality.Location = new Point(215, 7);
        labelAlphaQuality.Name = "labelAlphaQuality";
        labelAlphaQuality.Size = new Size(31, 15);
        labelAlphaQuality.TabIndex = 26;
        labelAlphaQuality.Text = "品質";
        // 
        // numericUpDownAlphaQuality
        // 
        numericUpDownAlphaQuality.Anchor = AnchorStyles.Left;
        numericUpDownAlphaQuality.Location = new Point(250, 3);
        numericUpDownAlphaQuality.Name = "numericUpDownAlphaQuality";
        numericUpDownAlphaQuality.Size = new Size(40, 23);
        numericUpDownAlphaQuality.TabIndex = 30;
        numericUpDownAlphaQuality.TextAlign = HorizontalAlignment.Center;
        // 
        // labelAlphaEncoderOption
        // 
        labelAlphaEncoderOption.Anchor = AnchorStyles.Left;
        labelAlphaEncoderOption.AutoSize = true;
        labelAlphaEncoderOption.Location = new Point(295, 7);
        labelAlphaEncoderOption.Name = "labelAlphaEncoderOption";
        labelAlphaEncoderOption.Size = new Size(50, 15);
        labelAlphaEncoderOption.TabIndex = 31;
        labelAlphaEncoderOption.Text = "オプション";
        // 
        // textBoxAlphaEncoderOption
        // 
        textBoxAlphaEncoderOption.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxAlphaEncoderOption.Location = new Point(350, 3);
        textBoxAlphaEncoderOption.Name = "textBoxAlphaEncoderOption";
        textBoxAlphaEncoderOption.Size = new Size(244, 23);
        textBoxAlphaEncoderOption.TabIndex = 40;
        // 
        // mainTableLayoutPanel
        // 
        mainTableLayoutPanel.BackColor = SystemColors.Control;
        mainTableLayoutPanel.ColumnCount = 1;
        mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        mainTableLayoutPanel.Controls.Add(groupBoxSaveDir);
        mainTableLayoutPanel.Controls.Add(groupBoxFileFormat);
        mainTableLayoutPanel.Controls.Add(groupBoxAlphaFileFormat);
        mainTableLayoutPanel.Dock = DockStyle.Fill;
        mainTableLayoutPanel.Location = new Point(0, 0);
        mainTableLayoutPanel.Margin = new Padding(0);
        mainTableLayoutPanel.Name = "mainTableLayoutPanel";
        mainTableLayoutPanel.RowCount = 4;
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
        mainTableLayoutPanel.Size = new Size(624, 321);
        mainTableLayoutPanel.TabIndex = 0;
        // 
        // buttonQualityTest
        // 
        buttonQualityTest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonQualityTest.Location = new Point(438, 3);
        buttonQualityTest.Name = "buttonQualityTest";
        buttonQualityTest.Size = new Size(75, 23);
        buttonQualityTest.TabIndex = 45;
        buttonQualityTest.Text = "テスト";
        buttonQualityTest.UseVisualStyleBackColor = true;
        // 
        // ConfigWindow
        // 
        AcceptButton = buttonSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(624, 321);
        Controls.Add(bottomPanel);
        Controls.Add(mainTableLayoutPanel);
        MaximumSize = new Size(640, 360);
        MinimumSize = new Size(640, 360);
        Name = "ConfigWindow";
        Text = "VRCImageHelper/設定";
        Load += ConfigWindow_Load;
        bottomPanel.ResumeLayout(false);
        bottomPanel.PerformLayout();
        groupBoxSaveDir.ResumeLayout(false);
        groupBoxSaveDir.PerformLayout();
        panelSaveDir.ResumeLayout(false);
        panelSaveDir.PerformLayout();
        groupBoxFileFormat.ResumeLayout(false);
        groupBoxFileFormat.PerformLayout();
        panelFilePattern.ResumeLayout(false);
        panelFilePattern.PerformLayout();
        panelFileFormat.ResumeLayout(false);
        panelFileFormat.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownQuality).EndInit();
        groupBoxAlphaFileFormat.ResumeLayout(false);
        groupBoxAlphaFileFormat.PerformLayout();
        panelAlphaFilePattern.ResumeLayout(false);
        panelAlphaFilePattern.PerformLayout();
        panelAlphaFormat.ResumeLayout(false);
        panelAlphaFormat.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownAlphaQuality).EndInit();
        mainTableLayoutPanel.ResumeLayout(false);
        mainTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Button buttonSave;
    private Button buttonCancel;
    private FlowLayoutPanel bottomPanel;
    private GroupBox groupBoxSaveDir;
    private GroupBox groupBoxFileFormat;
    private GroupBox groupBoxAlphaFileFormat;
    private TableLayoutPanel mainTableLayoutPanel;
    private Panel panelSaveDir;
    private Panel panelFilePattern;
    private Panel panelFileFormat;
    private Panel panelAlphaFormat;
    private Panel panelAlphaFilePattern;
    private Label labelSaveDir;
    private Label labelFilePattern;
    private Label labelFileFormat;
    private Label labelAlphaFormat;
    private Label labelAlphaFilePattern;
    private Label labelQuality;
    private Label labelAlphaQuality;
    private Label labelEncoderOption;
    private Label labelAlphaEncoderOption;
    private Button buttonSelectDir;
    private Button buttonResetFilePattern;
    private Button buttonResetAlphaFilePattern;
    private Button buttonQualityTest;
    private TextBox textBoxDir;
    private TextBox textBoxFilePattern;
    private TextBox textBoxAlphaFilePattern;
    private TextBox textBoxEncoderOption;
    private TextBox textBoxAlphaEncoderOption;
    private ComboBox comboBoxFileFormat;
    private ComboBox comboBoxEncoder;
    private ComboBox comboBoxAlphaFileFormat;
    private ComboBox comboBoxAlphaEncoder;
    private NumericUpDown numericUpDownQuality;
    private NumericUpDown numericUpDownAlphaQuality;
}
