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
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing.Printing;

namespace CSharp_MARC_Editor
{
    public partial class MainForm : Form
    {
        #region Private member variables

        private FileMARCReader marcRecords;
        public static string connectionString = "Data Source=MARC.db;Version=3";

        private string reloadingDB = "Reloading Database...";
        private string committingTransaction = "Committing Transaction...";
        private bool startEdit = false;
        private bool loading = true;
        private bool reloadFields = false;
        private decimal recordsPerFile = 0;
        private CustomFieldsForm customFieldsForm = new CustomFieldsForm();
        private List<string> linesToPrint;

        private const char START_OF_HEADING = '\x01';
        private const char NEW_PAGE = '\xFF';
        private const char END_OF_FILE = '\x1A';

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #region Utilities

        /// <summary>
        /// Loads the field.
        /// </summary>
        /// <param name="recordID">The record identifier.</param>
        private void LoadFields(int recordID)
        {
            marcDataSet.Tables["Fields"].Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Fields where RecordiD = @RecordID";
                
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
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

            LoadPreview(recordID);
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

        /// <summary>
        /// Loads the preview.
        /// </summary>
        /// <param name="recordID">The record identifier.</param>
        private void LoadPreview(int recordID)
        {
            Record record = new Record();

            using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY FieldID", new SQLiteConnection(connectionString)))
            {
                fieldsCommand.Connection.Open();
                fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);

                using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY SubfieldID", new SQLiteConnection(connectionString)))
                {
                    subfieldsCommand.Connection.Open();
                    subfieldsCommand.Parameters.Add("@FieldID", DbType.Int32);
                    fieldsCommand.Parameters["@RecordID"].Value = recordID;

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
                                char ind1 = ' ';
                                char ind2 = ' ';

                                if (fieldsReader["Ind1"].ToString().Length > 0)
                                    ind1 = fieldsReader["Ind1"].ToString()[0];

                                if (fieldsReader["Ind2"].ToString().Length > 0)
                                    ind2 = fieldsReader["Ind2"].ToString()[0];

                                DataField dataField = new DataField(fieldsReader["TagNumber"].ToString(), new List<Subfield>(), ind1, ind2);
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
                }
            }

            previewTextBox.Text = record.ToString();
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
            newRow["SubfieldID"] = -1;
            newRow["FieldID"] = FieldID;
            newRow["Code"] = "";
            newRow["Data"] = data;
            marcDataSet.Tables["Subfields"].Rows.Add(newRow);
        }

        /// <summary>
        /// Resets the database.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        private void ResetDatabase(bool force = false)
        {
            if (force || MessageBox.Show("This will permanently delete all records and reset the customizable options to their defaults." + Environment.NewLine + Environment.NewLine + "Are you sure you want to reset the database?", "Are you sure you want to reset the database?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Enabled = false;

                GC.Collect();
                GC.WaitForFullGCComplete();

                if (File.Exists("MARC.db"))
                    File.Delete("MARC.db");

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"CREATE TABLE [Fields](
                                                    [FieldID] integer PRIMARY KEY ASC AUTOINCREMENT NOT NULL, 
                                                    [RecordID] nvarchar(2147483647) NOT NULL, 
                                                    [TagNumber] nvarchar(2147483647) NOT NULL, 
                                                    [Ind1] char, 
                                                    [Ind2] char, 
                                                    [ControlData] nvarchar(2147483647), 
                                                    FOREIGN KEY([RecordID]) REFERENCES Records([RecordID]) ON DELETE CASCADE ON UPDATE RESTRICT);

                                                CREATE TABLE [Records](
                                                    [RecordID] integer PRIMARY KEY ASC AUTOINCREMENT NOT NULL, 
                                                    [DateAdded] datetime NOT NULL, 
                                                    [DateChanged] datetime, 
                                                    [Author] nvarchar(2147483647), 
                                                    [Title] nvarchar(2147483647), 
                                                    [CopyrightDate] integer, 
                                                    [Barcode] nvarchar(2147483647), 
                                                    [Classification] nvarchar(2147483647), 
                                                    [MainEntry] nvarchar(2147483647), 
                                                    [Custom1] nvarchar(2147483647), 
                                                    [Custom2] nvarchar(2147483647), 
                                                    [Custom3] nvarchar(2147483647), 
                                                    [Custom4] nvarchar(2147483647), 
                                                    [Custom5] nvarchar(2147483647), 
                                                    [ImportErrors] nvarchar(2147483647));

                                                CREATE TABLE [Settings](
                                                    [RecordListAtTop] bool, 
                                                    [ClearDatabaseOnExit] bool, 
                                                    [ExportFormat] char(1), 
                                                    [CustomTag1] nvarchar(3), 
                                                    [CustomCode1] nvarchar(1), 
                                                    [CustomData1] nvarchar(2147483647), 
                                                    [CustomTag2] nvarchar(3), 
                                                    [CustomCode2] nvarchar(1), 
                                                    [CustomData2] nvarchar(2147483647), 
                                                    [CustomTag3] nvarchar(3), 
                                                    [CustomCode3] nvarchar(1), 
                                                    [CustomData3] nvarchar(2147483647), 
                                                    [CustomTag4] nvarchar(3), 
                                                    [CustomCode4] nvarchar(1), 
                                                    [CustomData4] nvarchar(2147483647), 
                                                    [CustomTag5] nvarchar(3), 
                                                    [CustomCode5] varchar(1), 
                                                    [CustomData5] nvarchar(2147483647));

                                                CREATE TABLE [Subfields](
                                                    [SubfieldID] integer PRIMARY KEY ASC AUTOINCREMENT NOT NULL, 
                                                    [FieldID] bigint NOT NULL, 
                                                    [Code] char NOT NULL, 
                                                    [Data] nvarchar(2147483647) NOT NULL, 
                                                    FOREIGN KEY([FieldID]) REFERENCES Fields([FieldID]) ON DELETE CASCADE ON UPDATE RESTRICT);

                                                CREATE INDEX [FieldID]
                                                ON [Subfields](
                                                    [FieldID] ASC);

                                                CREATE INDEX [RecordID]
                                                ON [Fields](
                                                    [RecordID] ASC);";

                        command.ExecuteNonQuery();
                    }
                }

