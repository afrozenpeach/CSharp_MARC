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
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.recordsDataGridView = new System.Windows.Forms.DataGridView();
            this.recordIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateAddedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateChangedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.barcodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.classificationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainEntryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.marcDataSet = new CSharp_MARC_Editor.MARCDataSet();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.progressToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.helptextToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.previewTextBox = new System.Windows.Forms.TextBox();
            this.subfieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.subfieldIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldIDDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.fieldIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recordIDDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tagNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ind1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ind2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.controlDataDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.importingBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordListAtTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.exportingBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recordsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.marcDataSet)).BeginInit();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subfieldsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).BeginInit();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer.Panel1.Controls.Add(this.recordsDataGridView);
            this.splitContainer.Panel1MinSize = 50;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.statusStrip);
            this.splitContainer.Panel2.Controls.Add(this.previewTextBox);
            this.splitContainer.Panel2.Controls.Add(this.subfieldsDataGridView);
            this.splitContainer.Panel2.Controls.Add(this.fieldsDataGridView);
            this.splitContainer.Panel2.Enabled = false;
            this.splitContainer.Size = new System.Drawing.Size(984, 613);
            this.splitContainer.SplitterDistance = 150;
            this.splitContainer.SplitterWidth = 15;
            this.splitContainer.TabIndex = 1;
            // 
            // recordsDataGridView
            // 
            this.recordsDataGridView.AllowUserToAddRows = false;
            this.recordsDataGridView.AllowUserToResizeRows = false;
            this.recordsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recordsDataGridView.AutoGenerateColumns = false;
            this.recordsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.recordsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.recordIDDataGridViewTextBoxColumn,
            this.dateAddedDataGridViewTextBoxColumn,
            this.dateChangedDataGridViewTextBoxColumn,
            this.authorDataGridViewTextBoxColumn,
            this.titleDataGridViewTextBoxColumn,
            this.barcodeDataGridViewTextBoxColumn,
            this.classificationDataGridViewTextBoxColumn,
            this.mainEntryDataGridViewTextBoxColumn});
            this.recordsDataGridView.DataMember = "Records";
            this.recordsDataGridView.DataSource = this.marcDataSet;
            this.recordsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.recordsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.recordsDataGridView.MultiSelect = false;
            this.recordsDataGridView.Name = "recordsDataGridView";
            this.recordsDataGridView.ReadOnly = true;
            this.recordsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.recordsDataGridView.Size = new System.Drawing.Size(984, 150);
            this.recordsDataGridView.TabIndex = 0;
            this.recordsDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.recordsDataGridView_CellClick);
            this.recordsDataGridView.SelectionChanged += new System.EventHandler(this.recordsDataGridView_SelectionChanged);
            // 
            // recordIDDataGridViewTextBoxColumn
            // 
            this.recordIDDataGridViewTextBoxColumn.DataPropertyName = "RecordID";
            this.recordIDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.recordIDDataGridViewTextBoxColumn.MinimumWidth = 50;
            this.recordIDDataGridViewTextBoxColumn.Name = "recordIDDataGridViewTextBoxColumn";
            this.recordIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.recordIDDataGridViewTextBoxColumn.Width = 50;
            // 
            // dateAddedDataGridViewTextBoxColumn
            // 
            this.dateAddedDataGridViewTextBoxColumn.DataPropertyName = "DateAdded";
            this.dateAddedDataGridViewTextBoxColumn.HeaderText = "Date Added";
            this.dateAddedDataGridViewTextBoxColumn.MinimumWidth = 125;
            this.dateAddedDataGridViewTextBoxColumn.Name = "dateAddedDataGridViewTextBoxColumn";
            this.dateAddedDataGridViewTextBoxColumn.ReadOnly = true;
            this.dateAddedDataGridViewTextBoxColumn.Width = 125;
            // 
            // dateChangedDataGridViewTextBoxColumn
            // 
            this.dateChangedDataGridViewTextBoxColumn.DataPropertyName = "DateChanged";
            this.dateChangedDataGridViewTextBoxColumn.HeaderText = "Date Changed";
            this.dateChangedDataGridViewTextBoxColumn.MinimumWidth = 125;
            this.dateChangedDataGridViewTextBoxColumn.Name = "dateChangedDataGridViewTextBoxColumn";
            this.dateChangedDataGridViewTextBoxColumn.ReadOnly = true;
            this.dateChangedDataGridViewTextBoxColumn.Width = 125;
            // 
            // authorDataGridViewTextBoxColumn
            // 
            this.authorDataGridViewTextBoxColumn.DataPropertyName = "Author";
            this.authorDataGridViewTextBoxColumn.HeaderText = "Author";
            this.authorDataGridViewTextBoxColumn.MinimumWidth = 20;
            this.authorDataGridViewTextBoxColumn.Name = "authorDataGridViewTextBoxColumn";
            this.authorDataGridViewTextBoxColumn.ReadOnly = true;
            this.authorDataGridViewTextBoxColumn.Width = 200;
            // 
            // titleDataGridViewTextBoxColumn
            // 
            this.titleDataGridViewTextBoxColumn.DataPropertyName = "Title";
            this.titleDataGridViewTextBoxColumn.HeaderText = "Title";
            this.titleDataGridViewTextBoxColumn.MinimumWidth = 20;
            this.titleDataGridViewTextBoxColumn.Name = "titleDataGridViewTextBoxColumn";
            this.titleDataGridViewTextBoxColumn.ReadOnly = true;
            this.titleDataGridViewTextBoxColumn.Width = 400;
            // 
            // barcodeDataGridViewTextBoxColumn
            // 
            this.barcodeDataGridViewTextBoxColumn.DataPropertyName = "Barcode";
            this.barcodeDataGridViewTextBoxColumn.HeaderText = "Barcode";
            this.barcodeDataGridViewTextBoxColumn.Name = "barcodeDataGridViewTextBoxColumn";
            this.barcodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // classificationDataGridViewTextBoxColumn
            // 
            this.classificationDataGridViewTextBoxColumn.DataPropertyName = "Classification";
            this.classificationDataGridViewTextBoxColumn.HeaderText = "Classification";
            this.classificationDataGridViewTextBoxColumn.Name = "classificationDataGridViewTextBoxColumn";
            this.classificationDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mainEntryDataGridViewTextBoxColumn
            // 
            this.mainEntryDataGridViewTextBoxColumn.DataPropertyName = "MainEntry";
            this.mainEntryDataGridViewTextBoxColumn.HeaderText = "MainEntry";
            this.mainEntryDataGridViewTextBoxColumn.Name = "mainEntryDataGridViewTextBoxColumn";
            this.mainEntryDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // marcDataSet
            // 
            this.marcDataSet.DataSetName = "MARCDataSet";
            this.marcDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.progressToolStripStatusLabel,
            this.helptextToolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 426);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(984, 22);
            this.statusStrip.TabIndex = 3;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar.Visible = false;
            // 
            // progressToolStripStatusLabel
            // 
            this.progressToolStripStatusLabel.Name = "progressToolStripStatusLabel";
            this.progressToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // helptextToolStripStatusLabel
            // 
            this.helptextToolStripStatusLabel.Name = "helptextToolStripStatusLabel";
            this.helptextToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // previewTextBox
            // 
            this.previewTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewTextBox.Location = new System.Drawing.Point(624, 3);
            this.previewTextBox.Multiline = true;
            this.previewTextBox.Name = "previewTextBox";
            this.previewTextBox.ReadOnly = true;
            this.previewTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.previewTextBox.Size = new System.Drawing.Size(357, 420);
            this.previewTextBox.TabIndex = 2;
            // 
            // subfieldsDataGridView
            // 
            this.subfieldsDataGridView.AllowUserToResizeColumns = false;
            this.subfieldsDataGridView.AllowUserToResizeRows = false;
            this.subfieldsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.subfieldsDataGridView.AutoGenerateColumns = false;
            this.subfieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.subfieldsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.subfieldIDDataGridViewTextBoxColumn,
            this.fieldIDDataGridViewTextBoxColumn1,
            this.codeDataGridViewTextBoxColumn,
            this.dataDataGridViewTextBoxColumn});
            this.subfieldsDataGridView.DataMember = "Subfields";
            this.subfieldsDataGridView.DataSource = this.marcDataSet;
            this.subfieldsDataGridView.Location = new System.Drawing.Point(211, 3);
            this.subfieldsDataGridView.MultiSelect = false;
            this.subfieldsDataGridView.Name = "subfieldsDataGridView";
            this.subfieldsDataGridView.Size = new System.Drawing.Size(407, 420);
            this.subfieldsDataGridView.TabIndex = 1;
            this.subfieldsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.subfieldsDataGridView_CellBeginEdit);
            this.subfieldsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.subfieldsDataGridView_CellEndEdit);
            this.subfieldsDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.subfieldsDataGridView_CellValidated);
            this.subfieldsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.subfieldsDataGridView_CellValidating);
            // 
            // subfieldIDDataGridViewTextBoxColumn
            // 
            this.subfieldIDDataGridViewTextBoxColumn.DataPropertyName = "SubfieldID";
            this.subfieldIDDataGridViewTextBoxColumn.HeaderText = "SubfieldID";
            this.subfieldIDDataGridViewTextBoxColumn.Name = "subfieldIDDataGridViewTextBoxColumn";
            this.subfieldIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // fieldIDDataGridViewTextBoxColumn1
            // 
            this.fieldIDDataGridViewTextBoxColumn1.DataPropertyName = "FieldID";
            this.fieldIDDataGridViewTextBoxColumn1.HeaderText = "FieldID";
            this.fieldIDDataGridViewTextBoxColumn1.Name = "fieldIDDataGridViewTextBoxColumn1";
            this.fieldIDDataGridViewTextBoxColumn1.Visible = false;
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "Code";
            this.codeDataGridViewTextBoxColumn.HeaderText = "Code";
            this.codeDataGridViewTextBoxColumn.MinimumWidth = 40;
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            this.codeDataGridViewTextBoxColumn.Width = 40;
            // 
            // dataDataGridViewTextBoxColumn
            // 
            this.dataDataGridViewTextBoxColumn.DataPropertyName = "Data";
            this.dataDataGridViewTextBoxColumn.HeaderText = "Data";
            this.dataDataGridViewTextBoxColumn.MinimumWidth = 100;
            this.dataDataGridViewTextBoxColumn.Name = "dataDataGridViewTextBoxColumn";
            this.dataDataGridViewTextBoxColumn.Width = 300;
            // 
            // fieldsDataGridView
            // 
            this.fieldsDataGridView.AllowUserToResizeColumns = false;
            this.fieldsDataGridView.AllowUserToResizeRows = false;
            this.fieldsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fieldsDataGridView.AutoGenerateColumns = false;
            this.fieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fieldsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fieldIDDataGridViewTextBoxColumn,
            this.recordIDDataGridViewTextBoxColumn1,
            this.tagNumberDataGridViewTextBoxColumn,
            this.ind1DataGridViewTextBoxColumn,
            this.ind2DataGridViewTextBoxColumn,
            this.controlDataDataGridViewTextBoxColumn});
            this.fieldsDataGridView.DataMember = "Fields";
            this.fieldsDataGridView.DataSource = this.marcDataSet;
            this.fieldsDataGridView.Location = new System.Drawing.Point(3, 3);
            this.fieldsDataGridView.MultiSelect = false;
            this.fieldsDataGridView.Name = "fieldsDataGridView";
            this.fieldsDataGridView.Size = new System.Drawing.Size(202, 420);
            this.fieldsDataGridView.TabIndex = 0;
            this.fieldsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.fieldsDataGridView_CellBeginEdit);
            this.fieldsDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldsDataGridView_CellClick);
            this.fieldsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldsDataGridView_CellEndEdit);
            this.fieldsDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.fieldsDataGridView_CellValidated);
            this.fieldsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.fieldsDataGridView_CellValidating);
            this.fieldsDataGridView.SelectionChanged += new System.EventHandler(this.fieldsDataGridView_SelectionChanged);
            // 
            // fieldIDDataGridViewTextBoxColumn
            // 
            this.fieldIDDataGridViewTextBoxColumn.DataPropertyName = "FieldID";
            this.fieldIDDataGridViewTextBoxColumn.HeaderText = "FieldID";
            this.fieldIDDataGridViewTextBoxColumn.Name = "fieldIDDataGridViewTextBoxColumn";
            this.fieldIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // recordIDDataGridViewTextBoxColumn1
            // 
            this.recordIDDataGridViewTextBoxColumn1.DataPropertyName = "RecordID";
            this.recordIDDataGridViewTextBoxColumn1.HeaderText = "RecordID";
            this.recordIDDataGridViewTextBoxColumn1.Name = "recordIDDataGridViewTextBoxColumn1";
            this.recordIDDataGridViewTextBoxColumn1.Visible = false;
            // 
            // tagNumberDataGridViewTextBoxColumn
            // 
            this.tagNumberDataGridViewTextBoxColumn.DataPropertyName = "TagNumber";
            this.tagNumberDataGridViewTextBoxColumn.HeaderText = "Tag";
            this.tagNumberDataGridViewTextBoxColumn.MinimumWidth = 45;
            this.tagNumberDataGridViewTextBoxColumn.Name = "tagNumberDataGridViewTextBoxColumn";
            this.tagNumberDataGridViewTextBoxColumn.Width = 45;
            // 
            // ind1DataGridViewTextBoxColumn
            // 
            this.ind1DataGridViewTextBoxColumn.DataPropertyName = "Ind1";
            this.ind1DataGridViewTextBoxColumn.HeaderText = "Ind1";
            this.ind1DataGridViewTextBoxColumn.MinimumWidth = 40;
            this.ind1DataGridViewTextBoxColumn.Name = "ind1DataGridViewTextBoxColumn";
            this.ind1DataGridViewTextBoxColumn.Width = 40;
            // 
            // ind2DataGridViewTextBoxColumn
            // 
            this.ind2DataGridViewTextBoxColumn.DataPropertyName = "Ind2";
            this.ind2DataGridViewTextBoxColumn.HeaderText = "Ind2";
            this.ind2DataGridViewTextBoxColumn.MinimumWidth = 40;
            this.ind2DataGridViewTextBoxColumn.Name = "ind2DataGridViewTextBoxColumn";
            this.ind2DataGridViewTextBoxColumn.Width = 40;
            // 
            // controlDataDataGridViewTextBoxColumn
            // 
            this.controlDataDataGridViewTextBoxColumn.DataPropertyName = "ControlData";
            this.controlDataDataGridViewTextBoxColumn.HeaderText = "ControlData";
            this.controlDataDataGridViewTextBoxColumn.Name = "controlDataDataGridViewTextBoxColumn";
            this.controlDataDataGridViewTextBoxColumn.Visible = false;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "*.mrc";
            this.saveFileDialog.Filter = "MRC Files|*.mrc|USM Files|*.usm|001 Files|*.001|MARCXML Files|*.xml|All Files|*.*" +
    "";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "*.mrc";
            this.openFileDialog.Filter = "MARC Records|*.mrc;*.marc;*.usm;*.001;*.xml|MRC Files|*.mrc|MARC Files|*.marc|USM" +
    " Files|*.usm|001 Files|*.001|MARCXML Files|*.xml|All Files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // importingBackgroundWorker
            // 
            this.importingBackgroundWorker.WorkerReportsProgress = true;
            this.importingBackgroundWorker.WorkerSupportsCancellation = true;
            this.importingBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.importingBackgroundWorker_DoWork);
            this.importingBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.importingBackgroundWorker_ProgressChanged);
            this.importingBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.importingBackgroundWorker_RunWorkerCompleted);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(984, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importRecordsToolStripMenuItem,
            this.exportRecordsToolStripMenuItem,
            this.printToolStripMenuItem,
            this.toolStripSeparator,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importRecordsToolStripMenuItem
            // 
            this.importRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importRecordsToolStripMenuItem.Image")));
            this.importRecordsToolStripMenuItem.Name = "importRecordsToolStripMenuItem";
            this.importRecordsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.importRecordsToolStripMenuItem.Text = "Import Records";
            this.importRecordsToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportRecordsToolStripMenuItem
            // 
            this.exportRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportRecordsToolStripMenuItem.Image")));
            this.exportRecordsToolStripMenuItem.Name = "exportRecordsToolStripMenuItem";
            this.exportRecordsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportRecordsToolStripMenuItem.Text = "Export Records";
            this.exportRecordsToolStripMenuItem.Click += new System.EventHandler(this.exportRecordsToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allRecordsToolStripMenuItem,
            this.currentRecordToolStripMenuItem,
            this.selectedRecordsToolStripMenuItem});
            this.printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.printToolStripMenuItem.Text = "Print";
            // 
            // allRecordsToolStripMenuItem
            // 
            this.allRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("allRecordsToolStripMenuItem.Image")));
            this.allRecordsToolStripMenuItem.Name = "allRecordsToolStripMenuItem";
            this.allRecordsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.allRecordsToolStripMenuItem.Text = "All Records";
            // 
            // currentRecordToolStripMenuItem
            // 
            this.currentRecordToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("currentRecordToolStripMenuItem.Image")));
            this.currentRecordToolStripMenuItem.Name = "currentRecordToolStripMenuItem";
            this.currentRecordToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.currentRecordToolStripMenuItem.Text = "Current Record";
            // 
            // selectedRecordsToolStripMenuItem
            // 
            this.selectedRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectedRecordsToolStripMenuItem.Image")));
            this.selectedRecordsToolStripMenuItem.Name = "selectedRecordsToolStripMenuItem";
            this.selectedRecordsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.selectedRecordsToolStripMenuItem.Text = "Selected Records";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findAndReplaceToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordListAtTopToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // recordListAtTopToolStripMenuItem
            // 
            this.recordListAtTopToolStripMenuItem.Checked = true;
            this.recordListAtTopToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.recordListAtTopToolStripMenuItem.Name = "recordListAtTopToolStripMenuItem";
            this.recordListAtTopToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.recordListAtTopToolStripMenuItem.Text = "Record List at Top";
            this.recordListAtTopToolStripMenuItem.Click += new System.EventHandler(this.recordListAtTopToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearDatabaseToolStripMenuItem,
            this.resetDatabaseToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "Database";
            // 
            // clearDatabaseToolStripMenuItem
            // 
            this.clearDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearDatabaseToolStripMenuItem.Image")));
            this.clearDatabaseToolStripMenuItem.Name = "clearDatabaseToolStripMenuItem";
            this.clearDatabaseToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.clearDatabaseToolStripMenuItem.Text = "Clear Database";
            this.clearDatabaseToolStripMenuItem.Click += new System.EventHandler(this.clearDatabaseToolStripMenuItem_Click);
            // 
            // resetDatabaseToolStripMenuItem
            // 
            this.resetDatabaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetDatabaseToolStripMenuItem.Image")));
            this.resetDatabaseToolStripMenuItem.Name = "resetDatabaseToolStripMenuItem";
            this.resetDatabaseToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.resetDatabaseToolStripMenuItem.Text = "Reset Database";
            this.resetDatabaseToolStripMenuItem.Click += new System.EventHandler(this.resetDatabaseToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // exportingBackgroundWorker
            // 
            this.exportingBackgroundWorker.WorkerReportsProgress = true;
            this.exportingBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.exportingBackgroundWorker_DoWork);
            this.exportingBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.exportingBackgroundWorker_ProgressChanged);
            this.exportingBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.exportingBackgroundWorker_RunWorkerCompleted);
            // 
            // findAndReplaceToolStripMenuItem
            // 
            this.findAndReplaceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("findAndReplaceToolStripMenuItem.Image")));
            this.findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            this.findAndReplaceToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.findAndReplaceToolStripMenuItem.Text = "Find and Replace";
            this.findAndReplaceToolStripMenuItem.Click += new System.EventHandler(this.findAndReplaceToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 637);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1000, 676);
            this.Name = "MainForm";
            this.Text = "C# MARC Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recordsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.marcDataSet)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subfieldsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox previewTextBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.ComponentModel.BackgroundWorker importingBackgroundWorker;
        private System.Windows.Forms.DataGridView recordsDataGridView;
        private System.Windows.Forms.DataGridView subfieldsDataGridView;
        private System.Windows.Forms.DataGridView fieldsDataGridView;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordListAtTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private MARCDataSet marcDataSet;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel progressToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel helptextToolStripStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn recordIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateAddedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateChangedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn titleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn barcodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn classificationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mainEntryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn subfieldIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn recordIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn tagNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ind1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ind2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn controlDataDataGridViewTextBoxColumn;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.ComponentModel.BackgroundWorker exportingBackgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceToolStripMenuItem;

    }
}

