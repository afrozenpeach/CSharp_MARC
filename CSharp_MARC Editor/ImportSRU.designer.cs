/**
 * Editor for MARC records
 *
 * This project is built upon the CSharp_MARC project of the same name available
 * at http://csharpmarc.net, which itself is based on the File_MARC package
 * (http://pear.php.net/package/File_MARC) by Dan Scott, which was based on PHP
 * MARC package, originally called "php-marc", that is part of the Emilda
 * Project (http://www.emilda.org). Both projects were released under the LGPL
 * which allowed me to port the project to C# for use with the .NET Framework.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * @author    Matt Schraeder <mschraeder@csharpmarc.net> <mschraeder@btsb.com>
 * @copyright 2016 Matt Schraeder and Bound to Stay Bound Books
 * @license   http://www.gnu.org/licenses/gpl-3.0.html  GPL License 3
 */

namespace CSharp_MARC_Editor
{
	partial class ImportSRU
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportSRU));
            this.searchGroupBox = new System.Windows.Forms.GroupBox();
            this.searchResultsDataGridView = new System.Windows.Forms.DataGridView();
            this.titleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resetButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.authorTextBox = new System.Windows.Forms.TextBox();
            this.authorLabel = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.lccnTextBox = new System.Windows.Forms.TextBox();
            this.lccnLabel = new System.Windows.Forms.Label();
            this.isbnTextBox = new System.Windows.Forms.TextBox();
            this.isbnLabel = new System.Windows.Forms.Label();
            this.importGroupBox = new System.Windows.Forms.GroupBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importRichTextBox = new System.Windows.Forms.RichTextBox();
            this.importButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.serverGroupBox = new System.Windows.Forms.GroupBox();
            this.oclcTextBox = new System.Windows.Forms.TextBox();
            this.oclcWSKeyLabel = new System.Windows.Forms.Label();
            this.otherRadioButton = new System.Windows.Forms.RadioButton();
            this.oclcRadioButton = new System.Windows.Forms.RadioButton();
            this.locRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.namespaceTextBox = new System.Windows.Forms.TextBox();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.searchGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchResultsDataGridView)).BeginInit();
            this.importGroupBox.SuspendLayout();
            this.serverGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchGroupBox
            // 
            this.searchGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.searchGroupBox.Controls.Add(this.searchResultsDataGridView);
            this.searchGroupBox.Controls.Add(this.resetButton);
            this.searchGroupBox.Controls.Add(this.searchButton);
            this.searchGroupBox.Controls.Add(this.authorTextBox);
            this.searchGroupBox.Controls.Add(this.authorLabel);
            this.searchGroupBox.Controls.Add(this.titleTextBox);
            this.searchGroupBox.Controls.Add(this.titleLabel);
            this.searchGroupBox.Controls.Add(this.lccnTextBox);
            this.searchGroupBox.Controls.Add(this.lccnLabel);
            this.searchGroupBox.Controls.Add(this.isbnTextBox);
            this.searchGroupBox.Controls.Add(this.isbnLabel);
            this.searchGroupBox.Location = new System.Drawing.Point(12, 120);
            this.searchGroupBox.Name = "searchGroupBox";
            this.searchGroupBox.Size = new System.Drawing.Size(477, 318);
            this.searchGroupBox.TabIndex = 0;
            this.searchGroupBox.TabStop = false;
            this.searchGroupBox.Text = "Search";
            // 
            // searchResultsDataGridView
            // 
            this.searchResultsDataGridView.AllowUserToAddRows = false;
            this.searchResultsDataGridView.AllowUserToDeleteRows = false;
            this.searchResultsDataGridView.AllowUserToResizeRows = false;
            this.searchResultsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchResultsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.searchResultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchResultsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.titleColumn,
            this.authorColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.searchResultsDataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.searchResultsDataGridView.Location = new System.Drawing.Point(6, 126);
            this.searchResultsDataGridView.Name = "searchResultsDataGridView";
            this.searchResultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchResultsDataGridView.Size = new System.Drawing.Size(465, 186);
            this.searchResultsDataGridView.TabIndex = 8;
            this.searchResultsDataGridView.SelectionChanged += new System.EventHandler(this.searchResultsDataGridView_SelectionChanged);
            // 
            // titleColumn
            // 
            this.titleColumn.HeaderText = "Title";
            this.titleColumn.Name = "titleColumn";
            this.titleColumn.Width = 175;
            // 
            // authorColumn
            // 
            this.authorColumn.HeaderText = "Author";
            this.authorColumn.Name = "authorColumn";
            this.authorColumn.Width = 175;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(309, 97);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 6;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(390, 97);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 7;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // authorTextBox
            // 
            this.authorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authorTextBox.Location = new System.Drawing.Point(59, 71);
            this.authorTextBox.Name = "authorTextBox";
            this.authorTextBox.Size = new System.Drawing.Size(412, 20);
            this.authorTextBox.TabIndex = 4;
            this.authorTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchFieldTextBox_KeyPress);
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Location = new System.Drawing.Point(12, 74);
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Size = new System.Drawing.Size(41, 13);
            this.authorLabel.TabIndex = 6;
            this.authorLabel.Text = "Author:";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleTextBox.Location = new System.Drawing.Point(59, 45);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(412, 20);
            this.titleTextBox.TabIndex = 3;
            this.titleTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchFieldTextBox_KeyPress);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(23, 48);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(30, 13);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "Title:";
            // 
            // lccnTextBox
            // 
            this.lccnTextBox.Location = new System.Drawing.Point(209, 19);
            this.lccnTextBox.MaxLength = 10;
            this.lccnTextBox.Name = "lccnTextBox";
            this.lccnTextBox.Size = new System.Drawing.Size(74, 20);
            this.lccnTextBox.TabIndex = 2;
            this.lccnTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchFieldTextBox_KeyPress);
            // 
            // lccnLabel
            // 
            this.lccnLabel.AutoSize = true;
            this.lccnLabel.Location = new System.Drawing.Point(165, 22);
            this.lccnLabel.Name = "lccnLabel";
            this.lccnLabel.Size = new System.Drawing.Size(38, 13);
            this.lccnLabel.TabIndex = 2;
            this.lccnLabel.Text = "LCCN:";
            // 
            // isbnTextBox
            // 
            this.isbnTextBox.Location = new System.Drawing.Point(59, 19);
            this.isbnTextBox.MaxLength = 13;
            this.isbnTextBox.Name = "isbnTextBox";
            this.isbnTextBox.Size = new System.Drawing.Size(100, 20);
            this.isbnTextBox.TabIndex = 1;
            this.isbnTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchFieldTextBox_KeyPress);
            // 
            // isbnLabel
            // 
            this.isbnLabel.AutoSize = true;
            this.isbnLabel.Location = new System.Drawing.Point(18, 22);
            this.isbnLabel.Name = "isbnLabel";
            this.isbnLabel.Size = new System.Drawing.Size(35, 13);
            this.isbnLabel.TabIndex = 0;
            this.isbnLabel.Text = "ISBN:";
            // 
            // importGroupBox
            // 
            this.importGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importGroupBox.Controls.Add(this.cancelButton);
            this.importGroupBox.Controls.Add(this.importRichTextBox);
            this.importGroupBox.Controls.Add(this.importButton);
            this.importGroupBox.Enabled = false;
            this.importGroupBox.Location = new System.Drawing.Point(495, 12);
            this.importGroupBox.Name = "importGroupBox";
            this.importGroupBox.Size = new System.Drawing.Size(354, 426);
            this.importGroupBox.TabIndex = 1;
            this.importGroupBox.TabStop = false;
            this.importGroupBox.Text = "Import Preview";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(180, 397);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // importRichTextBox
            // 
            this.importRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importRichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importRichTextBox.Location = new System.Drawing.Point(6, 19);
            this.importRichTextBox.Name = "importRichTextBox";
            this.importRichTextBox.ReadOnly = true;
            this.importRichTextBox.Size = new System.Drawing.Size(342, 372);
            this.importRichTextBox.TabIndex = 9;
            this.importRichTextBox.Text = "";
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.Location = new System.Drawing.Point(99, 397);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 12;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "MARC Files|*.mrc;*.usm;*.001;*.xml;*.marc";
            // 
            // serverGroupBox
            // 
            this.serverGroupBox.Controls.Add(this.oclcTextBox);
            this.serverGroupBox.Controls.Add(this.oclcWSKeyLabel);
            this.serverGroupBox.Controls.Add(this.otherRadioButton);
            this.serverGroupBox.Controls.Add(this.oclcRadioButton);
            this.serverGroupBox.Controls.Add(this.locRadioButton);
            this.serverGroupBox.Controls.Add(this.label2);
            this.serverGroupBox.Controls.Add(this.label1);
            this.serverGroupBox.Controls.Add(this.namespaceTextBox);
            this.serverGroupBox.Controls.Add(this.serverTextBox);
            this.serverGroupBox.Location = new System.Drawing.Point(12, 12);
            this.serverGroupBox.Name = "serverGroupBox";
            this.serverGroupBox.Size = new System.Drawing.Size(477, 102);
            this.serverGroupBox.TabIndex = 2;
            this.serverGroupBox.TabStop = false;
            this.serverGroupBox.Text = "Server Information";
            // 
            // oclcTextBox
            // 
            this.oclcTextBox.Enabled = false;
            this.oclcTextBox.Location = new System.Drawing.Point(337, 68);
            this.oclcTextBox.Name = "oclcTextBox";
            this.oclcTextBox.Size = new System.Drawing.Size(134, 20);
            this.oclcTextBox.TabIndex = 8;
            // 
            // oclcWSKeyLabel
            // 
            this.oclcWSKeyLabel.AutoSize = true;
            this.oclcWSKeyLabel.Location = new System.Drawing.Point(251, 71);
            this.oclcWSKeyLabel.Name = "oclcWSKeyLabel";
            this.oclcWSKeyLabel.Size = new System.Drawing.Size(80, 13);
            this.oclcWSKeyLabel.TabIndex = 7;
            this.oclcWSKeyLabel.Text = "OCLC WS Key:";
            // 
            // otherRadioButton
            // 
            this.otherRadioButton.AutoSize = true;
            this.otherRadioButton.Location = new System.Drawing.Point(186, 19);
            this.otherRadioButton.Name = "otherRadioButton";
            this.otherRadioButton.Size = new System.Drawing.Size(51, 17);
            this.otherRadioButton.TabIndex = 6;
            this.otherRadioButton.Text = "Other";
            this.otherRadioButton.UseVisualStyleBackColor = true;
            this.otherRadioButton.CheckedChanged += new System.EventHandler(this.otherRadioButton_CheckedChanged);
            // 
            // oclcRadioButton
            // 
            this.oclcRadioButton.AutoSize = true;
            this.oclcRadioButton.Location = new System.Drawing.Point(127, 19);
            this.oclcRadioButton.Name = "oclcRadioButton";
            this.oclcRadioButton.Size = new System.Drawing.Size(53, 17);
            this.oclcRadioButton.TabIndex = 5;
            this.oclcRadioButton.Text = "OCLC";
            this.oclcRadioButton.UseVisualStyleBackColor = true;
            this.oclcRadioButton.CheckedChanged += new System.EventHandler(this.oclcRadioButton_CheckedChanged);
            // 
            // locRadioButton
            // 
            this.locRadioButton.AutoSize = true;
            this.locRadioButton.Checked = true;
            this.locRadioButton.Location = new System.Drawing.Point(6, 19);
            this.locRadioButton.Name = "locRadioButton";
            this.locRadioButton.Size = new System.Drawing.Size(115, 17);
            this.locRadioButton.TabIndex = 4;
            this.locRadioButton.TabStop = true;
            this.locRadioButton.Text = "Library of Congress";
            this.locRadioButton.UseVisualStyleBackColor = true;
            this.locRadioButton.CheckedChanged += new System.EventHandler(this.locRadioButton_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Namespace:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server URL:";
            // 
            // namespaceTextBox
            // 
            this.namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.namespaceTextBox.Enabled = false;
            this.namespaceTextBox.Location = new System.Drawing.Point(79, 68);
            this.namespaceTextBox.Name = "namespaceTextBox";
            this.namespaceTextBox.Size = new System.Drawing.Size(166, 20);
            this.namespaceTextBox.TabIndex = 1;
            this.namespaceTextBox.Text = "http://www.loc.gov/zing/srw/";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverTextBox.Enabled = false;
            this.serverTextBox.Location = new System.Drawing.Point(79, 42);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(392, 20);
            this.serverTextBox.TabIndex = 0;
            this.serverTextBox.Text = "http://lx2.loc.gov:210/lcdb?version=1.1&operation=searchRetrieve&query=";
            // 
            // ImportSRU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 450);
            this.Controls.Add(this.serverGroupBox);
            this.Controls.Add(this.importGroupBox);
            this.Controls.Add(this.searchGroupBox);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(877, 373);
            this.Name = "ImportSRU";
            this.Text = "Import MARC Records";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ImportSRU_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportMARC_FormClosing);
            this.searchGroupBox.ResumeLayout(false);
            this.searchGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchResultsDataGridView)).EndInit();
            this.importGroupBox.ResumeLayout(false);
            this.serverGroupBox.ResumeLayout(false);
            this.serverGroupBox.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox searchGroupBox;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Button searchButton;
		private System.Windows.Forms.TextBox authorTextBox;
		private System.Windows.Forms.Label authorLabel;
		private System.Windows.Forms.TextBox titleTextBox;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.TextBox lccnTextBox;
		private System.Windows.Forms.Label lccnLabel;
		private System.Windows.Forms.TextBox isbnTextBox;
		private System.Windows.Forms.Label isbnLabel;
		private System.Windows.Forms.DataGridView searchResultsDataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn titleColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn authorColumn;
		private System.Windows.Forms.GroupBox importGroupBox;
		private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.RichTextBox importRichTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox serverGroupBox;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox namespaceTextBox;
        private System.Windows.Forms.TextBox oclcTextBox;
        private System.Windows.Forms.Label oclcWSKeyLabel;
        private System.Windows.Forms.RadioButton otherRadioButton;
        private System.Windows.Forms.RadioButton oclcRadioButton;
        private System.Windows.Forms.RadioButton locRadioButton;
    }
}