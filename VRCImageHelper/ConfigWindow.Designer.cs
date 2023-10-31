namespace VRCImageHelper;

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
        mainGroupBox = new GroupBox();
        mainTableLayoutPanel = new TableLayoutPanel();
        panel1 = new Panel();
        label1 = new Label();
        buttonSelectDir = new Button();
        textBoxDir = new TextBox();
        panel2 = new Panel();
        label2 = new Label();
        textBoxFilePattern = new TextBox();
        buttonResetFilePattern = new Button();
        panel3 = new Panel();
        label3 = new Label();
        comboBoxFileFormat = new ComboBox();
        comboBoxEncoder = new ComboBox();
        labelQuality = new Label();
        numericUpDownQuality = new NumericUpDown();
        labelEncoderOption = new Label();
        textBoxEncoderOption = new TextBox();
        buttonQualityTest = new Button();
        bottomPanel.SuspendLayout();
        mainGroupBox.SuspendLayout();
        mainTableLayoutPanel.SuspendLayout();
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        panel3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownQuality).BeginInit();
        SuspendLayout();
        // 
        // buttonSave
        // 
        buttonSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonSave.AutoSize = true;
        buttonSave.Location = new Point(376, 3);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(77, 25);
        buttonSave.TabIndex = 100;
        buttonSave.Text = "保存";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += ButtonSave_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonCancel.AutoSize = true;
        buttonCancel.Location = new Point(459, 3);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(88, 25);
        buttonCancel.TabIndex = 110;
        buttonCancel.Text = "キャンセル";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // bottomPanel
        // 
        bottomPanel.AutoSize = true;
        bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        bottomPanel.Controls.Add(buttonCancel);
        bottomPanel.Controls.Add(buttonSave);
        bottomPanel.Dock = DockStyle.Bottom;
        bottomPanel.FlowDirection = FlowDirection.RightToLeft;
        bottomPanel.Location = new Point(0, 120);
        bottomPanel.Name = "bottomPanel";
        bottomPanel.Size = new Size(550, 31);
        bottomPanel.TabIndex = 4;
        // 
        // mainGroupBox
        // 
        mainGroupBox.Controls.Add(mainTableLayoutPanel);
        mainGroupBox.Dock = DockStyle.Fill;
        mainGroupBox.Location = new Point(0, 0);
        mainGroupBox.Margin = new Padding(3, 3, 0, 3);
        mainGroupBox.Name = "mainGroupBox";
        mainGroupBox.Padding = new Padding(3, 3, 3, 30);
        mainGroupBox.Size = new Size(550, 151);
        mainGroupBox.TabIndex = 2;
        mainGroupBox.TabStop = false;
        mainGroupBox.Text = "保存先 / 保存形式";
        // 
        // mainTableLayoutPanel
        // 
        mainTableLayoutPanel.BackColor = SystemColors.Control;
        mainTableLayoutPanel.ColumnCount = 1;
        mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        mainTableLayoutPanel.Controls.Add(panel1);
        mainTableLayoutPanel.Controls.Add(panel2);
        mainTableLayoutPanel.Controls.Add(panel3);
        mainTableLayoutPanel.Dock = DockStyle.Fill;
        mainTableLayoutPanel.Location = new Point(3, 19);
        mainTableLayoutPanel.Margin = new Padding(0);
        mainTableLayoutPanel.Name = "mainTableLayoutPanel";
        mainTableLayoutPanel.RowCount = 4;
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        mainTableLayoutPanel.Size = new Size(544, 102);
        mainTableLayoutPanel.TabIndex = 0;
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panel1.AutoSize = true;
        panel1.BackColor = SystemColors.Control;
        panel1.Controls.Add(label1);
        panel1.Controls.Add(buttonSelectDir);
        panel1.Controls.Add(textBoxDir);
        panel1.Location = new Point(0, 0);
        panel1.Margin = new Padding(0);
        panel1.MinimumSize = new Size(0, 30);
        panel1.Name = "panel1";
        panel1.Size = new Size(544, 30);
        panel1.TabStop = false;
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        label1.AutoSize = true;
        label1.Location = new Point(6, 7);
        label1.Name = "label1";
        label1.Size = new Size(43, 15);
        label1.Text = "保存先";
        // 
        // buttonSelectDir
        // 
        buttonSelectDir.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonSelectDir.Location = new Point(468, 3);
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
        textBoxDir.Size = new Size(398, 23);
        textBoxDir.TabStop = false; 
        // 
        // panel2
        // 
        panel2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panel2.AutoSize = true;
        panel2.BackColor = SystemColors.Control;
        panel2.Controls.Add(label2);
        panel2.Controls.Add(textBoxFilePattern);
        panel2.Controls.Add(buttonResetFilePattern);
        panel2.Location = new Point(0, 30);
        panel2.Margin = new Padding(0);
        panel2.MinimumSize = new Size(0, 30);
        panel2.Name = "panel2";
        panel2.Size = new Size(544, 30);
        // 
        // label2
        // 
        label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        label2.AutoSize = true;
        label2.Location = new Point(6, 7);
        label2.Name = "label2";
        label2.Size = new Size(53, 15);
        label2.Text = "ファイル名";
        // 
        // textBoxFilePattern
        // 
        textBoxFilePattern.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxFilePattern.Location = new Point(65, 3);
        textBoxFilePattern.Name = "textBoxFilePattern";
        textBoxFilePattern.Size = new Size(398, 23);
        textBoxFilePattern.TabIndex = 10;
        // 
        // buttonResetFilePattern
        // 
        buttonResetFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        buttonResetFilePattern.Location = new Point(468, 3);
        buttonResetFilePattern.Name = "buttonResetFilePattern";
        buttonResetFilePattern.Size = new Size(75, 23);
        buttonResetFilePattern.TabIndex = 15;
        buttonResetFilePattern.Text = "リセット";
        buttonResetFilePattern.UseVisualStyleBackColor = true;
        buttonResetFilePattern.Click += ButtonResetFilePattern_Click;
        // 
        // panel3
        // 
        panel3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        panel3.AutoSize = true;
        panel3.BackColor = SystemColors.Control;
        panel3.Controls.Add(label3);
        panel3.Controls.Add(comboBoxFileFormat);
        panel3.Controls.Add(comboBoxEncoder);
        panel3.Controls.Add(labelQuality);
        panel3.Controls.Add(numericUpDownQuality);
        panel3.Controls.Add(labelEncoderOption);
        panel3.Controls.Add(textBoxEncoderOption);
        panel3.Location = new Point(0, 60);
        panel3.Margin = new Padding(0);
        panel3.MinimumSize = new Size(0, 30);
        panel3.Name = "panel3";
        panel3.Size = new Size(544, 30);
        // 
        // label3
        // 
        label3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        label3.AutoSize = true;
        label3.Location = new Point(6, 7);
        label3.Name = "label3";
        label3.Size = new Size(31, 15);
        label3.Text = "形式";
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
        labelEncoderOption.Text = "オプション";
        // 
        // textBoxEncoderOption
        // 
        textBoxEncoderOption.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textBoxEncoderOption.Location = new Point(350, 3);
        textBoxEncoderOption.Name = "textBoxEncoderOption";
        textBoxEncoderOption.Size = new Size(192, 23);
        textBoxEncoderOption.TabIndex = 40;
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
        ClientSize = new Size(550, 151);
        Controls.Add(bottomPanel);
        Controls.Add(mainGroupBox);
        MaximumSize = new Size(1550, 190);
        MinimumSize = new Size(550, 190);
        Name = "ConfigWindow";
        Text = "VRCImageHelper/設定";
        Load += ConfigWindow_Load;
        bottomPanel.ResumeLayout(false);
        bottomPanel.PerformLayout();
        mainGroupBox.ResumeLayout(false);
        mainTableLayoutPanel.ResumeLayout(false);
        mainTableLayoutPanel.PerformLayout();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        panel3.ResumeLayout(false);
        panel3.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownQuality).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button buttonSave;
    private Button buttonCancel;
    private FlowLayoutPanel bottomPanel;
    private GroupBox mainGroupBox;
    private TableLayoutPanel mainTableLayoutPanel;
    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label labelQuality;
    private Label labelEncoderOption;
    private Button buttonSelectDir;
    private Button buttonResetFilePattern;
    private Button buttonQualityTest;
    private TextBox textBoxDir;
    private TextBox textBoxFilePattern;
    private TextBox textBoxEncoderOption;
    private ComboBox comboBoxFileFormat;
    private ComboBox comboBoxEncoder;
    private NumericUpDown numericUpDownQuality;
}
