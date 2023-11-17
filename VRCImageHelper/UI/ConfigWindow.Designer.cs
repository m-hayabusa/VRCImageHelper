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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWindow));
        buttonSave = new Button();
        buttonCancel = new Button();
        bottomPanel = new FlowLayoutPanel();
        groupBoxSaveDir = new GroupBox();
        panelOptions = new Panel();
        checkBoxOverwriteDest = new CheckBox();
        checkBoxDeleteOriginal = new CheckBox();
        labelOptions = new Label();
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
        panelOptions.SuspendLayout();
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
        resources.ApplyResources(buttonSave, "buttonSave");
        buttonSave.Name = "buttonSave";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += ButtonSave_Click;
        // 
        // buttonCancel
        // 
        resources.ApplyResources(buttonCancel, "buttonCancel");
        buttonCancel.Name = "buttonCancel";
        buttonCancel.UseVisualStyleBackColor = true;
        buttonCancel.Click += ButtonCancel_Click;
        // 
        // bottomPanel
        // 
        resources.ApplyResources(bottomPanel, "bottomPanel");
        bottomPanel.Controls.Add(buttonCancel);
        bottomPanel.Controls.Add(buttonSave);
        bottomPanel.Name = "bottomPanel";
        // 
        // groupBoxSaveDir
        // 
        resources.ApplyResources(groupBoxSaveDir, "groupBoxSaveDir");
        groupBoxSaveDir.Controls.Add(panelOptions);
        groupBoxSaveDir.Controls.Add(panelSaveDir);
        groupBoxSaveDir.Name = "groupBoxSaveDir";
        groupBoxSaveDir.TabStop = false;
        // 
        // panelOptions
        // 
        resources.ApplyResources(panelOptions, "panelOptions");
        panelOptions.BackColor = SystemColors.Control;
        panelOptions.Controls.Add(checkBoxOverwriteDest);
        panelOptions.Controls.Add(checkBoxDeleteOriginal);
        panelOptions.Controls.Add(labelOptions);
        panelOptions.Name = "panelOptions";
        // 
        // checkBoxOverwriteDest
        // 
        resources.ApplyResources(checkBoxOverwriteDest, "checkBoxOverwriteDest");
        checkBoxOverwriteDest.Name = "checkBoxOverwriteDest";
        checkBoxOverwriteDest.UseVisualStyleBackColor = true;
        // 
        // checkBoxDeleteOriginal
        // 
        resources.ApplyResources(checkBoxDeleteOriginal, "checkBoxDeleteOriginal");
        checkBoxDeleteOriginal.Name = "checkBoxDeleteOriginal";
        checkBoxDeleteOriginal.UseVisualStyleBackColor = true;
        // 
        // labelOptions
        // 
        resources.ApplyResources(labelOptions, "labelOptions");
        labelOptions.Name = "labelOptions";
        // 
        // panelSaveDir
        // 
        resources.ApplyResources(panelSaveDir, "panelSaveDir");
        panelSaveDir.BackColor = SystemColors.Control;
        panelSaveDir.Controls.Add(labelSaveDir);
        panelSaveDir.Controls.Add(buttonSelectDir);
        panelSaveDir.Controls.Add(textBoxDir);
        panelSaveDir.Name = "panelSaveDir";
        // 
        // labelSaveDir
        // 
        resources.ApplyResources(labelSaveDir, "labelSaveDir");
        labelSaveDir.Name = "labelSaveDir";
        // 
        // buttonSelectDir
        // 
        resources.ApplyResources(buttonSelectDir, "buttonSelectDir");
        buttonSelectDir.Name = "buttonSelectDir";
        buttonSelectDir.UseVisualStyleBackColor = true;
        buttonSelectDir.Click += ButtonSelectDir_Click;
        // 
        // textBoxDir
        // 
        resources.ApplyResources(textBoxDir, "textBoxDir");
        textBoxDir.Name = "textBoxDir";
        textBoxDir.ReadOnly = true;
        textBoxDir.TabStop = false;
        // 
        // groupBoxFileFormat
        // 
        resources.ApplyResources(groupBoxFileFormat, "groupBoxFileFormat");
        groupBoxFileFormat.Controls.Add(panelFilePattern);
        groupBoxFileFormat.Controls.Add(panelFileFormat);
        groupBoxFileFormat.Name = "groupBoxFileFormat";
        groupBoxFileFormat.TabStop = false;
        // 
        // panelFilePattern
        // 
        resources.ApplyResources(panelFilePattern, "panelFilePattern");
        panelFilePattern.BackColor = SystemColors.Control;
        panelFilePattern.Controls.Add(labelFilePattern);
        panelFilePattern.Controls.Add(textBoxFilePattern);
        panelFilePattern.Controls.Add(buttonResetFilePattern);
        panelFilePattern.Name = "panelFilePattern";
        // 
        // labelFilePattern
        // 
        resources.ApplyResources(labelFilePattern, "labelFilePattern");
        labelFilePattern.Name = "labelFilePattern";
        // 
        // textBoxFilePattern
        // 
        resources.ApplyResources(textBoxFilePattern, "textBoxFilePattern");
        textBoxFilePattern.Name = "textBoxFilePattern";
        // 
        // buttonResetFilePattern
        // 
        resources.ApplyResources(buttonResetFilePattern, "buttonResetFilePattern");
        buttonResetFilePattern.Name = "buttonResetFilePattern";
        buttonResetFilePattern.UseVisualStyleBackColor = true;
        buttonResetFilePattern.Click += ButtonResetFilePattern_Click;
        // 
        // panelFileFormat
        // 
        resources.ApplyResources(panelFileFormat, "panelFileFormat");
        panelFileFormat.BackColor = SystemColors.Control;
        panelFileFormat.Controls.Add(labelFileFormat);
        panelFileFormat.Controls.Add(comboBoxFileFormat);
        panelFileFormat.Controls.Add(comboBoxEncoder);
        panelFileFormat.Controls.Add(labelQuality);
        panelFileFormat.Controls.Add(numericUpDownQuality);
        panelFileFormat.Controls.Add(labelEncoderOption);
        panelFileFormat.Controls.Add(textBoxEncoderOption);
        panelFileFormat.Name = "panelFileFormat";
        // 
        // labelFileFormat
        // 
        resources.ApplyResources(labelFileFormat, "labelFileFormat");
        labelFileFormat.Name = "labelFileFormat";
        // 
        // comboBoxFileFormat
        // 
        resources.ApplyResources(comboBoxFileFormat, "comboBoxFileFormat");
        comboBoxFileFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxFileFormat.FormattingEnabled = true;
        comboBoxFileFormat.Name = "comboBoxFileFormat";
        comboBoxFileFormat.SelectedIndexChanged += ComboBoxFileFormat_SelectedIndexChanged;
        // 
        // comboBoxEncoder
        // 
        resources.ApplyResources(comboBoxEncoder, "comboBoxEncoder");
        comboBoxEncoder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxEncoder.FormattingEnabled = true;
        comboBoxEncoder.Name = "comboBoxEncoder";
        // 
        // labelQuality
        // 
        resources.ApplyResources(labelQuality, "labelQuality");
        labelQuality.Name = "labelQuality";
        // 
        // numericUpDownQuality
        // 
        resources.ApplyResources(numericUpDownQuality, "numericUpDownQuality");
        numericUpDownQuality.Name = "numericUpDownQuality";
        // 
        // labelEncoderOption
        // 
        resources.ApplyResources(labelEncoderOption, "labelEncoderOption");
        labelEncoderOption.Name = "labelEncoderOption";
        // 
        // textBoxEncoderOption
        // 
        resources.ApplyResources(textBoxEncoderOption, "textBoxEncoderOption");
        textBoxEncoderOption.Name = "textBoxEncoderOption";
        // 
        // groupBoxAlphaFileFormat
        // 
        resources.ApplyResources(groupBoxAlphaFileFormat, "groupBoxAlphaFileFormat");
        groupBoxAlphaFileFormat.Controls.Add(panelAlphaFilePattern);
        groupBoxAlphaFileFormat.Controls.Add(panelAlphaFormat);
        groupBoxAlphaFileFormat.Name = "groupBoxAlphaFileFormat";
        groupBoxAlphaFileFormat.TabStop = false;
        // 
        // panelAlphaFilePattern
        // 
        resources.ApplyResources(panelAlphaFilePattern, "panelAlphaFilePattern");
        panelAlphaFilePattern.BackColor = SystemColors.Control;
        panelAlphaFilePattern.Controls.Add(labelAlphaFilePattern);
        panelAlphaFilePattern.Controls.Add(textBoxAlphaFilePattern);
        panelAlphaFilePattern.Controls.Add(buttonResetAlphaFilePattern);
        panelAlphaFilePattern.Name = "panelAlphaFilePattern";
        // 
        // labelAlphaFilePattern
        // 
        resources.ApplyResources(labelAlphaFilePattern, "labelAlphaFilePattern");
        labelAlphaFilePattern.Name = "labelAlphaFilePattern";
        // 
        // textBoxAlphaFilePattern
        // 
        resources.ApplyResources(textBoxAlphaFilePattern, "textBoxAlphaFilePattern");
        textBoxAlphaFilePattern.Name = "textBoxAlphaFilePattern";
        // 
        // buttonResetAlphaFilePattern
        // 
        resources.ApplyResources(buttonResetAlphaFilePattern, "buttonResetAlphaFilePattern");
        buttonResetAlphaFilePattern.Name = "buttonResetAlphaFilePattern";
        buttonResetAlphaFilePattern.UseVisualStyleBackColor = true;
        buttonResetAlphaFilePattern.Click += ButtonResetFilePattern_Click;
        // 
        // panelAlphaFormat
        // 
        resources.ApplyResources(panelAlphaFormat, "panelAlphaFormat");
        panelAlphaFormat.BackColor = SystemColors.Control;
        panelAlphaFormat.Controls.Add(labelAlphaFormat);
        panelAlphaFormat.Controls.Add(comboBoxAlphaFileFormat);
        panelAlphaFormat.Controls.Add(comboBoxAlphaEncoder);
        panelAlphaFormat.Controls.Add(labelAlphaQuality);
        panelAlphaFormat.Controls.Add(numericUpDownAlphaQuality);
        panelAlphaFormat.Controls.Add(labelAlphaEncoderOption);
        panelAlphaFormat.Controls.Add(textBoxAlphaEncoderOption);
        panelAlphaFormat.Name = "panelAlphaFormat";
        // 
        // labelAlphaFormat
        // 
        resources.ApplyResources(labelAlphaFormat, "labelAlphaFormat");
        labelAlphaFormat.Name = "labelAlphaFormat";
        // 
        // comboBoxAlphaFileFormat
        // 
        resources.ApplyResources(comboBoxAlphaFileFormat, "comboBoxAlphaFileFormat");
        comboBoxAlphaFileFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxAlphaFileFormat.FormattingEnabled = true;
        comboBoxAlphaFileFormat.Name = "comboBoxAlphaFileFormat";
        comboBoxAlphaFileFormat.SelectedIndexChanged += ComboBoxFileFormat_SelectedIndexChanged;
        // 
        // comboBoxAlphaEncoder
        // 
        resources.ApplyResources(comboBoxAlphaEncoder, "comboBoxAlphaEncoder");
        comboBoxAlphaEncoder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxAlphaEncoder.FormattingEnabled = true;
        comboBoxAlphaEncoder.Name = "comboBoxAlphaEncoder";
        // 
        // labelAlphaQuality
        // 
        resources.ApplyResources(labelAlphaQuality, "labelAlphaQuality");
        labelAlphaQuality.Name = "labelAlphaQuality";
        // 
        // numericUpDownAlphaQuality
        // 
        resources.ApplyResources(numericUpDownAlphaQuality, "numericUpDownAlphaQuality");
        numericUpDownAlphaQuality.Name = "numericUpDownAlphaQuality";
        // 
        // labelAlphaEncoderOption
        // 
        resources.ApplyResources(labelAlphaEncoderOption, "labelAlphaEncoderOption");
        labelAlphaEncoderOption.Name = "labelAlphaEncoderOption";
        // 
        // textBoxAlphaEncoderOption
        // 
        resources.ApplyResources(textBoxAlphaEncoderOption, "textBoxAlphaEncoderOption");
        textBoxAlphaEncoderOption.Name = "textBoxAlphaEncoderOption";
        // 
        // mainTableLayoutPanel
        // 
        resources.ApplyResources(mainTableLayoutPanel, "mainTableLayoutPanel");
        mainTableLayoutPanel.BackColor = SystemColors.Control;
        mainTableLayoutPanel.Controls.Add(groupBoxSaveDir);
        mainTableLayoutPanel.Controls.Add(groupBoxFileFormat);
        mainTableLayoutPanel.Controls.Add(groupBoxAlphaFileFormat);
        mainTableLayoutPanel.Name = "mainTableLayoutPanel";
        // 
        // buttonQualityTest
        // 
        resources.ApplyResources(buttonQualityTest, "buttonQualityTest");
        buttonQualityTest.Name = "buttonQualityTest";
        buttonQualityTest.UseVisualStyleBackColor = true;
        // 
        // ConfigWindow
        // 
        AcceptButton = buttonSave;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        Controls.Add(bottomPanel);
        Controls.Add(mainTableLayoutPanel);
        Name = "ConfigWindow";
        Load += ConfigWindow_Load;
        bottomPanel.ResumeLayout(false);
        bottomPanel.PerformLayout();
        groupBoxSaveDir.ResumeLayout(false);
        groupBoxSaveDir.PerformLayout();
        panelOptions.ResumeLayout(false);
        panelOptions.PerformLayout();
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
    private Panel panelOptions;
    private Label labelOptions;
    private CheckBox checkBoxOverwriteDest;
    private CheckBox checkBoxDeleteOriginal;
}
