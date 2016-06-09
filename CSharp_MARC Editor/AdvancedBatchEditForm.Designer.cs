namespace CSharp_MARC_Editor
{
    partial class AdvancedBatchEditForm
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
            this.dataTextBox = new System.Windows.Forms.TextBox();
            this.dataLabel = new System.Windows.Forms.Label();
            this.codesListBox = new System.Windows.Forms.ListBox();
            this.codesLabel = new System.Windows.Forms.Label();
            this.ind2ListBox = new System.Windows.Forms.ListBox();
            this.ind1ListBox = new System.Windows.Forms.ListBox();
            this.indicatorsLabel = new System.Windows.Forms.Label();
            this.tagsLabel = new System.Windows.Forms.Label();
            this.tagsListBox = new System.Windows.Forms.ListBox();
            this.conditionsGroupBox = new System.Windows.Forms.GroupBox();
            this.regexCheckBox = new System.Windows.Forms.CheckBox();
            this.caseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.modificationGroupBox = new System.Windows.Forms.GroupBox();
            this.actionLabel = new System.Windows.Forms.Label();
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.tagLabel = new System.Windows.Forms.Label();
            this.tagTextBox = new System.Windows.Forms.TextBox();
            this.ind1Label = new System.Windows.Forms.Label();
            this.ind1TextBox = new System.Windows.Forms.TextBox();
            this.ind2TextBox = new System.Windows.Forms.TextBox();
            this.ind2Label = new System.Windows.Forms.Label();
            this.modificationsDataGridView = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.conditionsGroupBox.SuspendLayout();
            this.modificationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modificationsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataTextBox
            // 
            this.dataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataTextBox.Location = new System.Drawing.Point(9, 162);
            this.dataTextBox.Name = "dataTextBox";
            this.dataTextBox.Size = new System.Drawing.Size(218, 20);
            this.dataTextBox.TabIndex = 17;
            // 
            // dataLabel
            // 
            this.dataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dataLabel.AutoSize = true;
            this.dataLabel.Location = new System.Drawing.Point(6, 146);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(33, 13);
            this.dataLabel.TabIndex = 16;
            this.dataLabel.Text = "Data:";
            // 
            // codesListBox
            // 
            this.codesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.codesListBox.FormattingEnabled = true;
            this.codesListBox.Location = new System.Drawing.Point(177, 32);
            this.codesListBox.Name = "codesListBox";
            this.codesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.codesListBox.Size = new System.Drawing.Size(50, 108);
            this.codesListBox.TabIndex = 15;
            // 
            // codesLabel
            // 
            this.codesLabel.AutoSize = true;
            this.codesLabel.Location = new System.Drawing.Point(174, 16);
            this.codesLabel.Name = "codesLabel";
            this.codesLabel.Size = new System.Drawing.Size(40, 13);
            this.codesLabel.TabIndex = 14;
            this.codesLabel.Text = "Codes:";
            // 
            // ind2ListBox
            // 
            this.ind2ListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ind2ListBox.FormattingEnabled = true;
            this.ind2ListBox.Location = new System.Drawing.Point(121, 32);
            this.ind2ListBox.Name = "ind2ListBox";
            this.ind2ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ind2ListBox.Size = new System.Drawing.Size(50, 108);
            this.ind2ListBox.TabIndex = 13;
            // 
            // ind1ListBox
            // 
            this.ind1ListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ind1ListBox.FormattingEnabled = true;
            this.ind1ListBox.Location = new System.Drawing.Point(65, 32);
            this.ind1ListBox.Name = "ind1ListBox";
            this.ind1ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ind1ListBox.Size = new System.Drawing.Size(50, 108);
            this.ind1ListBox.TabIndex = 12;
            // 
            // indicatorsLabel
            // 
            this.indicatorsLabel.AutoSize = true;
            this.indicatorsLabel.Location = new System.Drawing.Point(62, 16);
            this.indicatorsLabel.Name = "indicatorsLabel";
            this.indicatorsLabel.Size = new System.Drawing.Size(56, 13);
            this.indicatorsLabel.TabIndex = 11;
            this.indicatorsLabel.Text = "Indicators:";
            // 
            // tagsLabel
            // 
            this.tagsLabel.AutoSize = true;
            this.tagsLabel.Location = new System.Drawing.Point(6, 16);
            this.tagsLabel.Name = "tagsLabel";
            this.tagsLabel.Size = new System.Drawing.Size(34, 13);
            this.tagsLabel.TabIndex = 10;
            this.tagsLabel.Text = "Tags:";
            // 
            // tagsListBox
            // 
            this.tagsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tagsListBox.FormattingEnabled = true;
            this.tagsListBox.Location = new System.Drawing.Point(9, 32);
            this.tagsListBox.Name = "tagsListBox";
            this.tagsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.tagsListBox.Size = new System.Drawing.Size(50, 108);
            this.tagsListBox.TabIndex = 9;
            // 
            // conditionsGroupBox
            // 
            this.conditionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.conditionsGroupBox.Controls.Add(this.regexCheckBox);
            this.conditionsGroupBox.Controls.Add(this.tagsLabel);
            this.conditionsGroupBox.Controls.Add(this.indicatorsLabel);
            this.conditionsGroupBox.Controls.Add(this.codesListBox);
            this.conditionsGroupBox.Controls.Add(this.caseSensitiveCheckBox);
            this.conditionsGroupBox.Controls.Add(this.dataLabel);
            this.conditionsGroupBox.Controls.Add(this.ind2ListBox);
            this.conditionsGroupBox.Controls.Add(this.ind1ListBox);
            this.conditionsGroupBox.Controls.Add(this.dataTextBox);
            this.conditionsGroupBox.Controls.Add(this.tagsListBox);
            this.conditionsGroupBox.Controls.Add(this.codesLabel);
            this.conditionsGroupBox.Location = new System.Drawing.Point(12, 12);
            this.conditionsGroupBox.Name = "conditionsGroupBox";
            this.conditionsGroupBox.Size = new System.Drawing.Size(233, 211);
            this.conditionsGroupBox.TabIndex = 18;
            this.conditionsGroupBox.TabStop = false;
            this.conditionsGroupBox.Text = "Conditions:";
            // 
            // regexCheckBox
            // 
            this.regexCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.regexCheckBox.AutoSize = true;
            this.regexCheckBox.Location = new System.Drawing.Point(110, 188);
            this.regexCheckBox.Name = "regexCheckBox";
            this.regexCheckBox.Size = new System.Drawing.Size(122, 17);
            this.regexCheckBox.TabIndex = 20;
            this.regexCheckBox.Text = "Regular Expressions";
            this.regexCheckBox.UseVisualStyleBackColor = true;
            // 
            // caseSensitiveCheckBox
            // 
            this.caseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.caseSensitiveCheckBox.AutoSize = true;
            this.caseSensitiveCheckBox.Location = new System.Drawing.Point(9, 188);
            this.caseSensitiveCheckBox.Name = "caseSensitiveCheckBox";
            this.caseSensitiveCheckBox.Size = new System.Drawing.Size(96, 17);
            this.caseSensitiveCheckBox.TabIndex = 19;
            this.caseSensitiveCheckBox.Text = "Case Sensitive";
            this.caseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // modificationGroupBox
            // 
            this.modificationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modificationGroupBox.Controls.Add(this.cancelButton);
            this.modificationGroupBox.Controls.Add(this.okButton);
            this.modificationGroupBox.Controls.Add(this.modificationsDataGridView);
            this.modificationGroupBox.Controls.Add(this.ind2TextBox);
            this.modificationGroupBox.Controls.Add(this.ind2Label);
            this.modificationGroupBox.Controls.Add(this.ind1TextBox);
            this.modificationGroupBox.Controls.Add(this.ind1Label);
            this.modificationGroupBox.Controls.Add(this.tagTextBox);
            this.modificationGroupBox.Controls.Add(this.tagLabel);
            this.modificationGroupBox.Controls.Add(this.actionComboBox);
            this.modificationGroupBox.Controls.Add(this.actionLabel);
            this.modificationGroupBox.Location = new System.Drawing.Point(251, 12);
            this.modificationGroupBox.Name = "modificationGroupBox";
            this.modificationGroupBox.Size = new System.Drawing.Size(486, 211);
            this.modificationGroupBox.TabIndex = 19;
            this.modificationGroupBox.TabStop = false;
            this.modificationGroupBox.Text = "Modifications:";
            // 
            // actionLabel
            // 
            this.actionLabel.AutoSize = true;
            this.actionLabel.Location = new System.Drawing.Point(6, 22);
            this.actionLabel.Name = "actionLabel";
            this.actionLabel.Size = new System.Drawing.Size(40, 13);
            this.actionLabel.TabIndex = 0;
            this.actionLabel.Text = "Action:";
            // 
            // actionComboBox
            // 
            this.actionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Items.AddRange(new object[] {
            "Add",
            "Delete",
            "Edit",
            "Replace"});
            this.actionComboBox.Location = new System.Drawing.Point(52, 19);
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size(65, 21);
            this.actionComboBox.TabIndex = 1;
            // 
            // tagLabel
            // 
            this.tagLabel.AutoSize = true;
            this.tagLabel.Location = new System.Drawing.Point(123, 22);
            this.tagLabel.Name = "tagLabel";
            this.tagLabel.Size = new System.Drawing.Size(29, 13);
            this.tagLabel.TabIndex = 2;
            this.tagLabel.Text = "Tag:";
            // 
            // tagTextBox
            // 
            this.tagTextBox.Location = new System.Drawing.Point(158, 19);
            this.tagTextBox.MaxLength = 3;
            this.tagTextBox.Name = "tagTextBox";
            this.tagTextBox.Size = new System.Drawing.Size(68, 20);
            this.tagTextBox.TabIndex = 3;
            // 
            // ind1Label
            // 
            this.ind1Label.AutoSize = true;
            this.ind1Label.Location = new System.Drawing.Point(232, 22);
            this.ind1Label.Name = "ind1Label";
            this.ind1Label.Size = new System.Drawing.Size(34, 13);
            this.ind1Label.TabIndex = 4;
            this.ind1Label.Text = "Ind 1:";
            // 
            // ind1TextBox
            // 
            this.ind1TextBox.Location = new System.Drawing.Point(272, 19);
            this.ind1TextBox.MaxLength = 1;
            this.ind1TextBox.Name = "ind1TextBox";
            this.ind1TextBox.Size = new System.Drawing.Size(37, 20);
            this.ind1TextBox.TabIndex = 5;
            // 
            // ind2TextBox
            // 
            this.ind2TextBox.Location = new System.Drawing.Point(355, 19);
            this.ind2TextBox.MaxLength = 1;
            this.ind2TextBox.Name = "ind2TextBox";
            this.ind2TextBox.Size = new System.Drawing.Size(37, 20);
            this.ind2TextBox.TabIndex = 7;
            // 
            // ind2Label
            // 
            this.ind2Label.AutoSize = true;
            this.ind2Label.Location = new System.Drawing.Point(315, 22);
            this.ind2Label.Name = "ind2Label";
            this.ind2Label.Size = new System.Drawing.Size(34, 13);
            this.ind2Label.TabIndex = 6;
            this.ind2Label.Text = "Ind 2:";
            // 
            // modificationsDataGridView
            // 
            this.modificationsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modificationsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.modificationsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.Data});
            this.modificationsDataGridView.Location = new System.Drawing.Point(6, 46);
            this.modificationsDataGridView.Name = "modificationsDataGridView";
            this.modificationsDataGridView.Size = new System.Drawing.Size(474, 130);
            this.modificationsDataGridView.TabIndex = 8;
            // 
            // Code
            // 
            this.Code.HeaderText = "Code";
            this.Code.MaxInputLength = 1;
            this.Code.MinimumWidth = 50;
            this.Code.Name = "Code";
            this.Code.Width = 50;
            // 
            // Data
            // 
            this.Data.HeaderText = "Data";
            this.Data.MinimumWidth = 50;
            this.Data.Name = "Data";
            this.Data.Width = 300;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.Location = new System.Drawing.Point(165, 182);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(246, 182);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // AdvancedBatchEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 235);
            this.Controls.Add(this.modificationGroupBox);
            this.Controls.Add(this.conditionsGroupBox);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedBatchEditForm";
            this.Text = "Advanced Batch Edit";
            this.Load += new System.EventHandler(this.AdvancedBatchEditForm_Load);
            this.conditionsGroupBox.ResumeLayout(false);
            this.conditionsGroupBox.PerformLayout();
            this.modificationGroupBox.ResumeLayout(false);
            this.modificationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modificationsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox dataTextBox;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.ListBox codesListBox;
        private System.Windows.Forms.Label codesLabel;
        private System.Windows.Forms.ListBox ind2ListBox;
        private System.Windows.Forms.ListBox ind1ListBox;
        private System.Windows.Forms.Label indicatorsLabel;
        private System.Windows.Forms.Label tagsLabel;
        private System.Windows.Forms.ListBox tagsListBox;
        private System.Windows.Forms.GroupBox conditionsGroupBox;
        private System.Windows.Forms.CheckBox regexCheckBox;
        private System.Windows.Forms.CheckBox caseSensitiveCheckBox;
        private System.Windows.Forms.GroupBox modificationGroupBox;
        private System.Windows.Forms.Label actionLabel;
        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.DataGridView modificationsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Data;
        private System.Windows.Forms.TextBox ind2TextBox;
        private System.Windows.Forms.Label ind2Label;
        private System.Windows.Forms.TextBox ind1TextBox;
        private System.Windows.Forms.Label ind1Label;
        private System.Windows.Forms.TextBox tagTextBox;
        private System.Windows.Forms.Label tagLabel;
    }
}