
namespace PMarcWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenMARC = new System.Windows.Forms.Button();
            this.lvRecords = new System.Windows.Forms.ListView();
            this.Författare = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Titel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Typ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Placering = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblNumRecordsLabel = new System.Windows.Forms.Label();
            this.lblNumRecords = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "iso2709";
            this.openFileDialog1.Filter = "KoHa listor|*.iso2709|Alla filer|*.*";
            // 
            // btnOpenMARC
            // 
            this.btnOpenMARC.BackColor = System.Drawing.Color.YellowGreen;
            this.btnOpenMARC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenMARC.Location = new System.Drawing.Point(1, 1);
            this.btnOpenMARC.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenMARC.Name = "btnOpenMARC";
            this.btnOpenMARC.Size = new System.Drawing.Size(85, 37);
            this.btnOpenMARC.TabIndex = 0;
            this.btnOpenMARC.Text = "Öppna lista";
            this.btnOpenMARC.UseVisualStyleBackColor = false;
            this.btnOpenMARC.Click += new System.EventHandler(this.button1_Click);
            // 
            // lvRecords
            // 
            this.lvRecords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Författare,
            this.Titel,
            this.Typ,
            this.Placering});
            this.lvRecords.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvRecords.FullRowSelect = true;
            this.lvRecords.GridLines = true;
            this.lvRecords.HideSelection = false;
            this.lvRecords.Location = new System.Drawing.Point(0, 43);
            this.lvRecords.Margin = new System.Windows.Forms.Padding(2);
            this.lvRecords.Name = "lvRecords";
            this.lvRecords.Size = new System.Drawing.Size(1017, 549);
            this.lvRecords.TabIndex = 1;
            this.lvRecords.UseCompatibleStateImageBehavior = false;
            this.lvRecords.View = System.Windows.Forms.View.Details;
            // 
            // Författare
            // 
            this.Författare.Text = "Författare";
            this.Författare.Width = 250;
            // 
            // Titel
            // 
            this.Titel.Text = "Titel";
            this.Titel.Width = 450;
            // 
            // Typ
            // 
            this.Typ.Text = "Typ";
            this.Typ.Width = 100;
            // 
            // Placering
            // 
            this.Placering.Text = "Placering";
            this.Placering.Width = 200;
            // 
            // lblNumRecordsLabel
            // 
            this.lblNumRecordsLabel.AutoSize = true;
            this.lblNumRecordsLabel.Location = new System.Drawing.Point(98, 13);
            this.lblNumRecordsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumRecordsLabel.Name = "lblNumRecordsLabel";
            this.lblNumRecordsLabel.Size = new System.Drawing.Size(63, 13);
            this.lblNumRecordsLabel.TabIndex = 2;
            this.lblNumRecordsLabel.Text = "Antal poster";
            this.lblNumRecordsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNumRecords
            // 
            this.lblNumRecords.AutoSize = true;
            this.lblNumRecords.Location = new System.Drawing.Point(166, 13);
            this.lblNumRecords.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumRecords.Name = "lblNumRecords";
            this.lblNumRecords.Size = new System.Drawing.Size(10, 13);
            this.lblNumRecords.TabIndex = 3;
            this.lblNumRecords.Text = "-";
            this.lblNumRecords.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.YellowGreen;
            this.btnExport.Enabled = false;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(923, 1);
            this.btnExport.Margin = new System.Windows.Forms.Padding(2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(85, 37);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Exportera";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(503, 13);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = " Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(822, 1);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 37);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Radera markerad";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excelfiler|*.xlsx|Alla filer|*.*";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1017, 592);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblNumRecords);
            this.Controls.Add(this.lblNumRecordsLabel);
            this.Controls.Add(this.lvRecords);
            this.Controls.Add(this.btnOpenMARC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "PMarcWin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnOpenMARC;
        private System.Windows.Forms.ListView lvRecords;
        private System.Windows.Forms.Label lblNumRecordsLabel;
        private System.Windows.Forms.Label lblNumRecords;
        private System.Windows.Forms.ColumnHeader Författare;
        private System.Windows.Forms.ColumnHeader Titel;
        private System.Windows.Forms.ColumnHeader Typ;
        private System.Windows.Forms.ColumnHeader Placering;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

