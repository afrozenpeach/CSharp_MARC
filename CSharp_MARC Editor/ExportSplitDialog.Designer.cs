namespace CSharp_MARC_Editor
{
    partial class ExportSplitDialog
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
            this.recordsPerFileNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.recordsPerFileLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.recordsPerFileNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // recordsPerFileNumericUpDown
            // 
            this.recordsPerFileNumericUpDown.Location = new System.Drawing.Point(102, 12);
            this.recordsPerFileNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.recordsPerFileNumericUpDown.Name = "recordsPerFileNumericUpDown";
            this.recordsPerFileNumericUpDown.Size = new System.Drawing.Size(95, 20);
            this.recordsPerFileNumericUpDown.TabIndex = 0;
            this.recordsPerFileNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // recordsPerFileLabel
            // 
            this.recordsPerFileLabel.AutoSize = true;
            this.recordsPerFileLabel.Location = new System.Drawing.Point(12, 14);
            this.recordsPerFileLabel.Name = "recordsPerFileLabel";
            this.recordsPerFileLabel.Size = new System.Drawing.Size(84, 13);
            this.recordsPerFileLabel.TabIndex = 1;
            this.recordsPerFileLabel.Text = "Records per file:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(107, 38);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(26, 38);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ExportSplitDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(209, 76);
            this.ControlBox = false;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.recordsPerFileLabel);
            this.Controls.Add(this.recordsPerFileNumericUpDown);
            this.MaximumSize = new System.Drawing.Size(225, 115);
            this.MinimumSize = new System.Drawing.Size(225, 115);
            this.Name = "ExportSplitDialog";
            this.Text = "Export -> Split";
            ((System.ComponentModel.ISupportInitialize)(this.recordsPerFileNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown recordsPerFileNumericUpDown;
        private System.Windows.Forms.Label recordsPerFileLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}