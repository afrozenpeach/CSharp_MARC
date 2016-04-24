namespace CSharp_MARC_Editor
{
    partial class FindReplaceForm
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
            this.tagsListBox = new System.Windows.Forms.ListBox();
            this.tagsLabel = new System.Windows.Forms.Label();
            this.indicatorsLabel = new System.Windows.Forms.Label();
            this.ind1ListBox = new System.Windows.Forms.ListBox();
            this.ind2ListBox = new System.Windows.Forms.ListBox();
            this.codesLabel = new System.Windows.Forms.Label();
            this.codesListBox = new System.Windows.Forms.ListBox();
            this.dataLabel = new System.Windows.Forms.Label();
            this.dataTextBox = new System.Windows.Forms.TextBox();
            this.replaceWithLabel = new System.Windows.Forms.Label();
            this.replaceWithTextBox = new System.Windows.Forms.TextBox();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.caseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tagsListBox
            // 
            this.tagsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tagsListBox.FormattingEnabled = true;
            this.tagsListBox.Location = new System.Drawing.Point(12, 31);
            this.tagsListBox.Name = "tagsListBox";
            this.tagsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.tagsListBox.Size = new System.Drawing.Size(50, 95);
            this.tagsListBox.TabIndex = 0;
            this.tagsListBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // tagsLabel
            // 
            this.tagsLabel.AutoSize = true;
            this.tagsLabel.Location = new System.Drawing.Point(9, 9);
            this.tagsLabel.Name = "tagsLabel";
            this.tagsLabel.Size = new System.Drawing.Size(34, 13);
            this.tagsLabel.TabIndex = 1;
            this.tagsLabel.Text = "Tags:";
            // 
            // indicatorsLabel
            // 
            this.indicatorsLabel.AutoSize = true;
            this.indicatorsLabel.Location = new System.Drawing.Point(65, 9);
            this.indicatorsLabel.Name = "indicatorsLabel";
            this.indicatorsLabel.Size = new System.Drawing.Size(56, 13);
            this.indicatorsLabel.TabIndex = 2;
            this.indicatorsLabel.Text = "Indicators:";
            // 
            // ind1ListBox
            // 
            this.ind1ListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ind1ListBox.FormattingEnabled = true;
            this.ind1ListBox.Location = new System.Drawing.Point(68, 31);
            this.ind1ListBox.Name = "ind1ListBox";
            this.ind1ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ind1ListBox.Size = new System.Drawing.Size(50, 95);
            this.ind1ListBox.TabIndex = 3;
            this.ind1ListBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // ind2ListBox
            // 
            this.ind2ListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ind2ListBox.FormattingEnabled = true;
            this.ind2ListBox.Location = new System.Drawing.Point(124, 31);
            this.ind2ListBox.Name = "ind2ListBox";
            this.ind2ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ind2ListBox.Size = new System.Drawing.Size(50, 95);
            this.ind2ListBox.TabIndex = 4;
            this.ind2ListBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // codesLabel
            // 
            this.codesLabel.AutoSize = true;
            this.codesLabel.Location = new System.Drawing.Point(177, 9);
            this.codesLabel.Name = "codesLabel";
            this.codesLabel.Size = new System.Drawing.Size(40, 13);
            this.codesLabel.TabIndex = 5;
            this.codesLabel.Text = "Codes:";
            // 
            // codesListBox
            // 
            this.codesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.codesListBox.FormattingEnabled = true;
            this.codesListBox.Location = new System.Drawing.Point(180, 31);
            this.codesListBox.Name = "codesListBox";
            this.codesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.codesListBox.Size = new System.Drawing.Size(50, 95);
            this.codesListBox.TabIndex = 6;
            this.codesListBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // dataLabel
            // 
            this.dataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataLabel.AutoSize = true;
            this.dataLabel.Location = new System.Drawing.Point(9, 129);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(33, 13);
            this.dataLabel.TabIndex = 7;
            this.dataLabel.Text = "Data:";
            // 
            // dataTextBox
            // 
            this.dataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataTextBox.Location = new System.Drawing.Point(12, 145);
            this.dataTextBox.Name = "dataTextBox";
            this.dataTextBox.Size = new System.Drawing.Size(218, 20);
            this.dataTextBox.TabIndex = 8;
            // 
            // replaceWithLabel
            // 
            this.replaceWithLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.replaceWithLabel.AutoSize = true;
            this.replaceWithLabel.Location = new System.Drawing.Point(9, 168);
            this.replaceWithLabel.Name = "replaceWithLabel";
            this.replaceWithLabel.Size = new System.Drawing.Size(75, 13);
            this.replaceWithLabel.TabIndex = 9;
            this.replaceWithLabel.Text = "Replace With:";
            // 
            // replaceWithTextBox
            // 
            this.replaceWithTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceWithTextBox.Location = new System.Drawing.Point(12, 184);
            this.replaceWithTextBox.Name = "replaceWithTextBox";
            this.replaceWithTextBox.Size = new System.Drawing.Size(218, 20);
            this.replaceWithTextBox.TabIndex = 10;
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.replaceAllButton.Location = new System.Drawing.Point(12, 233);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(75, 23);
            this.replaceAllButton.TabIndex = 11;
            this.replaceAllButton.Text = "Replace All";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(93, 233);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // caseSensitiveCheckBox
            // 
            this.caseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.caseSensitiveCheckBox.AutoSize = true;
            this.caseSensitiveCheckBox.Location = new System.Drawing.Point(12, 210);
            this.caseSensitiveCheckBox.Name = "caseSensitiveCheckBox";
            this.caseSensitiveCheckBox.Size = new System.Drawing.Size(96, 17);
            this.caseSensitiveCheckBox.TabIndex = 13;
            this.caseSensitiveCheckBox.Text = "Case Sensitive";
            this.caseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // FindReplaceForm
            // 
            this.AcceptButton = this.replaceAllButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(242, 268);
            this.Controls.Add(this.caseSensitiveCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.replaceAllButton);
            this.Controls.Add(this.replaceWithTextBox);
            this.Controls.Add(this.replaceWithLabel);
            this.Controls.Add(this.dataTextBox);
            this.Controls.Add(this.dataLabel);
            this.Controls.Add(this.codesListBox);
            this.Controls.Add(this.codesLabel);
            this.Controls.Add(this.ind2ListBox);
            this.Controls.Add(this.ind1ListBox);
            this.Controls.Add(this.indicatorsLabel);
            this.Controls.Add(this.tagsLabel);
            this.Controls.Add(this.tagsListBox);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(258, 307);
            this.Name = "FindReplaceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find and Replace";
            this.Load += new System.EventHandler(this.FindReplaceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox tagsListBox;
        private System.Windows.Forms.Label tagsLabel;
        private System.Windows.Forms.Label indicatorsLabel;
        private System.Windows.Forms.ListBox ind1ListBox;
        private System.Windows.Forms.ListBox ind2ListBox;
        private System.Windows.Forms.Label codesLabel;
        private System.Windows.Forms.ListBox codesListBox;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.TextBox dataTextBox;
        private System.Windows.Forms.Label replaceWithLabel;
        private System.Windows.Forms.TextBox replaceWithTextBox;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox caseSensitiveCheckBox;
    }
}