                this.OnLoad(new EventArgs());
                this.Enabled = true;
            }
        }

        /// <summary>
        /// Rebuilds the records preview information.
        /// This consists of the Author, Title, Barcode, Classification, and MainEntry fields
        /// </summary>
        private void RebuildRecordsPreviewInformation(int? recordID = null)
        {
            using (SQLiteConnection readerConnection = new SQLiteConnection(connectionString))
            {
                readerConnection.Open();

                using (SQLiteCommand readerCommand = new SQLiteCommand(readerConnection))
                {
                    StringBuilder query = new StringBuilder("SELECT r.RecordID as RecordID, TagNumber, Code, Data, Author, Title, Barcode, Classification, MainEntry FROM Records r LEFT OUTER JOIN Fields f ON r.RecordID = f.RecordID LEFT OUTER JOIN Subfields s ON f.FieldID = s.FieldID");

                    if (recordID.HasValue)
                    {
                        query.Append(" WHERE r.RecordID = @RecordID");
                        readerCommand.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                    }

                    query.Append(" UNION SELECT '-2' as RecordID, '', '', '', '', '', '', '', ''");
                    query.Append(" ORDER BY RecordID, TagNumber, Code");

                    readerCommand.CommandText = query.ToString();

                    using (SQLiteConnection updaterConnection = new SQLiteConnection(connectionString))
                    {
                        updaterConnection.Open();

                        using (SQLiteCommand updaterCommand = new SQLiteCommand(updaterConnection))
                        {
                            updaterCommand.CommandText = "BEGIN;";
                            updaterCommand.ExecuteNonQuery();

                            updaterCommand.CommandText = "UPDATE Records SET DateChanged = @DateChanged, Author = @Author, Title = @Title, CopyrightDate = @CopyrightDate, Barcode = @Barcode, Classification = @Classification, MainEntry = @MainEntry, Custom1 = @Custom1, Custom2 = @Custom2, Custom3 = @Custom3, Custom4 = @Custom4, Custom5 = @Custom5 WHERE RecordID = @RecordID";
                            
                            updaterCommand.Parameters.Add("@Author", DbType.String);
                            updaterCommand.Parameters.Add("@Title", DbType.String);
                            updaterCommand.Parameters.Add("@Barcode", DbType.String);
                            updaterCommand.Parameters.Add("@CopyrightDate", DbType.Int32);
                            updaterCommand.Parameters.Add("@Classification", DbType.String);
                            updaterCommand.Parameters.Add("@MainEntry", DbType.String);
                            updaterCommand.Parameters.Add("@RecordID", DbType.Int32);
                            updaterCommand.Parameters.Add("@DateChanged", DbType.DateTime);
                            updaterCommand.Parameters.Add("@Custom1", DbType.String);
                            updaterCommand.Parameters.Add("@Custom2", DbType.String);
                            updaterCommand.Parameters.Add("@Custom3", DbType.String);
                            updaterCommand.Parameters.Add("@Custom4", DbType.String);
                            updaterCommand.Parameters.Add("@Custom5", DbType.String);

                            using (SQLiteDataReader reader = readerCommand.ExecuteReader())
                            {
                                int currentRecord = -1;

                                string author = null;
                                string title = null;
                                string barcode = null;
                                string classification = null;
                                string mainEntry = null;
                                string custom1 = null;
                                string custom2 = null;
                                string custom3 = null;
                                string custom4 = null;
                                string custom5 = null;
                                int tempCopyrightDate = -1;
                                int? copyrightDate = null;
                                bool bc = false;

                                while (reader.Read())
                                {
                                    if (currentRecord != Int32.Parse(reader["RecordID"].ToString()))
                                    {
                                        if (currentRecord >= 0)
                                        {
                                            updaterCommand.Parameters["@DateChanged"].Value = DateTime.Now;
                                            updaterCommand.Parameters["@Author"].Value = author;
                                            updaterCommand.Parameters["@Title"].Value = title;
                                            if (copyrightDate.HasValue)
                                                updaterCommand.Parameters["@CopyrightDate"].Value = copyrightDate;
                                            else
                                                updaterCommand.Parameters["@CopyrightDate"].Value = DBNull.Value;
                                            updaterCommand.Parameters["@Barcode"].Value = barcode;
                                            updaterCommand.Parameters["@Classification"].Value = classification;
                                            updaterCommand.Parameters["@MainEntry"].Value = mainEntry;
                                            updaterCommand.Parameters["@RecordID"].Value = currentRecord;
                                            updaterCommand.Parameters["@Custom1"].Value = custom1;
                                            updaterCommand.Parameters["@Custom2"].Value = custom2;
                                            updaterCommand.Parameters["@Custom3"].Value = custom3;
                                            updaterCommand.Parameters["@Custom4"].Value = custom4;
                                            updaterCommand.Parameters["@Custom5"].Value = custom5;

                                            updaterCommand.ExecuteNonQuery();

                                            if (recordID != null)
                                            {
                                                foreach (DataGridViewRow row in recordsDataGridView.Rows)
                                                {
                                                    if (Int32.Parse(row.Cells[0].Value.ToString()) == recordID)
                                                    {
                                                        if (barcode == null)
                                                            barcode = "";
                                                        
                                                        if (classification == null)
                                                            classification = "";
                                                        
                                                        if (mainEntry == null)
                                                            mainEntry = "";

                                                        row.Cells[2].Value = updaterCommand.Parameters["@DateChanged"].Value.ToString();
                                                        row.Cells[3].Value = author;
                                                        row.Cells[4].Value = title;
                                                        row.Cells[5].Value = copyrightDate;
                                                        row.Cells[6].Value = barcode;
                                                        row.Cells[7].Value = classification;
                                                        row.Cells[8].Value = mainEntry;
                                                        row.Cells[9].Value = custom1;
                                                        row.Cells[10].Value = custom2;
                                                        row.Cells[11].Value = custom3;
                                                        row.Cells[12].Value = custom4;
                                                        row.Cells[13].Value = custom5;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        currentRecord = Int32.Parse(reader["RecordID"].ToString());

                                        author = null;
                                        title = null;
                                        barcode = null;
                                        classification = null;
                                        mainEntry = null;
                                        custom1 = null;
                                        custom2 = null;
                                        custom3 = null;
                                        custom4 = null;
                                        custom5 = null;
                                    }

                                    if (author == null && (string)reader["TagNumber"] == "100" && (string)reader["Code"] == "a")
                                        author = (string)reader["Data"];
                                    else if (author == null && (string)reader["TagNumber"] == "245" && (string)reader["Code"] == "c")
                                        author = (string)reader["Data"];

                                    if (title == null && (string)reader["TagNumber"] == "245" && (string)reader["Code"] == "a")
                                        title = (string)reader["Data"];
                                    else if (title == null && (string)reader["TagNumber"] == "245" && (string)reader["Code"] == "b")
                                        title += " " + (string)reader["Data"];

                                    if (copyrightDate == null && (string)reader["TagNumber"] == "260" && (string)reader["Code"] == "c" && int.TryParse(Regex.Replace((string)reader["Data"], "[^0-9]", ""), out tempCopyrightDate))
                                        copyrightDate = tempCopyrightDate;
                                    else if (copyrightDate == null && (string)reader["TagNumber"] == "264" && (string)reader["Code"] == "c" && int.TryParse(Regex.Replace((string)reader["Data"], "[^0-9]", ""), out tempCopyrightDate))
                                        copyrightDate = tempCopyrightDate;

                                    if (barcode == null && (string)reader["TagNumber"] == "852" && (string)reader["Code"] == "p")
                                        barcode = (string)reader["Data"];
                                    else if (barcode == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "i")
                                        barcode = (string)reader["Data"];
                                    else if (barcode == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "g")
                                        barcode = (string)reader["Data"];
                                    else if (barcode == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "b")
                                        barcode = (string)reader["Data"];

                                    if (classification == null && (string)reader["TagNumber"] == "852" && (string)reader["Code"] == "h")
                                    {
                                        string[] split = ((string)reader["Data"]).Split(' ');
                                        classification = split[0];

                                        if (split.Length > 1)
                                            mainEntry = split[1];
                                    }
                                    else if (classification == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "a")
                                    {
                                        string[] split = ((string)reader["Data"]).Split(' ');
                                        if (split.Length > 2)
                                        {
                                            classification = split[0] + " " + split[1];
                                            mainEntry = split[2];
                                        }
                                        else
                                        {
                                            classification = split[0];
                                            mainEntry = split[1];
                                        }
                                    }
                                    else if (classification == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "b")
                                    {
                                        classification = (string)reader["Data"];
                                        bc = true;
                                    }
                                    else if (classification != null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "c" && bc)
                                    {
                                        classification += " " + (string)reader["Data"];
                                        bc = false;
                                    }
                                    else if (classification == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "c")
                                    {
                                        string[] split = ((string)reader["Data"]).Split(' ');
                                        if (split.Length > 2)
                                        {
                                            classification = split[0] + " " + split[1];
                                            mainEntry = split[2];
                                        }
                                        else if (split.Length > 1)
                                        {
                                            classification = split[0];
                                            mainEntry = split[1];
                                        }
                                        else
                                            classification = split[0];
                                    }
                                    else if (classification == null && (string)reader["TagNumber"] == "949" && (string)reader["Code"] == "c")
                                    {
                                        string[] split = ((string)reader["Data"]).Split(' ');
                                        if (split.Length > 2)
                                        {
                                            classification = split[0] + " " + split[1];
                                            mainEntry = split[2];
                                        }
                                        else
                                        {
                                            classification = split[0];
                                            mainEntry = split[1];
                                        }
                                    }

                                    if (custom1 == null && (string)reader["TagNumber"] == customFieldsForm.TagNumber1 && (string)reader["Code"] == customFieldsForm.Code1)
                                    {
                                        if (customFieldsForm.Data1 != "")
                                            custom1 = Regex.Match((string)reader["Data"], customFieldsForm.Data1).Value;
                                        else
                                            custom1 = (string)reader["Data"];
                                    }

                                    if (custom2 == null && (string)reader["TagNumber"] == customFieldsForm.TagNumber2 && (string)reader["Code"] == customFieldsForm.Code2)
                                    {
                                        if (customFieldsForm.Data2 != "")
                                            custom2 = Regex.Match((string)reader["Data"], customFieldsForm.Data2).Value;
                                        else
                                            custom2 = (string)reader["Data"];
                                    }

                                    if (custom3 == null && (string)reader["TagNumber"] == customFieldsForm.TagNumber3 && (string)reader["Code"] == customFieldsForm.Code3)
                                    {
                                        if (customFieldsForm.Data3 != "")
                                            custom3 = Regex.Match((string)reader["Data"], customFieldsForm.Data3).Value;
                                        else
                                            custom3 = (string)reader["Data"];
                                    }

                                    if (custom4 == null && (string)reader["TagNumber"] == customFieldsForm.TagNumber4 && (string)reader["Code"] == customFieldsForm.Code4)
                                    {
                                        if (customFieldsForm.Data4 != "")
                                            custom4 = Regex.Match((string)reader["Data"], customFieldsForm.Data4).Value;
                                        else
                                            custom4 = (string)reader["Data"];
                                    }

                                    if (custom5 == null && (string)reader["TagNumber"] == customFieldsForm.TagNumber5 && (string)reader["Code"] == customFieldsForm.Code5)
                                    {
                                        if (customFieldsForm.Data5 != "")
                                            custom5 = Regex.Match((string)reader["Data"], customFieldsForm.Data5).Value;
                                        else
                                            custom5 = (string)reader["Data"];
                                    }
                                }
                            }
                            
                            updaterCommand.CommandText = "END;";
                            updaterCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the options.
        /// </summary>
        private void SaveOptions()
        {
            if (loading)
                return;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Settings SET RecordListAtTop = @RecordListAtTop, ClearDatabaseOnExit = @ClearDatabaseOnExit, ExportFormat = @ExportFormat, CustomTag1 = @CustomTag1, CustomCode1 = @CustomCode1, CustomData1 = @CustomData1, CustomTag2 = @CustomTag2, CustomCode2 = @CustomCode2, CustomData2 = @CustomData2, CustomTag3 = @CustomTag3, CustomCode3 = @CustomCode3, CustomData3 = @CustomData3, CustomTag4 = @CustomTag4, CustomCode4 = @CustomCode4, CustomData4 = @CustomData4, CustomTag5 = @CustomTag5, CustomCode5 = @CustomCode5, CustomData5 = @CustomData5";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.Add("@RecordListAtTop", DbType.Boolean).Value = recordListAtTopToolStripMenuItem.Checked;
                    command.Parameters.Add("@ClearDatabaseOnExit", DbType.Boolean).Value = clearDatabaseOnExitToolStripMenuItem.Checked;

                    if (mARC8ToolStripMenuItem.Checked)
                        command.Parameters.Add("@ExportFormat", DbType.Boolean).Value = 'M';
                    else if (uTF8ToolStripMenuItem.Checked)
                        command.Parameters.Add("@ExportFormat", DbType.Boolean).Value = 'U';
                    else if (mARCXMLToolStripMenuItem.Checked)
                        command.Parameters.Add("@ExportFormat", DbType.Boolean).Value = 'X';
                    else
                        command.Parameters.Add("@ExportFormat", DbType.Boolean).Value = null;

                    command.Parameters.Add("@CustomTag1", DbType.String).Value = customFieldsForm.TagNumber1;
                    command.Parameters.Add("@CustomCode1", DbType.String).Value = customFieldsForm.Code1;
                    command.Parameters.Add("@CustomData1", DbType.String).Value = customFieldsForm.Data1;
                    command.Parameters.Add("@CustomTag2", DbType.String).Value = customFieldsForm.TagNumber2;
                    command.Parameters.Add("@CustomCode2", DbType.String).Value = customFieldsForm.Code2;
                    command.Parameters.Add("@CustomData2", DbType.String).Value = customFieldsForm.Data2;
                    command.Parameters.Add("@CustomTag3", DbType.String).Value = customFieldsForm.TagNumber3;
                    command.Parameters.Add("@CustomCode3", DbType.String).Value = customFieldsForm.Code3;
                    command.Parameters.Add("@CustomData3", DbType.String).Value = customFieldsForm.Data3;
                    command.Parameters.Add("@CustomTag4", DbType.String).Value = customFieldsForm.TagNumber4;
                    command.Parameters.Add("@CustomCode4", DbType.String).Value = customFieldsForm.Code4;
                    command.Parameters.Add("@CustomData4", DbType.String).Value = customFieldsForm.Data4;
                    command.Parameters.Add("@CustomTag5", DbType.String).Value = customFieldsForm.TagNumber5;
                    command.Parameters.Add("@CustomCode5", DbType.String).Value = customFieldsForm.Code5;
                    command.Parameters.Add("@CustomData5", DbType.String).Value = customFieldsForm.Data5;

                    int changes = command.ExecuteNonQuery();

                    if (changes == 0)
                    {
                        command.CommandText = "INSERT INTO Settings (RecordListAtTop, ClearDatabaseOnExit, ExportFormat, CustomTag1, CustomCode1, CustomData1, CustomTag2, CustomCode2, CustomData2, CustomTag3, CustomCode3, CustomData3, CustomTag4, CustomCode4, CustomData4, CustomTag5, CustomCode5, CustomData5) VALUES (@RecordListAtTop, @ClearDatabaseOnExit, @ExportFormat, @CustomTag1, @CustomCode1, @CustomData1, @CustomTag2, @CustomCode2, @CustomData2, @CustomTag3, @CustomCode3, @CustomData3, @CustomTag4, @CustomCode4, @CustomData4, @CustomTag5, @CustomCode5, @CustomData5)";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
        class REGEXP : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Regex.IsMatch(args[1].ToString(), args[0].ToString());
            }
        }

        [SQLiteFunction(Name = "REGEXREPLACE", Arguments = 3, FuncType = FunctionType.Scalar)]
        class REGEXREPLACE : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Regex.Replace(args[0].ToString(), args[1].ToString(), args[2].ToString());
            }
        }

        [SQLiteFunction(Name = "REPLACENOCASE", Arguments = 3, FuncType = FunctionType.Scalar)]
        class REPLACENOCASE : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Regex.Replace(args[0].ToString(), Regex.Escape(args[1].ToString()), args[2].ToString().Replace("$", "$$"), RegexOptions.IgnoreCase);
            }
        }

        [SQLiteFunction(Name = "REGEXREPLACENOCASE", Arguments = 3, FuncType = FunctionType.Scalar)]
        class REGEXREPLACENOCASE : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Regex.Replace(args[0].ToString(), args[1].ToString(), args[2].ToString().Replace("$", "$$"), RegexOptions.IgnoreCase);
            }
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

                recordsDataGridView.DataSource = null;
                fieldsDataGridView.DataSource = null;
                subfieldsDataGridView.DataSource = null;

                marcDataSet.Tables["Records"].Rows.Clear();
                marcDataSet.Tables["Fields"].Rows.Clear();
                marcDataSet.Tables["Subfields"].Rows.Clear();

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

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Settings LIMIT 1", connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["RecordListAtTop"] != DBNull.Value && !(bool)reader["RecordListAtTop"] && recordListAtTopToolStripMenuItem.Checked)
                                    recordListAtTopToolStripMenuItem_Click(sender, e);
                                else
                                    recordListAtTopToolStripMenuItem.Checked = true;

                                if (reader["ClearDatabaseOnExit"] != DBNull.Value && (bool)reader["ClearDatabaseOnExit"] && !clearDatabaseOnExitToolStripMenuItem.Checked)
                                    clearDatabaseOnExitToolStripMenuItem_Click(sender, e);
                                else
                                    clearDatabaseOnExitToolStripMenuItem.Checked = false;

                                switch (reader["ExportFormat"].ToString())
                                {
                                    case "M":
                                        uTF8ToolStripMenuItem.Checked = true;
                                        mARC8ToolStripMenuItem.Checked = false;
                                        mARCXMLToolStripMenuItem.Checked = false;
                                        break;
                                    case "U":
                                        uTF8ToolStripMenuItem.Checked = false;
                                        mARC8ToolStripMenuItem.Checked = true;
                                        mARCXMLToolStripMenuItem.Checked = false;
                                        break;
                                    case "X":
                                        uTF8ToolStripMenuItem.Checked = false;
                                        mARC8ToolStripMenuItem.Checked = false;
                                        mARCXMLToolStripMenuItem.Checked = true;
                                        break;
                                }

                                customFieldsForm.TagNumber1 = reader["CustomTag1"].ToString();
                                customFieldsForm.Code1 = reader["CustomCode1"].ToString();
                                customFieldsForm.Data1 = reader["CustomData1"].ToString();
                                customFieldsForm.TagNumber2 = reader["CustomTag2"].ToString();
                                customFieldsForm.Code2 = reader["CustomCode2"].ToString();
                                customFieldsForm.Data2 = reader["CustomData2"].ToString();
                                customFieldsForm.TagNumber3 = reader["CustomTag3"].ToString();
                                customFieldsForm.Code3 = reader["CustomCode3"].ToString();
                                customFieldsForm.Data3 = reader["CustomData3"].ToString();
                                customFieldsForm.TagNumber4 = reader["CustomTag4"].ToString();
                                customFieldsForm.Code4 = reader["CustomCode4"].ToString();
                                customFieldsForm.Data4 = reader["CustomData4"].ToString();
                                customFieldsForm.TagNumber5 = reader["CustomTag5"].ToString();
                                customFieldsForm.Code5 = reader["CustomCode5"].ToString();
                                customFieldsForm.Data5 = reader["CustomData5"].ToString();
                            }
                            else
                            {
                                reader.Close();
                                command.CommandText = "INSERT INTO Settings (RecordListAtTop, ClearDatabaseOnExit, ExportFormat, CustomTag1, CustomCode1, CustomData1, CustomTag2, CustomCode2, CustomData2, CustomTag3, CustomCode3, CustomData3, CustomTag4, CustomCode4, CustomData4, CustomTag5, CustomCode5, CustomData5) VALUES (@RecordListAtTop, @ClearDatabaseOnExit, @ExportFormat, @CustomTag1, @CustomCode1, @CustomData1, @CustomTag2, @CustomCode2, @CustomData2, @CustomTag3, @CustomCode3, @CustomData3, @CustomTag4, @CustomCode4, @CustomData4, @CustomTag5, @CustomCode5, @CustomData5)";
                                command.Parameters.Add("@RecordListAtTop", DbType.Boolean).Value = true;
                                command.Parameters.Add("@ClearDatabaseOnExit", DbType.Boolean).Value = false;
                                command.Parameters.Add("@ExportFormat", DbType.Boolean).Value = 'U';
                                command.Parameters.Add("@CustomTag1", DbType.String).Value = customFieldsForm.TagNumber1;
                                command.Parameters.Add("@CustomCode1", DbType.String).Value = customFieldsForm.Code1;
                                command.Parameters.Add("@CustomData1", DbType.String).Value = customFieldsForm.Data1;
                                command.Parameters.Add("@CustomTag2", DbType.String).Value = customFieldsForm.TagNumber2;
                                command.Parameters.Add("@CustomCode2", DbType.String).Value = customFieldsForm.Code2;
                                command.Parameters.Add("@CustomData2", DbType.String).Value = customFieldsForm.Data2;
                                command.Parameters.Add("@CustomTag3", DbType.String).Value = customFieldsForm.TagNumber3;
                                command.Parameters.Add("@CustomCode3", DbType.String).Value = customFieldsForm.Code3;
                                command.Parameters.Add("@CustomData3", DbType.String).Value = customFieldsForm.Data3;
                                command.Parameters.Add("@CustomTag4", DbType.String).Value = customFieldsForm.TagNumber4;
                                command.Parameters.Add("@CustomCode4", DbType.String).Value = customFieldsForm.Code4;
                                command.Parameters.Add("@CustomData4", DbType.String).Value = customFieldsForm.Data4;
                                command.Parameters.Add("@CustomTag5", DbType.String).Value = customFieldsForm.TagNumber5;
                                command.Parameters.Add("@CustomCode5", DbType.String).Value = customFieldsForm.Code5;
                                command.Parameters.Add("@CustomData5", DbType.String).Value = customFieldsForm.Data5;

                                command.ExecuteNonQuery();
                            }
                        }
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
                if (MessageBox.Show("Error loading database. " + ex.Message + Environment.NewLine + Environment.NewLine + "If you continue to see this message, it may be necessary to reset the database. Doing so will permanently delete all records from the database." + Environment.NewLine + Environment.NewLine + "Do you want to reset the database?", "Error loading database.", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    ResetDatabase(true);
                }
                else
                {
                    this.Close();
                }
            }
        }

        #region Loading Records

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
                if (!rowClicked.IsNewRow && rowClicked.Cells[0].Value.ToString() != "")
                {
                    if (rowClicked.Cells[2].Value.ToString().StartsWith("00"))
                        LoadControlField(Int32.Parse(rowClicked.Cells[0].Value.ToString()), rowClicked.Cells[5].Value.ToString());
                    else
                        LoadSubfields(Int32.Parse(rowClicked.Cells[0].Value.ToString()));
                }
                else
                    marcDataSet.Tables["Subfields"].Clear();
            }
        }

        #endregion

        #region Importing Records

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
            IEnumerable recordEnumerator = null;
            List<Record> importedSRU = new List<Record>();

            if (e.Argument.GetType() == typeof(string))
            {
                marcRecords = new FileMARCReader(e.Argument.ToString());
                recordEnumerator = marcRecords;
            }
            else
            {
                importedSRU.Add((Record)e.Argument);
                recordEnumerator = importedSRU;
            }

            int i = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "BEGIN";
                    command.ExecuteNonQuery();

                    foreach (Record record in recordEnumerator)
                    {
                        i++;
                        
                        command.CommandText = "INSERT INTO Records (DateAdded, DateChanged, ImportErrors) VALUES (@DateAdded, @DateChanged, @ImportErrors)";
                        command.Parameters.Add("@DateAdded", DbType.DateTime).Value = DateTime.Now;
                        command.Parameters.Add("@DateChanged", DbType.DateTime).Value = DBNull.Value;
                        
                        string errors = "";
                        foreach (string error in record.Warnings)
                            errors += error + Environment.NewLine;
                        
                        if (errors.Length > 1)
                            errors = errors.Substring(0, errors.Length - 1);

                        command.Parameters.Add("@ImportErrors", DbType.String).Value = errors;

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
            RebuildRecordsPreviewInformation();

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
        /// Handles the Click event of the fromZ3950SRUToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void fromZ3950SRUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportSRU sru = new ImportSRU())
            {
                if (sru.ShowDialog() == DialogResult.OK)
                {
                    Record importedRecord = sru.SelectedRecord;
                    importingBackgroundWorker.RunWorkerAsync(importedRecord);
                }
            }
        }

        #endregion

        #region Exporting Records

        /// <summary>
        /// Handles the Click event of the exportRecordsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exportRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (sender == splitToolStripMenuItem)
                {
                    using (ExportSplitDialog splitDialog = new ExportSplitDialog())
                    {
                        if (splitDialog.ShowDialog() == DialogResult.OK)
                            recordsPerFile = splitDialog.RecordsPerFile;
                        else
                            return;
                    }
                }

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

                    int i = 1;
                    int recordCounter = 1;
                    int fileCounter = 1;
                    FileMARCWriter.RecordEncoding recordEncoding;

                    if (mARC8ToolStripMenuItem.Checked)
                        recordEncoding = FileMARCWriter.RecordEncoding.MARC8;
                    else
                        recordEncoding = FileMARCWriter.RecordEncoding.UTF8;

                    int max = marcDataSet.Tables["Records"].Rows.Count;

                    FileMARCXMLWriter xmlWriter = null;
                    FileMARCWriter marcWriter = null;
                    string fileName = e.Argument.ToString();
                    
                    if (recordsPerFile > 0)
                        fileName = e.Argument.ToString().Substring(0, e.Argument.ToString().LastIndexOf('.')) + "." + fileCounter + "." + e.Argument.ToString().Substring(e.Argument.ToString().LastIndexOf('.') + 1);
                    
                    if (mARCXMLToolStripMenuItem.Checked)
                        xmlWriter = new FileMARCXMLWriter(fileName);
                    else
                        marcWriter = new FileMARCWriter(fileName, recordEncoding);

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

                        if (mARCXMLToolStripMenuItem.Checked)
                            xmlWriter.Write(record);
                        else
                            marcWriter.Write(record);
                        i++;
                        recordCounter++;
                        exportingBackgroundWorker.ReportProgress(i / max);

                        if (recordsPerFile != 0 && recordCounter > recordsPerFile)
                        {
                            recordCounter = 1;
                            fileCounter++;

                            if (mARCXMLToolStripMenuItem.Checked)
                                marcWriter.WriteEnd();

                            if (marcWriter != null)
                                marcWriter.Dispose();

                            if (xmlWriter != null)
                                xmlWriter.Dispose();

                            if (mARCXMLToolStripMenuItem.Checked)
                                xmlWriter = new FileMARCXMLWriter(fileName);
                            else
                                marcWriter = new FileMARCWriter(fileName, recordEncoding);
                        }
                    }

                    if (marcWriter != null)
                    {
                        marcWriter.WriteEnd();
                        marcWriter.Dispose();
                    }

                    if (xmlWriter != null)
                        xmlWriter.Dispose();
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
        /// Handles the Click event of the toCSVFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void toCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFilters = saveFileDialog.Filter;
            string saveDefaultExt = saveFileDialog.DefaultExt;

            saveFileDialog.Filter = "CSV Files|*.csv";
            saveFileDialog.DefaultExt = "*.csv";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (sender == splitToolStripMenuItem)
                {
                    using (ExportSplitDialog splitDialog = new ExportSplitDialog())
                    {
                        if (splitDialog.ShowDialog() == DialogResult.OK)
                            recordsPerFile = splitDialog.RecordsPerFile;
                        else
                            return;
                    }
                }

                this.Enabled = false;
                toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                csvExportBackgroundWorker.RunWorkerAsync(saveFileDialog.FileName);
            }

            saveFileDialog.Filter = saveFilters;
            saveFileDialog.DefaultExt = saveDefaultExt;
        }

        /// <summary>
        /// Handles the DoWork event of the csvExportBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void csvExportBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("PRAGMA table_info('Records')", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader["name"].ToString(), "");
                        }
                    }
                }

                using (SQLiteCommand command = new SQLiteCommand("PRAGMA table_info('Fields')", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader["name"].ToString().Contains("ID"))
                                columns.Add(reader["name"].ToString(), "");
                        }
                    }
                }

                using (SQLiteCommand command = new SQLiteCommand("SELECT DISTINCT TagNumber FROM Fields ORDER BY TagNumber", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader["TagNumber"].ToString(), "");
                        }
                    }
                }
            }

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
                    string fileName = e.Argument.ToString();

                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        string line = "";
                        //Write header row
                        foreach (KeyValuePair<string, string> keyValue in columns)
                        {
                            line += keyValue.Key + ",";
                        }

                        line = line.Substring(0, line.Length - 1);

                        writer.WriteLine(line);

                        foreach (DataGridViewRow row in recordsDataGridView.Rows)
                        {
                            Record record = new Record();
                            fieldsCommand.Parameters["@RecordID"].Value = row.Cells[0].Value;
                            
                            columns["RecordID"] = row.Cells[0].Value.ToString();
                            columns["DateAdded"] = row.Cells[1].Value.ToString();
                            columns["DateChanged"] = row.Cells[2].Value.ToString();
                            columns["Author"] = row.Cells[3].Value.ToString();
                            columns["Title"] = row.Cells[4].Value.ToString();
                            columns["CopyrightDate"] = row.Cells[5].Value.ToString();
                            columns["Barcode"] = row.Cells[6].Value.ToString();
                            columns["Classification"] = row.Cells[7].Value.ToString();
                            columns["MainEntry"] = row.Cells[8].Value.ToString();
                            columns["Custom1"] = row.Cells[9].Value.ToString();
                            columns["Custom2"] = row.Cells[10].Value.ToString();
                            columns["Custom3"] = row.Cells[11].Value.ToString();
                            columns["Custom4"] = row.Cells[12].Value.ToString();
                            columns["Custom5"] = row.Cells[13].Value.ToString();
                            columns["ImportErrors"] = row.Cells[14].Value.ToString();

                            using (SQLiteDataReader fieldsReader = fieldsCommand.ExecuteReader())
                            {
                                while (fieldsReader.Read())
                                {
                                    columns["TagNumber"] = fieldsReader["TagNumber"].ToString();

                                    if (fieldsReader["TagNumber"].ToString().StartsWith("00"))
                                    {
                                        columns["ControlData"] = fieldsReader["ControlData"].ToString();
                                    }
                                    else
                                    {
                                        columns["Ind1"] = fieldsReader["Ind1"].ToString();
                                        columns["Ind2"] = fieldsReader["Ind2"].ToString();

                                        subfieldsCommand.Parameters["@FieldID"].Value = fieldsReader["FieldID"];

                                        using (SQLiteDataReader subfieldReader = subfieldsCommand.ExecuteReader())
                                        {
                                            while (subfieldReader.Read())
                                            {
                                                columns[fieldsReader["TagNumber"].ToString()] += "$" + subfieldReader["Code"] + subfieldReader["Data"];
                                            }
                                        }
                                    }
                                }
                            }

                            line = "";

                            foreach (KeyValuePair<string, string> keyValue in columns)
                            {
                                line += "\"" + keyValue.Value.Replace("\"", "\"\"") + "\",";
                            }

                            line = line.Substring(0, line.Length - 1);
                            writer.WriteLine(line);

                            //Reset values
                            foreach (string key in columns.Keys.ToList())
                            {
                                columns[key] = "";
                            }

                            i++;
                            csvExportBackgroundWorker.ReportProgress(i / max);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the csvExportBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void csvExportBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString();
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the csvExportBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void csvExportBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            loading = false;
            this.Enabled = true;
        }

        #endregion

        #region Editing Cells

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

                    RebuildRecordsPreviewInformation(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
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

                    RebuildRecordsPreviewInformation(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
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
            if (!loading && fieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() != "")
            {
                switch (e.ColumnIndex)
                {
                    case 2:
                        break;
                    case 3:
                    case 4:
                        string tagNumber = fieldsDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                        if (tagNumber.StartsWith("00") || tagNumber == "")
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
            if (!loading && subfieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() != "")
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
            LoadPreview(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
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
            LoadPreview(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
        }

        #endregion

        #region Adding Rows

        /// <summary>
        /// Handles the Click event of the createBlankRecordToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void createBlankRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Records (DateAdded, Author, Title) VALUES (CURRENT_DATE, 'New Record', 'New Record')";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    int recordID = (int)connection.LastInsertRowId;

                    this.OnLoad(new EventArgs());
                    recordsDataGridView.Rows[recordsDataGridView.Rows.Count - 1].Cells[0].Selected = true;
                }
            }
        }

        private void fieldsDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int recordID = Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
            e.Row.Cells[1].Value = recordID;
        }

        /// <summary>
        /// Handles the RowValidating event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellCancelEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!loading && !fieldsDataGridView.Rows[e.RowIndex].IsNewRow && fieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() == "")
            {
                try
                {
                    int recordID = Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                    string tagNumber = fieldsDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                    string ind1 = fieldsDataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
                    string ind2 = fieldsDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (!Field.ValidateTag(tagNumber) || (tagNumber.StartsWith("00") && (ind1 != "" || ind2 != "")))
                    {
                        e.Cancel = true;
                        return;
                    }

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2) VALUES (@RecordID, @TagNumber, @Ind1, @Ind2)";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                            command.Parameters.Add("@TagNumber", DbType.String).Value = tagNumber;
                            command.Parameters.Add("@Ind1", DbType.String).Value = ind1;
                            command.Parameters.Add("@Ind2", DbType.String).Value = ind2;

                            command.ExecuteNonQuery();
                            LoadFields(recordID);
                            RebuildRecordsPreviewInformation(recordID);
                        }
                    }
                }
                catch (Exception)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Handles the RowValidating event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellCancelEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!loading && !subfieldsDataGridView.Rows[e.RowIndex].IsNewRow && subfieldsDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() == "")
            {
                try
                {
                    int recordID = Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                    int fieldID = Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                    string code = subfieldsDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                    string data = subfieldsDataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Subfields (FieldID, Code, Data) VALUES (@FieldID, @Code, @Data)";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                            command.Parameters.Add("@Code", DbType.String).Value = code;
                            command.Parameters.Add("@Data", DbType.String).Value = data;

                            command.ExecuteNonQuery();
                            LoadSubfields(fieldID);
                            RebuildRecordsPreviewInformation(recordID);
                        }
                    }
                }
                catch (Exception)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Deleting Rows

        /// <summary>
        /// Handles the UserDeletingRow event of the recordsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewRowCancelEventArgs"/> instance containing the event data.</param>
        private void recordsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() != "")
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Records WHERE RecordID = @RecordID";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add("@RecordID", DbType.Int32).Value = Int32.Parse(e.Row.Cells[0].Value.ToString());
                        command.ExecuteNonQuery();
                    }

                    RebuildRecordsPreviewInformation(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
                }
            }
        }

        /// <summary>
        /// Handles the UserDeletingRow event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewRowCancelEventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() != "")
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Fields WHERE FieldID = @FieldID";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add("@FieldID", DbType.Int32).Value = Int32.Parse(e.Row.Cells[0].Value.ToString());
                        command.ExecuteNonQuery();
                    }

                    RebuildRecordsPreviewInformation(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
                }
            }
        }

        /// <summary>
        /// Handles the UserDeletingRow event of the subfieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewRowCancelEventArgs"/> instance containing the event data.</param>
        private void subfieldsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() != "")
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Subfields WHERE SubfieldID = @SubfieldID";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add("@SubfieldID", DbType.Int32).Value = Int32.Parse(e.Row.Cells[0].Value.ToString());
                        command.ExecuteNonQuery();
                    }

                    RebuildRecordsPreviewInformation(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
                }
            }
        }

        #endregion

        #region Batch Editing

        /// <summary>
        /// Handles the Click event of the findAndReplaceToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FindReplaceForm form = new FindReplaceForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.Enabled = false;

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            StringBuilder query = new StringBuilder("UPDATE Subfields SET Data = ");

                            if (form.CaseSensitive)
                            {
                                if (form.Regex)
                                    query.Append("REGEX");
                                else //Make sure the where clause is also case sensitive
                                    query.Insert(0, "PRAGMA case_sensitive_like=ON;");

                                query.Append("REPLACE(Data, @ReplaceData, @ReplaceWith)");
                                
                            }
                            else
                            {
                                if (form.Regex)
                                    query.Append("REGEX");

                                query.Append("REPLACENOCASE(Data, @ReplaceData, @ReplaceWith)");
                            }

                            command.Parameters.Add("@ReplaceData", DbType.String).Value = form.Data;
                            command.Parameters.Add("@ReplaceWith", DbType.String).Value = form.ReplaceWith;

                            StringBuilder whereClause = new StringBuilder(" WHERE ");

                            if (form.SelectedTags.Contains("Any"))
                            {
                                // Do nothing!
                            } 
                            else if (form.SelectedTags.Count == 1)
                            {
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE TagNumber = @TagNumber) AND ");
                                command.Parameters.Add("@TagNumber", DbType.String).Value = form.SelectedTags[0];
                            }
                            else if (form.SelectedTags.Count > 1)
                            {
                                int i = 0;
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE TagNumber IN (");

                                foreach (string tag in form.SelectedTags)
                                {
                                    string tagNumber = string.Format("@TagNumber{0}", i);
                                    command.Parameters.Add(tagNumber, DbType.String).Value = tag;
                                    whereClause.AppendFormat("{0}, ", tagNumber);

                                    i++;
                                }

                                whereClause.Remove(whereClause.Length - 2, 2);
                                whereClause.Append(") AND ");
                            }

                            if (form.SelectedIndicator1s.Contains("Any"))
                            {
                                // Do nothing!
                            }
                            else if (form.SelectedIndicator1s.Count == 1)
                            {
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE Ind1 = @Ind1) AND ");
                                command.Parameters.Add("@Ind1", DbType.String).Value = form.SelectedIndicator1s[0];
                            }
                            else if (form.SelectedIndicator1s.Count > 1)
                            {
                                int i = 0;
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE Ind1 IN (");

                                foreach (string ind1 in form.SelectedIndicator1s)
                                {
                                    string indicator = string.Format("@Ind1{0}", i);
                                    command.Parameters.Add(indicator, DbType.String).Value = ind1;
                                    whereClause.AppendFormat("{0}, ", indicator);

                                    i++;
                                }
                                whereClause.Remove(whereClause.Length - 2, 2);
                                whereClause.Append(") AND ");
                            }

                            if (form.SelectedIndicator2s.Contains("Any"))
                            {
                                // Do nothing!
                            }
                            else if (form.SelectedIndicator2s.Count == 1)
                            {
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE Ind2 = @Ind2) AND ");
                                command.Parameters.Add("@Ind2", DbType.String).Value = form.SelectedIndicator2s[0];
                            }
                            else if (form.SelectedIndicator1s.Count > 1)
                            {
                                int i = 0;
                                whereClause.Append("FieldID IN (SELECT FieldID FROM Fields WHERE Ind2 IN (");

                                foreach (string ind2 in form.SelectedIndicator2s)
                                {
                                    string indicator = string.Format("@Ind2{0}", i);
                                    command.Parameters.Add(indicator, DbType.String).Value = ind2;
                                    whereClause.AppendFormat("{0}, ", indicator);

                                    i++;
                                }
                                whereClause.Remove(whereClause.Length - 2, 2);
                                whereClause.Append(") AND ");
                            }

                            if (form.SelectedCodes.Contains("Any"))
                            {
                                // Do nothing!
                            }
                            else if (form.SelectedCodes.Count == 1)
                            {
                                whereClause.Append("Code = @Code AND ");
                                command.Parameters.Add("@Code", DbType.String).Value = form.SelectedCodes[0];
                            }
                            else if (form.SelectedCodes.Count > 1)
                            {
                                int i = 0;
                                whereClause.Append("Code IN (");

                                foreach (string code in form.SelectedCodes)
                                {
                                    string codeParam = string.Format("@Code{0}", i);
                                    command.Parameters.Add(codeParam, DbType.String).Value = code;
                                    whereClause.AppendFormat("{0}, ", codeParam);

                                    i++;
                                }
                                whereClause.Remove(whereClause.Length - 2, 2);
                                whereClause.Append(") AND ");
                            }

                            if (form.Regex)
                                whereClause.Append("Data REGEXP @Data;");
                            else
                                whereClause.Append("Data LIKE @Data;");
                            
                            command.Parameters.Add("@Data", DbType.String).Value = "%" + form.Data + "%";

                            query.Append(whereClause);
                            query.Append("PRAGMA case_sensitive_like=OFF;");

                            command.CommandText = query.ToString();
                            int count = command.ExecuteNonQuery();
                            MessageBox.Show("Found and replaced " + count + " instances.", "Find and Replace Completed.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    RebuildRecordsPreviewInformation();

                    this.OnLoad(new EventArgs());
                    this.Enabled = true;
                }
            }
        }

        #endregion

        #region Print Events

        /// <summary>
        /// Handles the Click event of the currentRecordToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void currentRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                linesToPrint = new List<string>();
                printDocument.PrinterSettings = printDialog.PrinterSettings;

                printDocument.DefaultPageSettings.Margins.Left = 75;
                printDocument.DefaultPageSettings.Margins.Top = 75;
                printDocument.DefaultPageSettings.Margins.Right = 75;
                printDocument.DefaultPageSettings.Margins.Bottom = 75;

                linesToPrint.Add(START_OF_HEADING.ToString() + "Title: " + recordsDataGridView.SelectedCells[0].OwningRow.Cells[4].Value.ToString() + " -- Author: " + recordsDataGridView.SelectedCells[0].OwningRow.Cells[3].Value.ToString());

                linesToPrint.AddRange(previewTextBox.Lines);

                linesToPrint.Add(NEW_PAGE.ToString());
                printDocument.Print();
                
                linesToPrint = null;
            }
        }

        /// <summary>
        /// Handles the Click event of the allRecordsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void allRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                linesToPrint = new List<string>();

                foreach (DataGridViewRow row in recordsDataGridView.Rows)
                {
                    LoadPreview((int)row.Cells[0].Value);

                    //For some reason the Print Dialog doesn't have a working collate button. It always acts as false. :psyduck:
                    //I figure "true" is a better and more often used option
                    printDocument.PrinterSettings.Collate = true;

                    printDocument.DefaultPageSettings.Margins.Left = 75;
                    printDocument.DefaultPageSettings.Margins.Top = 75;
                    printDocument.DefaultPageSettings.Margins.Right = 75;
                    printDocument.DefaultPageSettings.Margins.Bottom = 75;

                    linesToPrint.Add(START_OF_HEADING.ToString() + "Title: " + row.Cells[4].Value.ToString() + " -- Author: " + row.Cells[3].Value.ToString());

                    linesToPrint.AddRange(previewTextBox.Lines);

                    linesToPrint.Add(NEW_PAGE.ToString());
                }

                printDocument.Print();
                linesToPrint = null;
                LoadPreview((int)recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value);
            }
        }

        /// <summary>
        /// Handles the Click event of the selectedRecordsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void selectedRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                linesToPrint = new List<string>();

                foreach (DataGridViewRow row in recordsDataGridView.SelectedRows)
                {
                    LoadPreview((int)row.Cells[0].Value);

                    //For some reason the Print Dialog doesn't have a working collate button. It always acts as false. :psyduck:
                    //I figure "true" is a better and more often used option
                    printDocument.PrinterSettings.Collate = true;

                    printDocument.DefaultPageSettings.Margins.Left = 75;
                    printDocument.DefaultPageSettings.Margins.Top = 75;
                    printDocument.DefaultPageSettings.Margins.Right = 75;
                    printDocument.DefaultPageSettings.Margins.Bottom = 75;

                    linesToPrint.Add(START_OF_HEADING.ToString() + "Title: " + row.Cells[4].Value.ToString() + " -- Author: " + row.Cells[3].Value.ToString());

                    linesToPrint.AddRange(previewTextBox.Lines);

                    linesToPrint.Add(NEW_PAGE.ToString());
                }

                printDocument.Print();
                linesToPrint = null;
                LoadPreview((int)recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value);
            }
        }

        /// <summary>
        /// Handles the PrintPage event of the printDocument control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Drawing.Printing.PrintPageEventArgs"/> instance containing the event data.</param>
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float yPos = topMargin;
            Font headerFont = new Font("Courier New", 8, FontStyle.Bold);
            Font printFont = new Font("Courier New", 10, FontStyle.Regular);
            SizeF marginBoundsSize = new SizeF(e.MarginBounds.Size.Width, e.MarginBounds.Size.Height);
            SizeF actualSize;
            bool newPage = true;
            string currentHeader = "";

            while (yPos < topMargin + e.MarginBounds.Size.Height && linesToPrint.Count > 0)
            {
                string line = linesToPrint[0];

                if (line.StartsWith(START_OF_HEADING.ToString()))
                {
                    currentHeader = line.Substring(1);
                    linesToPrint.Remove(line);
                    continue;
                }
                else if (line.StartsWith(NEW_PAGE.ToString()))
                {
                    currentHeader = string.Empty;
                    linesToPrint.Remove(line);
                    if (linesToPrint.Count > 0)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;
                    break;
                }

                if (newPage && currentHeader != string.Empty)
                {
                    e.Graphics.DrawString(currentHeader, headerFont, Brushes.Black, new RectangleF(15, 15, e.MarginBounds.Size.Width + 30, e.MarginBounds.Size.Height));
                    newPage = false;
                }

                StringFormat format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;
                format.FormatFlags = StringFormatFlags.LineLimit;
                format.Trimming = StringTrimming.Word;

                actualSize = e.Graphics.MeasureString(line, printFont, marginBoundsSize, format);
                e.Graphics.DrawString(line, printFont, Brushes.Black, new RectangleF(leftMargin, yPos, e.MarginBounds.Size.Width, e.MarginBounds.Size.Height), format);

                yPos = yPos + actualSize.Height;

                linesToPrint.Remove(line);
                if (linesToPrint.Count > 0)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
        }

        #endregion

        #region Selecting cells

        /// <summary>
        /// Handles the SelectionChanged event of the recordsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void recordsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (!loading && recordsDataGridView.SelectedCells.Count > 0)
                recordsDataGridView_CellClick(sender, new DataGridViewCellEventArgs(recordsDataGridView.SelectedCells[0].ColumnIndex, recordsDataGridView.SelectedCells[0].RowIndex));
        }

        /// <summary>
        /// Handles the SelectionChanged event of the fieldsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void fieldsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (!loading && fieldsDataGridView.SelectedCells.Count > 0)
                fieldsDataGridView_CellClick(sender, new DataGridViewCellEventArgs(fieldsDataGridView.SelectedCells[0].ColumnIndex, fieldsDataGridView.SelectedCells[0].RowIndex));
        }

        #endregion

        #region Misc Events

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

            SaveOptions();
        }

        /// <summary>
        /// Handles the Click event of the clearDatabaseOnExitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void clearDatabaseOnExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clearDatabaseOnExitToolStripMenuItem.Checked)
                clearDatabaseOnExitToolStripMenuItem.Checked = false;
            else
                clearDatabaseOnExitToolStripMenuItem.Checked = true;

            SaveOptions();
        }

        /// <summary>
        /// Handles the Click event of the exportFormatToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exportFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == uTF8ToolStripMenuItem)
            {
                uTF8ToolStripMenuItem.Checked = true;
                mARC8ToolStripMenuItem.Checked = false;
                mARCXMLToolStripMenuItem.Checked = false;
            }
            else if (sender == mARC8ToolStripMenuItem)
            {
                uTF8ToolStripMenuItem.Checked = false;
                mARC8ToolStripMenuItem.Checked = true;
                mARCXMLToolStripMenuItem.Checked = false;
            }
            else if (sender == mARCXMLToolStripMenuItem)
            {
                uTF8ToolStripMenuItem.Checked = false;
                mARC8ToolStripMenuItem.Checked = false;
                mARCXMLToolStripMenuItem.Checked = true;
            }

            SaveOptions();
        }

        /// <summary>
        /// Handles the Click event of the customFieldsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void customFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customFieldsForm.ShowDialog() == DialogResult.OK)
            {
                SaveOptions();
                RebuildRecordsPreviewInformation();
            }
            else
                this.OnLoad(new EventArgs());
        }

        /// <summary>
        /// Handles the Click event of the clearDatabaseToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void clearDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will permanently delete all records from the MARC database." + Environment.NewLine + Environment.NewLine + "Are you sure you want to delete all records?", "Delete all records?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Enabled = false;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM Records";
                        command.ExecuteNonQuery();
                    }
                }

                this.OnLoad(new EventArgs());
                this.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the resetDatabaseToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDatabase();
        }

        /// <summary>
        /// Handles the Click event of the aboutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm form = new AboutForm())
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clearDatabaseOnExitToolStripMenuItem.Checked)
                clearDatabaseToolStripMenuItem_Click(sender, e);
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

        #endregion
    }
}
