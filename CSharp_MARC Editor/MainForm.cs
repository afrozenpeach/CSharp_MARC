using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MARC;
using System.Data.SQLite;

namespace CSharp_MARC_Editor
{
    public partial class MainForm : Form
    {
        private FileMARCReader marcRecords;
        private string connectionString = "Data Source=MARC.db;Version=3";

        string reloadingDB = "Reloading Database...";
        string committingTransaction = "Committing Transaction...";
        bool startEdit = false;
        bool loading = true;
        bool reloadFields = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #region Utilities

        /// <summary>
        /// Gets the marc record row for inserting into the Record List Data Table
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        private DataRow GetMARCRecordRow(Record record)
        {
            DataRow newRow = marcDataSet.Tables["Records"].NewRow();

            DataField record100 = (DataField)record["100"];
            DataField record245 = (DataField)record["245"];
            string author = "";
            string title = "";

            if (record100 != null && record100['a'] != null)
                author = record100['a'].Data;
            else if (record245 != null && record245['c'] != null)
                author += " " + record245['c'].Data;

            if (record245 != null && record245['a'] != null)
                title = record245['a'].Data;
            else
                title = string.Empty;

            if (record245 != null && record245['b'] != null)
                title += " " + record245['b'].Data;

            newRow["DateAdded"] = new DateTime();
            newRow["DateChanged"] = DBNull.Value;
            newRow["Author"] = author;
            newRow["Title"] = title;

            return newRow;
        }

        /// <summary>
        /// Loads the field.
        /// </summary>
        /// <param name="RecordID">The record identifier.</param>
        private void LoadFields(int RecordID)
        {
            marcDataSet.Tables["Fields"].Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Fields where RecordiD = @RecordID";
                
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add("@RecordID", DbType.Int32).Value = RecordID;
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                    dataAdapter.Fill(marcDataSet, "Fields");
                    fieldsDataGridView.DataSource = marcDataSet.Tables["Fields"];
                }

                foreach (DataGridViewRow row in fieldsDataGridView.Rows)
                {
                    if (!row.IsNewRow && row.Cells[2].Value.ToString().StartsWith("00"))
                    {
                        row.Cells[3].Value = "-";
                        row.Cells[4].Value = "-";
                    }
                }
            }

            if (fieldsDataGridView.Rows.Count > 0)
            {
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(0, 0);
                fieldsDataGridView_CellClick(this, args);
            }

