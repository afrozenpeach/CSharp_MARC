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
 * @author    Matt Schraeder <mschraeder@csharpmarc.net>
 * @copyright 2016 Matt Schraeder
 * @license   http://www.gnu.org/licenses/gpl-3.0.html  GPL License 3
 */

namespace CSharp_MARC_Editor
{
    partial class CustomFieldsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFieldsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataLabel1 = new System.Windows.Forms.Label();
            this.codeLabel1 = new System.Windows.Forms.Label();
            this.tagNumberLabel1 = new System.Windows.Forms.Label();
            this.dataTextBox1 = new System.Windows.Forms.TextBox();
            this.codeTextBox1 = new System.Windows.Forms.TextBox();
            this.tagNumberTextBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataLabel2 = new System.Windows.Forms.Label();
            this.codeLabel2 = new System.Windows.Forms.Label();
            this.tagNumberLabel2 = new System.Windows.Forms.Label();
            this.dataTextBox2 = new System.Windows.Forms.TextBox();
            this.codeTextBox2 = new System.Windows.Forms.TextBox();
            this.tagNumberTextBox2 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataLabel3 = new System.Windows.Forms.Label();
            this.codeLabel3 = new System.Windows.Forms.Label();
            this.tagNumberLabel3 = new System.Windows.Forms.Label();
            this.dataTextBox3 = new System.Windows.Forms.TextBox();
            this.codeTextBox3 = new System.Windows.Forms.TextBox();
            this.tagNumberTextBox3 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dataLabel5 = new System.Windows.Forms.Label();
            this.codeLabel5 = new System.Windows.Forms.Label();
            this.tagNumberLabel5 = new System.Windows.Forms.Label();
            this.dataTextBox5 = new System.Windows.Forms.TextBox();
            this.codeTextBox5 = new System.Windows.Forms.TextBox();
            this.tagNumberTextBox5 = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dataLabel4 = new System.Windows.Forms.Label();
            this.codeLabel4 = new System.Windows.Forms.Label();
            this.tagNumberLabel4 = new System.Windows.Forms.Label();
            this.dataTextBox4 = new System.Windows.Forms.TextBox();
            this.codeTextBox4 = new System.Windows.Forms.TextBox();
            this.tagNumberTextBox4 = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataLabel1);
            this.groupBox1.Controls.Add(this.codeLabel1);
            this.groupBox1.Controls.Add(this.tagNumberLabel1);
            this.groupBox1.Controls.Add(this.dataTextBox1);
            this.groupBox1.Controls.Add(this.codeTextBox1);
            this.groupBox1.Controls.Add(this.tagNumberTextBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom Field 1";
            // 
            // dataLabel1
            // 
            this.dataLabel1.AutoSize = true;
            this.dataLabel1.Location = new System.Drawing.Point(42, 74);
            this.dataLabel1.Name = "dataLabel1";
            this.dataLabel1.Size = new System.Drawing.Size(33, 13);
            this.dataLabel1.TabIndex = 5;
            this.dataLabel1.Text = "Data:";
            // 
            // codeLabel1
            // 
            this.codeLabel1.AutoSize = true;
            this.codeLabel1.Location = new System.Drawing.Point(40, 48);
            this.codeLabel1.Name = "codeLabel1";
            this.codeLabel1.Size = new System.Drawing.Size(35, 13);
            this.codeLabel1.TabIndex = 4;
            this.codeLabel1.Text = "Code:";
            // 
            // tagNumberLabel1
            // 
            this.tagNumberLabel1.AutoSize = true;
            this.tagNumberLabel1.Location = new System.Drawing.Point(6, 22);
            this.tagNumberLabel1.Name = "tagNumberLabel1";
            this.tagNumberLabel1.Size = new System.Drawing.Size(69, 13);
            this.tagNumberLabel1.TabIndex = 3;
            this.tagNumberLabel1.Text = "Tag Number:";
            // 
            // dataTextBox1
            // 
            this.dataTextBox1.Location = new System.Drawing.Point(81, 71);
            this.dataTextBox1.Name = "dataTextBox1";
            this.dataTextBox1.Size = new System.Drawing.Size(100, 20);
            this.dataTextBox1.TabIndex = 2;
            // 
            // codeTextBox1
            // 
            this.codeTextBox1.Location = new System.Drawing.Point(81, 45);
            this.codeTextBox1.MaxLength = 1;
            this.codeTextBox1.Name = "codeTextBox1";
            this.codeTextBox1.Size = new System.Drawing.Size(100, 20);
            this.codeTextBox1.TabIndex = 1;
            // 
            // tagNumberTextBox1
            // 
            this.tagNumberTextBox1.Location = new System.Drawing.Point(81, 19);
            this.tagNumberTextBox1.MaxLength = 3;
            this.tagNumberTextBox1.Name = "tagNumberTextBox1";
            this.tagNumberTextBox1.Size = new System.Drawing.Size(100, 20);
            this.tagNumberTextBox1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataLabel2);
            this.groupBox2.Controls.Add(this.codeLabel2);
            this.groupBox2.Controls.Add(this.tagNumberLabel2);
            this.groupBox2.Controls.Add(this.dataTextBox2);
            this.groupBox2.Controls.Add(this.codeTextBox2);
            this.groupBox2.Controls.Add(this.tagNumberTextBox2);
            this.groupBox2.Location = new System.Drawing.Point(205, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Custom Field 2";
            // 
            // dataLabel2
            // 
            this.dataLabel2.AutoSize = true;
            this.dataLabel2.Location = new System.Drawing.Point(42, 74);
            this.dataLabel2.Name = "dataLabel2";
            this.dataLabel2.Size = new System.Drawing.Size(33, 13);
            this.dataLabel2.TabIndex = 5;
            this.dataLabel2.Text = "Data:";
            // 
            // codeLabel2
            // 
            this.codeLabel2.AutoSize = true;
            this.codeLabel2.Location = new System.Drawing.Point(40, 48);
            this.codeLabel2.Name = "codeLabel2";
            this.codeLabel2.Size = new System.Drawing.Size(35, 13);
            this.codeLabel2.TabIndex = 4;
            this.codeLabel2.Text = "Code:";
            // 
            // tagNumberLabel2
            // 
            this.tagNumberLabel2.AutoSize = true;
            this.tagNumberLabel2.Location = new System.Drawing.Point(6, 22);
            this.tagNumberLabel2.Name = "tagNumberLabel2";
            this.tagNumberLabel2.Size = new System.Drawing.Size(69, 13);
            this.tagNumberLabel2.TabIndex = 3;
            this.tagNumberLabel2.Text = "Tag Number:";
            // 
            // dataTextBox2
            // 
            this.dataTextBox2.Location = new System.Drawing.Point(81, 71);
            this.dataTextBox2.Name = "dataTextBox2";
            this.dataTextBox2.Size = new System.Drawing.Size(100, 20);
            this.dataTextBox2.TabIndex = 2;
            // 
            // codeTextBox2
            // 
            this.codeTextBox2.Location = new System.Drawing.Point(81, 45);
            this.codeTextBox2.MaxLength = 1;
            this.codeTextBox2.Name = "codeTextBox2";
            this.codeTextBox2.Size = new System.Drawing.Size(100, 20);
            this.codeTextBox2.TabIndex = 1;
            // 
            // tagNumberTextBox2
            // 
            this.tagNumberTextBox2.Location = new System.Drawing.Point(81, 19);
            this.tagNumberTextBox2.MaxLength = 3;
            this.tagNumberTextBox2.Name = "tagNumberTextBox2";
            this.tagNumberTextBox2.Size = new System.Drawing.Size(100, 20);
            this.tagNumberTextBox2.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dataLabel3);
            this.groupBox3.Controls.Add(this.codeLabel3);
            this.groupBox3.Controls.Add(this.tagNumberLabel3);
            this.groupBox3.Controls.Add(this.dataTextBox3);
            this.groupBox3.Controls.Add(this.codeTextBox3);
            this.groupBox3.Controls.Add(this.tagNumberTextBox3);
            this.groupBox3.Location = new System.Drawing.Point(398, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Custom Field 3";
            // 
            // dataLabel3
            // 
            this.dataLabel3.AutoSize = true;
            this.dataLabel3.Location = new System.Drawing.Point(42, 74);
            this.dataLabel3.Name = "dataLabel3";
            this.dataLabel3.Size = new System.Drawing.Size(33, 13);
            this.dataLabel3.TabIndex = 5;
            this.dataLabel3.Text = "Data:";
            // 
            // codeLabel3
            // 
            this.codeLabel3.AutoSize = true;
            this.codeLabel3.Location = new System.Drawing.Point(40, 48);
            this.codeLabel3.Name = "codeLabel3";
            this.codeLabel3.Size = new System.Drawing.Size(35, 13);
            this.codeLabel3.TabIndex = 4;
            this.codeLabel3.Text = "Code:";
            // 
            // tagNumberLabel3
            // 
            this.tagNumberLabel3.AutoSize = true;
            this.tagNumberLabel3.Location = new System.Drawing.Point(6, 22);
            this.tagNumberLabel3.Name = "tagNumberLabel3";
            this.tagNumberLabel3.Size = new System.Drawing.Size(69, 13);
            this.tagNumberLabel3.TabIndex = 3;
            this.tagNumberLabel3.Text = "Tag Number:";
            // 
            // dataTextBox3
            // 
            this.dataTextBox3.Location = new System.Drawing.Point(81, 71);
            this.dataTextBox3.Name = "dataTextBox3";
            this.dataTextBox3.Size = new System.Drawing.Size(100, 20);
            this.dataTextBox3.TabIndex = 2;
            // 
            // codeTextBox3
            // 
            this.codeTextBox3.Location = new System.Drawing.Point(81, 45);
            this.codeTextBox3.MaxLength = 1;
            this.codeTextBox3.Name = "codeTextBox3";
            this.codeTextBox3.Size = new System.Drawing.Size(100, 20);
            this.codeTextBox3.TabIndex = 1;
            // 
            // tagNumberTextBox3
            // 
            this.tagNumberTextBox3.Location = new System.Drawing.Point(81, 19);
            this.tagNumberTextBox3.MaxLength = 3;
            this.tagNumberTextBox3.Name = "tagNumberTextBox3";
            this.tagNumberTextBox3.Size = new System.Drawing.Size(100, 20);
            this.tagNumberTextBox3.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dataLabel5);
            this.groupBox4.Controls.Add(this.codeLabel5);
            this.groupBox4.Controls.Add(this.tagNumberLabel5);
            this.groupBox4.Controls.Add(this.dataTextBox5);
            this.groupBox4.Controls.Add(this.codeTextBox5);
            this.groupBox4.Controls.Add(this.tagNumberTextBox5);
            this.groupBox4.Location = new System.Drawing.Point(301, 118);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(187, 100);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Custom Field 5";
            // 
            // dataLabel5
            // 
            this.dataLabel5.AutoSize = true;
            this.dataLabel5.Location = new System.Drawing.Point(42, 74);
            this.dataLabel5.Name = "dataLabel5";
            this.dataLabel5.Size = new System.Drawing.Size(33, 13);
            this.dataLabel5.TabIndex = 5;
            this.dataLabel5.Text = "Data:";
            // 
            // codeLabel5
            // 
            this.codeLabel5.AutoSize = true;
            this.codeLabel5.Location = new System.Drawing.Point(40, 48);
            this.codeLabel5.Name = "codeLabel5";
            this.codeLabel5.Size = new System.Drawing.Size(35, 13);
            this.codeLabel5.TabIndex = 4;
            this.codeLabel5.Text = "Code:";
            // 
            // tagNumberLabel5
            // 
            this.tagNumberLabel5.AutoSize = true;
            this.tagNumberLabel5.Location = new System.Drawing.Point(6, 22);
            this.tagNumberLabel5.Name = "tagNumberLabel5";
            this.tagNumberLabel5.Size = new System.Drawing.Size(69, 13);
            this.tagNumberLabel5.TabIndex = 3;
            this.tagNumberLabel5.Text = "Tag Number:";
            // 
            // dataTextBox5
            // 
            this.dataTextBox5.Location = new System.Drawing.Point(81, 71);
            this.dataTextBox5.Name = "dataTextBox5";
            this.dataTextBox5.Size = new System.Drawing.Size(100, 20);
            this.dataTextBox5.TabIndex = 2;
            // 
            // codeTextBox5
            // 
            this.codeTextBox5.Location = new System.Drawing.Point(81, 45);
            this.codeTextBox5.MaxLength = 1;
            this.codeTextBox5.Name = "codeTextBox5";
            this.codeTextBox5.Size = new System.Drawing.Size(100, 20);
            this.codeTextBox5.TabIndex = 1;
            // 
            // tagNumberTextBox5
            // 
            this.tagNumberTextBox5.Location = new System.Drawing.Point(81, 19);
            this.tagNumberTextBox5.MaxLength = 3;
            this.tagNumberTextBox5.Name = "tagNumberTextBox5";
            this.tagNumberTextBox5.Size = new System.Drawing.Size(100, 20);
            this.tagNumberTextBox5.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dataLabel4);
            this.groupBox5.Controls.Add(this.codeLabel4);
            this.groupBox5.Controls.Add(this.tagNumberLabel4);
            this.groupBox5.Controls.Add(this.dataTextBox4);
            this.groupBox5.Controls.Add(this.codeTextBox4);
            this.groupBox5.Controls.Add(this.tagNumberTextBox4);
            this.groupBox5.Location = new System.Drawing.Point(108, 118);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(187, 100);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Custom Field 4";
            // 
            // dataLabel4
            // 
            this.dataLabel4.AutoSize = true;
            this.dataLabel4.Location = new System.Drawing.Point(42, 74);
            this.dataLabel4.Name = "dataLabel4";
            this.dataLabel4.Size = new System.Drawing.Size(33, 13);
            this.dataLabel4.TabIndex = 5;
            this.dataLabel4.Text = "Data:";
            // 
            // codeLabel4
            // 
            this.codeLabel4.AutoSize = true;
            this.codeLabel4.Location = new System.Drawing.Point(40, 48);
            this.codeLabel4.Name = "codeLabel4";
            this.codeLabel4.Size = new System.Drawing.Size(35, 13);
            this.codeLabel4.TabIndex = 4;
            this.codeLabel4.Text = "Code:";
            // 
            // tagNumberLabel4
            // 
            this.tagNumberLabel4.AutoSize = true;
            this.tagNumberLabel4.Location = new System.Drawing.Point(6, 22);
            this.tagNumberLabel4.Name = "tagNumberLabel4";
            this.tagNumberLabel4.Size = new System.Drawing.Size(69, 13);
            this.tagNumberLabel4.TabIndex = 3;
            this.tagNumberLabel4.Text = "Tag Number:";
            // 
            // dataTextBox4
            // 
            this.dataTextBox4.Location = new System.Drawing.Point(81, 71);
            this.dataTextBox4.Name = "dataTextBox4";
            this.dataTextBox4.Size = new System.Drawing.Size(100, 20);
            this.dataTextBox4.TabIndex = 2;
            // 
            // codeTextBox4
            // 
            this.codeTextBox4.Location = new System.Drawing.Point(81, 45);
            this.codeTextBox4.MaxLength = 1;
            this.codeTextBox4.Name = "codeTextBox4";
            this.codeTextBox4.Size = new System.Drawing.Size(100, 20);
            this.codeTextBox4.TabIndex = 1;
            // 
            // tagNumberTextBox4
            // 
            this.tagNumberTextBox4.Location = new System.Drawing.Point(81, 19);
            this.tagNumberTextBox4.MaxLength = 3;
            this.tagNumberTextBox4.Name = "tagNumberTextBox4";
            this.tagNumberTextBox4.Size = new System.Drawing.Size(100, 20);
            this.tagNumberTextBox4.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(220, 224);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(301, 224);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // CustomFieldsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(597, 259);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(613, 298);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(613, 298);
            this.Name = "CustomFieldsForm";
            this.Text = "Custom Fields";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.CustomFieldsForm_HelpButtonClicked);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label dataLabel1;
        private System.Windows.Forms.Label codeLabel1;
        private System.Windows.Forms.Label tagNumberLabel1;
        private System.Windows.Forms.TextBox dataTextBox1;
        private System.Windows.Forms.TextBox codeTextBox1;
        private System.Windows.Forms.TextBox tagNumberTextBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label dataLabel2;
        private System.Windows.Forms.Label codeLabel2;
        private System.Windows.Forms.Label tagNumberLabel2;
        private System.Windows.Forms.TextBox dataTextBox2;
        private System.Windows.Forms.TextBox codeTextBox2;
        private System.Windows.Forms.TextBox tagNumberTextBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label dataLabel3;
        private System.Windows.Forms.Label codeLabel3;
        private System.Windows.Forms.Label tagNumberLabel3;
        private System.Windows.Forms.TextBox dataTextBox3;
        private System.Windows.Forms.TextBox codeTextBox3;
        private System.Windows.Forms.TextBox tagNumberTextBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label dataLabel5;
        private System.Windows.Forms.Label codeLabel5;
        private System.Windows.Forms.Label tagNumberLabel5;
        private System.Windows.Forms.TextBox dataTextBox5;
        private System.Windows.Forms.TextBox codeTextBox5;
        private System.Windows.Forms.TextBox tagNumberTextBox5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label dataLabel4;
        private System.Windows.Forms.Label codeLabel4;
        private System.Windows.Forms.Label tagNumberLabel4;
        private System.Windows.Forms.TextBox dataTextBox4;
        private System.Windows.Forms.TextBox codeTextBox4;
        private System.Windows.Forms.TextBox tagNumberTextBox4;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}