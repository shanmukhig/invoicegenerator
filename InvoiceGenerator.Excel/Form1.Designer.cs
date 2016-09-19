namespace InvoiceGenerator.Excel
{
  partial class Form1
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
      this.btnSelectFile = new System.Windows.Forms.Button();
      this.lblStatus = new System.Windows.Forms.Label();
      this.btnGenerate = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnSelectFile
      // 
      this.btnSelectFile.Location = new System.Drawing.Point(486, 223);
      this.btnSelectFile.Name = "btnSelectFile";
      this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
      this.btnSelectFile.TabIndex = 8;
      this.btnSelectFile.Text = "Select File...";
      this.btnSelectFile.UseVisualStyleBackColor = true;
      this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(9, 160);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(169, 13);
      this.lblStatus.TabIndex = 7;
      this.lblStatus.Text = "Generating invoice for customer ...";
      // 
      // btnGenerate
      // 
      this.btnGenerate.Location = new System.Drawing.Point(567, 223);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(125, 23);
      this.btnGenerate.TabIndex = 6;
      this.btnGenerate.Text = "Generate Invoices";
      this.btnGenerate.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(704, 258);
      this.Controls.Add(this.btnSelectFile);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.btnGenerate);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnGenerate;
  }
}

