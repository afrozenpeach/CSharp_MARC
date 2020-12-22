
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenMARC = new System.Windows.Forms.Button();
            this.lvRecords = new System.Windows.Forms.ListView();
            this.lblNumRecordsLabel = new System.Windows.Forms.Label();
            this.lblNumRecords = new System.Windows.Forms.Label();
            this.Författare = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Titel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Typ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Bibliotek = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Placering = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "iso2709";
            this.openFileDialog1.Filter = "KoHa listor|*.iso2709|Alla filer|*.*";
            // 
            // btnOpenMARC
            // 
            this.btnOpenMARC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnOpenMARC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenMARC.Location = new System.Drawing.Point(1, 1);
            this.btnOpenMARC.Name = "btnOpenMARC";
            this.btnOpenMARC.Size = new System.Drawing.Size(113, 46);
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
            this.Bibliotek,
            this.Placering});
            this.lvRecords.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvRecords.FullRowSelect = true;
            this.lvRecords.GridLines = true;
            this.lvRecords.HideSelection = false;
            this.lvRecords.Location = new System.Drawing.Point(0, 53);
            this.lvRecords.Name = "lvRecords";
            this.lvRecords.Size = new System.Drawing.Size(1356, 675);
            this.lvRecords.TabIndex = 1;
            this.lvRecords.UseCompatibleStateImageBehavior = false;
            this.lvRecords.View = System.Windows.Forms.View.Details;
            this.lvRecords.SelectedIndexChanged += new System.EventHandler(this.lvRecords_SelectedIndexChanged);
            // 
            // lblNumRecordsLabel
            // 
            this.lblNumRecordsLabel.AutoSize = true;
            this.lblNumRecordsLabel.Location = new System.Drawing.Point(131, 16);
            this.lblNumRecordsLabel.Name = "lblNumRecordsLabel";
            this.lblNumRecordsLabel.Size = new System.Drawing.Size(84, 17);
            this.lblNumRecordsLabel.TabIndex = 2;
            this.lblNumRecordsLabel.Text = "Antal poster";
            // 
            // lblNumRecords
            // 
            this.lblNumRecords.AutoSize = true;
            this.lblNumRecords.Location = new System.Drawing.Point(221, 16);
            this.lblNumRecords.Name = "lblNumRecords";
            this.lblNumRecords.Size = new System.Drawing.Size(13, 17);
            this.lblNumRecords.TabIndex = 3;
            this.lblNumRecords.Text = "-";
            // 
            // Författare
            // 
            this.Författare.Text = "Författare";
            this.Författare.Width = 200;
            // 
            // Titel
            // 
            this.Titel.Text = "Titel";
            this.Titel.Width = 350;
            // 
            // Typ
            // 
            this.Typ.Text = "Typ";
            // 
            // Bibliotek
            // 
            this.Bibliotek.Text = "Bibliotek";
            this.Bibliotek.Width = 120;
            // 
            // Placering
            // 
            this.Placering.Text = "Placering";
            this.Placering.Width = 100;
            // 
            // txtFilter
            // 
            this.txtFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.Location = new System.Drawing.Point(393, 12);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(98, 30);
            this.txtFilter.TabIndex = 4;
            this.txtFilter.Text = "TORE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(288, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Biblioteksfilter:";
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnExport.Enabled = false;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(1231, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(113, 46);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Exportera";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(614, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(12, 17);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = " ";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(1096, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(113, 46);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Radera markerad";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1356, 728);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lblNumRecords);
            this.Controls.Add(this.lblNumRecordsLabel);
            this.Controls.Add(this.lvRecords);
            this.Controls.Add(this.btnOpenMARC);
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
        private System.Windows.Forms.ColumnHeader Bibliotek;
        private System.Windows.Forms.ColumnHeader Placering;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDelete;
    }
}