            splitContainer.Panel2.Enabled = true;
        }

        /// <summary>
        /// Loads the subfield.
        /// </summary>
        /// <param name="FieldID">The field identifier.</param>
        private void LoadSubfields(int FieldID)
        {
            marcDataSet.Tables["Subfields"].Rows.Clear();
            codeDataGridViewTextBoxColumn.Visible = true;
            subfieldsDataGridView.AllowUserToAddRows = true;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Subfields where FieldID = @FieldID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add("@FieldID", DbType.Int32).Value = FieldID;
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                    dataAdapter.Fill(marcDataSet, "Subfields");
                    subfieldsDataGridView.DataSource = marcDataSet.Tables["Subfields"];
                }
            }
        }

        private void LoadPreview(int RecordID)
        {
            
        }

        /// <summary>
        /// Loads the control field.
        /// </summary>
        /// <param name="FieldID">The field identifier.</param>
        /// <param name="data">The data.</param>
        private void LoadControlField(int FieldID, string data)
        {
            marcDataSet.Tables["Subfields"].Rows.Clear();
            codeDataGridViewTextBoxColumn.Visible = false;
            subfieldsDataGridView.AllowUserToAddRows = false;
            DataRow newRow = marcDataSet.Tables["Subfields"].NewRow();
            newRow["FieldID"] = FieldID;
            newRow["Code"] = "";
            newRow["Data"] = data;
            marcDataSet.Tables["Subfields"].Rows.Add(newRow);
        }

        #endregion

        #region Form Events

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                loading = true;
                //MessageBox.Show((Convert.ToDateTime("4/19/2016 7:09:06 PM") - Convert.ToDateTime("4/19/2016 6:13:04 PM")).TotalSeconds.ToString());
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Records", connection))
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                        dataAdapter.Fill(marcDataSet, "Records");
                        recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
                    }

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Fields WHERE 1 = 0", connection))
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                        dataAdapter.Fill(marcDataSet, "Fields");
                        fieldsDataGridView.DataSource = marcDataSet.Tables["Fields"];
                    }

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Subfields WHERE 1 = 0", connection))
                    {
                        SQLiteDataAdapter recordsDataAdapter = new SQLiteDataAdapter(command);
                        recordsDataAdapter.Fill(marcDataSet, "Subfields");
                        subfieldsDataGridView.DataSource = marcDataSet.Tables["Subfields"];
                    }
                }

                if (recordsDataGridView.Rows.Count > 0)
                {
                    DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(0, 0);
                    recordsDataGridView_CellClick(this, args);
                }

                loading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading database. " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the CellClick event of the recordsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void recordsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rowClicked = recordsDataGridView.Rows[e.RowIndex];
                if (!rowClicked.IsNewRow)
                    LoadFields(Int32.Parse(rowClicked.Cells[0].Value.ToString()));
                else
                    fieldsDataGridView.Rows.Clear();
            }
        }

        /// <summary>
        /// Handles the CellClick event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rowClicked = fieldsDataGridView.Rows[e.RowIndex];
                if (!rowClicked.IsNewRow)
                {
                    if (rowClicked.Cells[2].Value.ToString().StartsWith("00"))
                        LoadControlField(Int32.Parse(rowClicked.Cells[0].Value.ToString()), rowClicked.Cells[5].Value.ToString());
                    else
                        LoadSubfields(Int32.Parse(rowClicked.Cells[0].Value.ToString()));
                }
                else
                    subfieldsDataGridView.Rows.Clear();
            }
        }

        /// <summary>
        /// Handles the Click event of the openToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Enabled = false;
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar.MarqueeAnimationSpeed = 30;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                recordsDataGridView.SuspendLayout();
                recordsDataGridView.DataSource = null;
                importingBackgroundWorker.RunWorkerAsync(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the loadingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void importingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            marcRecords = new FileMARCReader(e.Argument.ToString());

            int i = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "BEGIN";
                    command.ExecuteNonQuery();

                    foreach (Record record in marcRecords)
                    {
                        i++;
                        DataRow newRow = GetMARCRecordRow(record);
                        
                        command.CommandText = "INSERT INTO Records (DateAdded, DateChanged, Author, Title, Barcode, Classification, MainEntry) VALUES (@DateAdded, @DateChanged, @Author, @Title, @Barcode, @Classification, @MainEntry)";
                        command.Parameters.Add("@DateAdded", DbType.DateTime).Value = DateTime.Now;
                        command.Parameters.Add("@DateChanged", DbType.DateTime).Value = DBNull.Value;
                        command.Parameters.Add("@Author", DbType.String).Value = newRow["Author"];
                        command.Parameters.Add("@Title", DbType.String).Value = newRow["Title"];
                        command.Parameters.Add("@Barcode", DbType.String).Value = newRow["Barcode"];
                        command.Parameters.Add("@Classification", DbType.String).Value = newRow["Classification"];
                        command.Parameters.Add("@MainEntry", DbType.String).Value = newRow["MainEntry"];

                        command.ExecuteNonQuery();
                        
                        int recordID = (int)connection.LastInsertRowId;

                        foreach (Field field in record.Fields)
                        {
                            command.CommandText = "INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, ControlData) VALUES (@RecordID, @TagNumber, @Ind1, @Ind2, @ControlData)";
                            command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                            command.Parameters.Add("@TagNumber", DbType.String).Value = field.Tag;
                            if (field.IsDataField())
                            {
                                command.Parameters.Add("@Ind1", DbType.String).Value = ((DataField)field).Indicator1;
                                command.Parameters.Add("@Ind2", DbType.String).Value = ((DataField)field).Indicator2;
                                command.Parameters.Add("@ControlData", DbType.String).Value = DBNull.Value;
                                
                                command.ExecuteNonQuery();
                                
                                int fieldID = (int)connection.LastInsertRowId;

                                foreach (Subfield subfield in ((DataField)field).Subfields)
                                {
                                    command.CommandText = "INSERT INTO Subfields (FieldID, Code, Data) VALUES (@FieldID, @Code, @Data)";
                                    command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                                    command.Parameters.Add("@Code", DbType.String).Value = subfield.Code;
                                    command.Parameters.Add("@Data", DbType.String).Value = subfield.Data;
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                command.Parameters.Add("@Ind1", DbType.String).Value = DBNull.Value;
                                command.Parameters.Add("@Ind2", DbType.String).Value = DBNull.Value;
                                command.Parameters.Add("@ControlData", DbType.String).Value = ((ControlField)field).Data;
                                command.ExecuteNonQuery();
                            }
                        }
                        command.Parameters.Clear();

                        importingBackgroundWorker.ReportProgress(i);
                    }

                    i = -2;
                    importingBackgroundWorker.ReportProgress(i);

                    command.CommandText = "END";
                    command.ExecuteNonQuery();
                }

                i = -1;
                importingBackgroundWorker.ReportProgress(i);
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the loadingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void importingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case -1:
                    progressToolStripStatusLabel.Text = reloadingDB;
                    break;
                case -2:
                    progressToolStripStatusLabel.Text = committingTransaction;
                    break;
                default:
                    progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString();
                    break;
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the loadingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void importingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            marcDataSet.Tables["Records"].Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Records", connection))
                {
                    SQLiteDataAdapter recordsDataAdapter = new SQLiteDataAdapter(command);
                    recordsDataAdapter.Fill(marcDataSet, "Records");
                    SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(recordsDataAdapter);
                    recordsDataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                    recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
                }
            }

            if (recordsDataGridView.Rows.Count > 0)
            {
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(0, 0);
                recordsDataGridView_CellClick(this, args);
            }

            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
            recordsDataGridView.ResumeLayout();
            loading = false;
            this.Enabled = true;
        }

        /// <summary>
        /// Handles the Click event of the exportRecordsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exportRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Enabled = false;
                toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                exportingBackgroundWorker.RunWorkerAsync(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the exportingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void exportingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY FieldID", new SQLiteConnection(connectionString)))
            {
                fieldsCommand.Connection.Open();
                fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);

                using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY SubfieldID", new SQLiteConnection(connectionString)))
                {
                    subfieldsCommand.Connection.Open();
                    subfieldsCommand.Parameters.Add("@FieldID", DbType.Int32);

                    int i = 0;
                    int max = marcDataSet.Tables["Records"].Rows.Count;
                    FileMARCWriter marcWriter = new FileMARCWriter(saveFileDialog.FileName);

                    foreach (DataGridViewRow row in recordsDataGridView.Rows)
                    {
                        Record record = new Record();
                        fieldsCommand.Parameters["@RecordID"].Value = row.Cells[0].Value;

                        using (SQLiteDataReader fieldsReader = fieldsCommand.ExecuteReader())
                        {
                            while (fieldsReader.Read())
                            {
                                if (fieldsReader["TagNumber"].ToString().StartsWith("00"))
                                {
                                    ControlField controlField = new ControlField(fieldsReader["TagNumber"].ToString(), fieldsReader["ControlData"].ToString());
                                    record.InsertField(controlField);
                                }
                                else
                                {
                                    DataField dataField = new DataField(fieldsReader["TagNumber"].ToString(), new List<Subfield>(), fieldsReader["Ind1"].ToString()[0], fieldsReader["Ind2"].ToString()[0]);
                                    subfieldsCommand.Parameters["@FieldID"].Value = fieldsReader["FieldID"];

                                    using (SQLiteDataReader subfieldReader = subfieldsCommand.ExecuteReader())
                                    {
                                        while (subfieldReader.Read())
                                        {
                                            dataField.InsertSubfield(new Subfield(subfieldReader["Code"].ToString()[0], subfieldReader["Data"].ToString()));
                                        }
                                    }

                                    record.InsertField(dataField);
                                }
                            }
                        }

                        marcWriter.Write(record);
                        exportingBackgroundWorker.ReportProgress(i / max);
                    }

                    marcWriter.WriteEnd();
                    marcWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the exportingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void exportingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString();
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the exportingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void exportingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            loading = false;
            this.Enabled = true;

        }

        /// <summary>
        /// Handles the CellValidating event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellValidatingEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (!fieldsDataGridView.Rows[e.RowIndex].IsNewRow && startEdit)
                {
                    string query = "UPDATE fields SET ";
                    switch (e.ColumnIndex)
                    {
                        case 2:
                            if (fieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().StartsWith("00") && !e.FormattedValue.ToString().StartsWith("00"))
                                throw new Exception("Cannot change a control field to a data field.");
                            else if (!fieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().StartsWith("00") && e.FormattedValue.ToString().StartsWith("00"))
                                throw new Exception("Cannot change a data field to a control field.");
                            else if (Field.ValidateTag(e.FormattedValue.ToString()))
                                query += "tagnumber = @Value ";
                            else
                                throw new Exception("Invalid tag number.");
                            break;
                        case 3:
                            if (e.FormattedValue.ToString().Length == 1 && DataField.ValidateIndicator(e.FormattedValue.ToString()[0]))
                                query += "ind1 = @Value ";
                            else
                                throw new Exception("Invalid indicator.");
                            break;
                        case 4:
                            if (e.FormattedValue.ToString().Length == 1 && DataField.ValidateIndicator(e.FormattedValue.ToString()[0]))
                                query += "ind2 = @Value ";
                            else
                                throw new Exception("Invalid indicator.");
                            break;
                        case 5:
                            query += "controldata = @Value ";
                            break;
                        default:
                            e.Cancel = true;
                            fieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Error 001 - This should never happen. ColumnIndex: " + e.ColumnIndex;
                            return;
                    }

                    query += "WHERE FieldID = @FieldID";

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.Add("@Value", DbType.String).Value = e.FormattedValue;
                            command.Parameters.Add("@FieldID", DbType.String).Value = fieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                errorProvider.SetError((Control)fieldsDataGridView.EditingControl, ex.Message);
                errorProvider.SetIconAlignment((Control)fieldsDataGridView.EditingControl, ErrorIconAlignment.MiddleRight);
                errorProvider.SetIconPadding((Control)fieldsDataGridView.EditingControl, -20);
            }
        }

        /// <summary>
        /// Handles the CellValidating event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellValidatingEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (!subfieldsDataGridView.Rows[e.RowIndex].IsNewRow && startEdit)
                {
                    if (subfieldsDataGridView.Rows[e.RowIndex].Cells[2].Visible)
                    {
                        string query = "UPDATE subfields SET ";

                        switch (e.ColumnIndex)
                        {
                            case 2:
                                if (e.FormattedValue.ToString().Length == 1 && DataField.ValidateIndicator(e.FormattedValue.ToString()[0]))
                                {
                                    query += "code = @Value ";
                                }
                                else
                                    throw new Exception("Invalid subfield code.");
                                break;
                            case 3:
                                query += "data = @Value ";
                                break;
                            default:
                                throw new Exception("Error 002 - This should never happen. ColumnIndex: " + e.ColumnIndex);
                        }

                        query += "WHERE SubfieldID = @SubfieldID";

                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            using (SQLiteCommand command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.Add("@Value", DbType.String).Value = e.FormattedValue;
                                command.Parameters.Add("@SubfieldID", DbType.String).Value = subfieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else //It's a control field -> we need to update the field row instead
                    {
                        string query = "UPDATE fields SET ";

                        switch (e.ColumnIndex)
                        {
                            case 3:
                                query += "controldata = @Value ";
                                break;
                            default:
                                throw new Exception("Error 003 - This should never happen. ColumnIndex: " + e.ColumnIndex);
                        }

                        query += "WHERE FieldID = @FieldID";

                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            using (SQLiteCommand command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.Add("@Value", DbType.String).Value = e.FormattedValue;
                                command.Parameters.Add("@FieldID", DbType.String).Value = subfieldsDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

                                command.ExecuteNonQuery();
                                reloadFields = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                errorProvider.SetError((Control)subfieldsDataGridView.EditingControl, ex.Message);
                errorProvider.SetIconAlignment((Control)subfieldsDataGridView.EditingControl, ErrorIconAlignment.MiddleRight);
                errorProvider.SetIconPadding((Control)subfieldsDataGridView.EditingControl, -20);
            }
        }

        /// <summary>
        /// Handles the CellBeginEdit event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellCancelEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!loading)
            {
                startEdit = true;

                switch (e.ColumnIndex)
                {
                    case 2:
                        break;
                    case 3:
                    case 4:
                        if (fieldsDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString().StartsWith("00"))
                        {
                            MessageBox.Show("Cannot edit indicators on control fields.");
                            startEdit = false;
                            e.Cancel = true;
                        }
                        break;
                    default:
                        throw new Exception("Error 004 - This should never happen. Column: " + e.ColumnIndex);
                }
            }
        }

        /// <summary>
        /// Handles the CellBeginEdit event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellCancelEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!loading)
                startEdit = true;
        }

        /// <summary>
        /// Handles the CellValidated event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            startEdit = false;
            fieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }

        /// <summary>
        /// Handles the CellValidated event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (reloadFields && !subfieldsDataGridView.Rows[e.RowIndex].Cells[2].Visible)
            {
                reloadFields = false;
                recordsDataGridView_CellClick(sender, new DataGridViewCellEventArgs(recordsDataGridView.SelectedCells[0].ColumnIndex, recordsDataGridView.SelectedCells[0].RowIndex));
            }

            startEdit = false;
            subfieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }

        /// <summary>
        /// Handles the CellEndEdit event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            startEdit = false;
            fieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }

        /// <summary>
        /// Handles the CellEndEdit event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            startEdit = false;
            subfieldsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
        }

        /// <summary>
        /// Handles the Click event of the recordListAtTopToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void recordListAtTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (recordListAtTopToolStripMenuItem.Checked)
            {
                recordListAtTopToolStripMenuItem.Checked = false;
                splitContainer.Orientation = Orientation.Vertical;
            }
            else
            {
                recordListAtTopToolStripMenuItem.Checked = true;
                splitContainer.Orientation = Orientation.Horizontal;
            }
        }

        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
