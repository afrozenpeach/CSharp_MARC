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
 * @author    Mattie Schraeder <mattie@csharpmarc.net>
 * @copyright 2016-2018 Mattie Schraeder
 * @license   http://www.gnu.org/licenses/gpl-3.0.html  GPL License 3
 */

namespace CSharp_MARC_Editor
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.lgpl3PictureBox = new System.Windows.Forms.PictureBox();
            this.btsbPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.emailLabel = new System.Windows.Forms.LinkLabel();
            this.btsbLabel = new System.Windows.Forms.Label();
            this.btsbLinkLabel = new System.Windows.Forms.LinkLabel();
            this.gpl3PictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.lgpl3PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btsbPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpl3PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lgpl3PictureBox
            // 
            this.lgpl3PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lgpl3PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("lgpl3PictureBox.Image")));
            this.lgpl3PictureBox.Location = new System.Drawing.Point(742, 9);
            this.lgpl3PictureBox.Name = "lgpl3PictureBox";
            this.lgpl3PictureBox.Size = new System.Drawing.Size(148, 50);
            this.lgpl3PictureBox.TabIndex = 1;
            this.lgpl3PictureBox.TabStop = false;
            // 
            // btsbPictureBox
            // 
            this.btsbPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btsbPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("btsbPictureBox.Image")));
            this.btsbPictureBox.Location = new System.Drawing.Point(769, 46);
            this.btsbPictureBox.Name = "btsbPictureBox";
            this.btsbPictureBox.Size = new System.Drawing.Size(100, 210);
            this.btsbPictureBox.TabIndex = 2;
            this.btsbPictureBox.TabStop = false;
            this.btsbPictureBox.Click += new System.EventHandler(this.btsbPictureBox_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(261, 385);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Location = new System.Drawing.Point(12, 29);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(262, 17);
            this.copyrightLabel.TabIndex = 4;
            this.copyrightLabel.Text = "Copyright @ 2016-2018 Mattie Schraeder";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(12, 64);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(48, 13);
            this.versionLabel.TabIndex = 5;
            this.versionLabel.Text = "Version: ";
            // 
            // linkLabel
            // 
            this.linkLabel.AutoSize = true;
            this.linkLabel.Location = new System.Drawing.Point(12, 77);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(111, 13);
            this.linkLabel.TabIndex = 6;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "http://csharpmarc.net";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "C# MARC Editor";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 93);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(751, 286);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(12, 46);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(119, 13);
            this.emailLabel.TabIndex = 9;
            this.emailLabel.TabStop = true;
            this.emailLabel.Text = "mattie@csharpmarc.net";
            // 
            // btsbLabel
            // 
            this.btsbLabel.Location = new System.Drawing.Point(769, 247);
            this.btsbLabel.Name = "btsbLabel";
            this.btsbLabel.Size = new System.Drawing.Size(100, 43);
            this.btsbLabel.TabIndex = 10;
            this.btsbLabel.Text = "Special thanks to Bound to Stay Bound Books";
            this.btsbLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btsbLinkLabel
            // 
            this.btsbLinkLabel.AutoSize = true;
            this.btsbLinkLabel.Location = new System.Drawing.Point(765, 290);
            this.btsbLinkLabel.Name = "btsbLinkLabel";
            this.btsbLinkLabel.Size = new System.Drawing.Size(108, 13);
            this.btsbLinkLabel.TabIndex = 11;
            this.btsbLinkLabel.TabStop = true;
            this.btsbLinkLabel.Text = "http://www.btsb.com";
            // 
            // gpl3PictureBox
            // 
            this.gpl3PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gpl3PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("gpl3PictureBox.Image")));
            this.gpl3PictureBox.Location = new System.Drawing.Point(606, 9);
            this.gpl3PictureBox.Name = "gpl3PictureBox";
            this.gpl3PictureBox.Size = new System.Drawing.Size(130, 50);
            this.gpl3PictureBox.TabIndex = 12;
            this.gpl3PictureBox.TabStop = false;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(902, 423);
            this.Controls.Add(this.gpl3PictureBox);
            this.Controls.Add(this.btsbLinkLabel);
            this.Controls.Add(this.btsbLabel);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.lgpl3PictureBox);
            this.Controls.Add(this.btsbPictureBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(918, 462);
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About C# MARC Editor";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lgpl3PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btsbPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpl3PictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox lgpl3PictureBox;
        private System.Windows.Forms.PictureBox btsbPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.LinkLabel emailLabel;
        private System.Windows.Forms.Label btsbLabel;
        private System.Windows.Forms.LinkLabel btsbLinkLabel;
        private System.Windows.Forms.PictureBox gpl3PictureBox;
    }
}