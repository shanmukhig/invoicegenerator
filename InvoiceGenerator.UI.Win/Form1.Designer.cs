namespace InvoiceGenerator.UI.Win
{
  partial class FrmInvoiceGenerator
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
      this.btnGenerate = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.lblProgress = new System.Windows.Forms.Label();
      this.lblStatus = new System.Windows.Forms.Label();
      this.btnSelectFile = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnGenerate
      // 
      this.btnGenerate.Location = new System.Drawing.Point(402, 224);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(125, 23);
      this.btnGenerate.TabIndex = 0;
      this.btnGenerate.Text = "Generate Invoices";
      this.btnGenerate.UseVisualStyleBackColor = true;
      this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(533, 224);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(12, 120);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(596, 23);
      this.progressBar1.TabIndex = 2;
      // 
      // lblProgress
      // 
      this.lblProgress.AutoSize = true;
      this.lblProgress.Location = new System.Drawing.Point(13, 101);
      this.lblProgress.Name = "lblProgress";
      this.lblProgress.Size = new System.Drawing.Size(48, 13);
      this.lblProgress.TabIndex = 3;
      this.lblProgress.Text = "Progress";
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(12, 161);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(169, 13);
      this.lblStatus.TabIndex = 4;
      this.lblStatus.Text = "Generating invoice for customer ...";
      // 
      // btnSelectFile
      // 
      this.btnSelectFile.Location = new System.Drawing.Point(321, 224);
      this.btnSelectFile.Name = "btnSelectFile";
      this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
      this.btnSelectFile.TabIndex = 5;
      this.btnSelectFile.Text = "Select File...";
      this.btnSelectFile.UseVisualStyleBackColor = true;
      this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
      // 
      // frmInvoiceGenerator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(620, 259);
      this.Controls.Add(this.btnSelectFile);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.lblProgress);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnGenerate);
      this.Name = "FrmInvoiceGenerator";
      this.Text = "Invoice Generator";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnGenerate;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Label lblProgress;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnSelectFile;
  }
}

