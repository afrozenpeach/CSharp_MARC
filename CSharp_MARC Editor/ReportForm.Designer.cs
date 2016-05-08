namespace CSharp_MARC_Editor
{
    partial class ReportForm
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
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.mARCDataSet = new CSharp_MARC_Editor.MARCDataSet();
            this.mARCDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mARCDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mARCDataSetBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource2.Name = "RecordsReportDataSet";
            reportDataSource2.Value = this.mARCDataSetBindingSource;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "CSharp_MARC_Editor.Reports.RecordsReport.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(0, 0);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.Size = new System.Drawing.Size(1030, 493);
            this.reportViewer.TabIndex = 0;
            // 
            // mARCDataSet
            // 
            this.mARCDataSet.DataSetName = "MARCDataSet";
            this.mARCDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // mARCDataSetBindingSource
            // 
            this.mARCDataSetBindingSource.DataSource = this.mARCDataSet;
            this.mARCDataSetBindingSource.Position = 0;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 493);
            this.Controls.Add(this.reportViewer);
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.Shown += new System.EventHandler(this.ReportForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.mARCDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mARCDataSetBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.BindingSource mARCDataSetBindingSource;
        private MARCDataSet mARCDataSet;
    }
}