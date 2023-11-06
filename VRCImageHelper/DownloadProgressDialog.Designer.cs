namespace VRCImageHelper;

partial class DownloadProgressDialog
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
        if (disposing && (components != null))
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
        progressBar1 = new ProgressBar();
        SuspendLayout();
        // 
        // progressBar1
        // 
        progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressBar1.Location = new Point(12, 12);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new Size(210, 23);
        progressBar1.Style = ProgressBarStyle.Marquee;
        progressBar1.TabIndex = 0;
        // 
        // DownloadProgressBar
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(234, 47);
        Controls.Add(progressBar1);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Name = "DownloadProgressBar";
        Text = "DownloadProgressBar";
        UseWaitCursor = true;
        ResumeLayout(false);
    }

    #endregion

    private ProgressBar progressBar1;
}
