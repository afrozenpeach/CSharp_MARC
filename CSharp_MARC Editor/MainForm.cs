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
        private string rebuildingPreview = "Rebuilding Records Previews...";
        private bool startEdit = false;
        private bool loading = true;
        private bool reloadFields = false;
        private decimal recordsPerFile = 0;
        private CustomFieldsForm customFieldsForm = new CustomFieldsForm();
        private List<string> linesToPrint;
        private Exception errorLoading = null;
        int? rebuildingID = null;

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
        /// Gets the record row.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        private DataRow GetRecordRow(Record record)
        {
            DataRow row = marcDataSet.Tables["Records"].NewRow();

            string dateChanged = null;
            string author = null;
            string title = null;
            string copyright = null;
            string barcode = null;
            string classification = null;
            string mainEntry = null;
            string custom1 = null;
            string custom2 = null;
            string custom3 = null;
            string custom4 = null;
            string custom5 = null;

            ControlField field005 = null;
            Subfield subfield100a = null;
            Subfield subfield245a = null;
            Subfield subfield245b = null;
            Subfield subfield245c = null;
            Subfield subfield260c = null;
            Subfield subfield264c = null;
            Subfield subfield852h = null;
            Subfield subfield852p = null;
            Subfield subfield949a = null;
            Subfield subfield949b = null;
            Subfield subfield949c = null;
            Subfield subfield949i = null;
            Subfield subfield949g = null;
            Subfield subfieldCustom1 = null;
            Subfield subfieldCustom2 = null;
            Subfield subfieldCustom3 = null;
            Subfield subfieldCustom4 = null;
            Subfield subfieldCustom5 = null;

            field005 = (ControlField)record["005"];
            if (field005 != null && field005.Data.Length >= 14)
            {
                dateChanged = field005.Data.Substring(0, 4) + "/" + field005.Data.Substring(4, 2) + "/" + field005.Data.Substring(6, 2) + " " + field005.Data.Substring(8, 2) + ":" + field005.Data.Substring(10, 2) + ":" + field005.Data.Substring(12);
                DateTime dateTimeChanged = new DateTime();

                if (DateTime.TryParse(dateChanged, out dateTimeChanged))
                    dateChanged = dateTimeChanged.ToString();
                else
                    dateChanged = null;
            }

            DataField datafield = (DataField)record["100"];
            if (datafield != null)
                subfield100a= datafield['a'];

            datafield = (DataField)record["245"];
            if (datafield != null)
            {
                subfield245a = datafield['a'];
                subfield245b = datafield['b'];
                subfield245c = datafield['c'];
            }

            datafield = (DataField)record["260"];
            if (datafield != null)
                subfield260c = datafield['c'];

            datafield = (DataField)record["264"];
            if (datafield != null)
                subfield264c = datafield['c'];

            datafield = (DataField)record["852"];
            if (datafield != null)
            {
                subfield852h = datafield['h'];
                subfield852p = datafield['p'];
            }

            datafield = (DataField)record["949"];
            if (datafield != null)
            {
                subfield949a = datafield['a'];
                subfield949b = datafield['b'];
                subfield949c = datafield['c'];
                subfield949g = datafield['g'];
                subfield949i = datafield['i'];
            }

            if (customFieldsForm.TagNumber1 != "")
            {
                datafield = (DataField)record[customFieldsForm.TagNumber1];
                if (datafield != null)
                    subfieldCustom1 = datafield[customFieldsForm.Code1[0]];
            }

            if (customFieldsForm.TagNumber2 != "")
            {
                datafield = (DataField)record[customFieldsForm.TagNumber2];
                if (datafield != null)
                    subfieldCustom1 = datafield[customFieldsForm.Code1[0]];
            }

            if (customFieldsForm.TagNumber3 != "")
            {
                datafield = (DataField)record[customFieldsForm.TagNumber3];
                if (datafield != null)
                    subfieldCustom1 = datafield[customFieldsForm.Code1[0]];
            }

            if (customFieldsForm.TagNumber4 != "")
            {
                datafield = (DataField)record[customFieldsForm.TagNumber4];
                if (datafield != null)
                    subfieldCustom1 = datafield[customFieldsForm.Code1[0]];
            }

            if (customFieldsForm.TagNumber5 != "")
            {
                datafield = (DataField)record[customFieldsForm.TagNumber5];
                if (datafield != null)
                    subfieldCustom1 = datafield[customFieldsForm.Code1[0]];
            }

            if (subfield100a != null)
                author = subfield100a.Data;
            else if (subfield245c != null)
                author = subfield245c.Data;

            if (subfield245a != null)
                title = subfield245a.Data;
            if (subfield245b != null)
                title += " " + subfield245b.Data;

            if (subfield260c != null)
                copyright = Regex.Replace(subfield260c.Data, "[^0-9]", "");
            else if (subfield264c != null)
                copyright = Regex.Replace(subfield264c.Data, "[^0-9]", "");

            if (copyright != null && copyright.Length > 4)
                copyright = copyright.Substring(0, 4);

            if (copyright == "")
                copyright = null;

            if (subfield852p != null)
                barcode = subfield852p.Data;
            else if (subfield949i != null)
                barcode = subfield949i.Data;
            else if (subfield949g != null)
                barcode = subfield949g.Data;
            else if (subfield949b != null)
                barcode = subfield949b.Data;

            if (subfield852h != null)
            {
                string[] split = subfield852h.Data.Split(' ');
                classification = split[0];

                if (split.Length > 1)
                    mainEntry = split[1];
            }
            else if (subfield949a != null && subfield949i == null)
            {
                string[] split = subfield949a.Data.Split(' ');

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
            else if (subfield949b != null)
            {
                classification = subfield949b.Data + " " + subfield949c.Data;
            }
            else if (subfield949c != null)
            {
                string[] split = subfield949c.Data.Split(' ');

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

            if (subfieldCustom1 != null)
            {
                if (customFieldsForm.Data1 == "")
                    custom1 = subfieldCustom1.Data;
                else
                {
                    custom1 = "";
                    foreach (Match match in Regex.Matches(subfieldCustom1.Data, customFieldsForm.Data1))
                    {
                        custom1 += match.Value;
                    }
                }
            }

            if (subfieldCustom2 != null)
            {
                if (customFieldsForm.Data2 == "")
                    custom2 = subfieldCustom2.Data;
                else
                {
                    custom2 = "";
                    foreach (Match match in Regex.Matches(subfieldCustom2.Data, customFieldsForm.Data2))
                    {
                        custom2 += match.Value;
                    }
                }
            }

            if (subfieldCustom3 != null)
            {
                if (customFieldsForm.Data3 == "")
                    custom3 = subfieldCustom3.Data;
                else
                {
                    custom3 = "";
                    foreach (Match match in Regex.Matches(subfieldCustom3.Data, customFieldsForm.Data3))
                    {
                        custom3 += match.Value;
                    }
                }
            }

            if (subfieldCustom4 != null)
            {
                if (customFieldsForm.Data4 == "")
                    custom4 = subfieldCustom4.Data;
                else
                {
                    custom4 = "";
                    foreach (Match match in Regex.Matches(subfieldCustom4.Data, customFieldsForm.Data4))
                    {
                        custom4 += match.Value;
                    }
                }
            }

            if (subfieldCustom5 != null)
            {
                if (customFieldsForm.Data5 == "")
                    custom5 = subfieldCustom5.Data;
                else
                {
                    custom5 = "";
                    foreach (Match match in Regex.Matches(subfieldCustom5.Data, customFieldsForm.Data5))
                    {
                        custom5 += match.Value;
                    }
                }
            }

            if (dateChanged != null)
                row["DateChanged"] = dateChanged;

            if (author != null)
                row["Author"] = author;
            
            if (title != null)
                row["Title"] = title;

            if (copyright != null)
                row["CopyrightDate"] = copyright;

            if (barcode != null)
                row["Barcode"] = barcode;

            if (classification != null)
                row["Classification"] = classification;

            if (mainEntry != null)
                row["MainEntry"] = mainEntry;

            if (custom1 != null)
                row["Custom1"] = custom1;

            if (custom2 != null)
                row["Custom2"] = custom2;

            if (custom3 != null)
                row["Custom3"] = custom3;

            if (custom4 != null)
                row["Custom4"] = custom4;

            if (custom5 != null)
                row["Custom5"] = custom5;

            return row;
        }

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
                    if (!row.IsNewRow && row.Cells[2].Value.ToString().StartsWith("00") && !row.IsNewRow && row.Cells[2].Value.ToString().StartsWith("LDR"))
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
            fieldsDataGridView.Enabled = true;
            subfieldsDataGridView.Enabled = true;
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
        private void LoadPreview(int recordID, bool recordsTableReload = false)
        {
            Record record = new Record();

            using (SQLiteConnection fieldsConnection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY FieldID", fieldsConnection))
                {
                    fieldsCommand.Connection.Open();
                    fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);

                    using (SQLiteConnection subfieldsConnection = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY SubfieldID", subfieldsConnection))
                        {
                            subfieldsCommand.Connection.Open();
                            subfieldsCommand.Parameters.Add("@FieldID", DbType.Int32);
                            fieldsCommand.Parameters["@RecordID"].Value = recordID;

                            using (SQLiteDataReader fieldsReader = fieldsCommand.ExecuteReader())
                            {
                                while (fieldsReader.Read())
                                {
                                    if (fieldsReader["TagNumber"].ToString() == "LDR")
                                    	record.Leader = fieldsReader["ControlData"].ToString();
                                    else if (fieldsReader["TagNumber"].ToString().StartsWith("00"))
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
                }
            }

            previewTextBox.Text = record.ToString();

            if (recordsTableReload == true)
            {
                DataRow newRow = GetRecordRow(record);
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Records SET DateChanged = @DateChanged, Author = @Author, Title = @Title, CopyrightDate = @CopyrightDate, Barcode = @Barcode, Classification = @Classification, MainEntry = @MainEntry, Custom1 = @Custom1, Custom2 = @Custom2, Custom3 = @Custom3, Custom4 = @Custom4, Custom5 = @Custom5, ImportErrors = @ImportErrors WHERE RecordID = @RecordID";

                        command.Parameters.Add("@DateChanged", DbType.DateTime).Value = newRow["DateChanged"];
                        command.Parameters.Add("@Author", DbType.String).Value = newRow["Author"];
                        command.Parameters.Add("@Title", DbType.String).Value = newRow["Title"];
                        command.Parameters.Add("@CopyrightDate", DbType.String).Value = newRow["CopyrightDate"];
                        command.Parameters.Add("@Barcode", DbType.String).Value = newRow["Barcode"];
                        command.Parameters.Add("@Classification", DbType.String).Value = newRow["Classification"];
                        command.Parameters.Add("@MainEntry", DbType.String).Value = newRow["MainEntry"];
                        command.Parameters.Add("@Custom1", DbType.String).Value = newRow["Custom1"];
                        command.Parameters.Add("@Custom2", DbType.String).Value = newRow["Custom2"];
                        command.Parameters.Add("@Custom3", DbType.String).Value = newRow["Custom3"];
                        command.Parameters.Add("@Custom4", DbType.String).Value = newRow["Custom4"];
                        command.Parameters.Add("@Custom5", DbType.String).Value = newRow["Custom5"];
                        
                        string errors = "";
                        foreach (string error in record.Warnings)
                            errors += error + Environment.NewLine;

                        if (errors.Length > 1)
                            errors = errors.Substring(0, errors.Length - 1);

                        command.Parameters.Add("@ImportErrors", DbType.String).Value = errors;
                        command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                        command.ExecuteNonQuery();

                        foreach (DataGridViewRow row in recordsDataGridView.Rows)
                        {
                            if (Int32.Parse(row.Cells[0].Value.ToString()) == recordID)
                            {
                                row.Cells[2].Value = newRow["DateChanged"];
                                row.Cells[3].Value = newRow["Author"];
                                row.Cells[4].Value = newRow["Title"];
                                row.Cells[5].Value = newRow["CopyrightDate"];
                                row.Cells[6].Value = newRow["Barcode"];
                                row.Cells[7].Value = newRow["Classification"];
                                row.Cells[8].Value = newRow["MainEntry"];
                                row.Cells[9].Value = newRow["Custom1"];
                                row.Cells[10].Value = newRow["Custom2"];
                                row.Cells[11].Value = newRow["Custom3"];
                                row.Cells[12].Value = newRow["Custom4"];
                                row.Cells[13].Value = newRow["Custom5"];
                                break;
                            }
                        }
                    }
                }
            }
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
                DisableForm();

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

                                                CREATE TABLE [TempUpdates](
                                                    [RecordID] integer, 
                                                    [Data] nvarchar);

                                                CREATE INDEX [FieldID]
                                                ON [Subfields](
                                                    [FieldID] ASC);

                                                CREATE INDEX [RecordID]
                                                ON [Fields](
                                                    [RecordID] ASC);

                                                CREATE INDEX [RecordID_Data]
                                                ON [TempUpdates](
                                                    [RecordID], 
                                                    [Data]);";

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Rebuilds the records preview information.
        /// This consists of the Author, Title, Barcode, Classification, and MainEntry fields
        /// </summary>
        private void RebuildRecordsPreviewInformation(int? recordID = null, string tagNumber = null)
        {
            Console.WriteLine("Begin rebuild: " + DateTime.Now.ToString());
            string whereRecordID = "";

            if (recordID != null)
                whereRecordID = " AND RecordID = " + recordID;

            using (SQLiteConnection readerConnection = new SQLiteConnection(connectionString))
            {
                readerConnection.Open();

                using (SQLiteCommand command = new SQLiteCommand(readerConnection))
                {
                    switch (tagNumber)
                    {
                        case "customOnly":
                            command.CommandText = "UPDATE Records SET Custom1 = null, Custom2 = null, Custom3 = null, Custom4 = null, Custom5 = null";
                            break;
                        case "100":
                        case "245":
                            command.CommandText = "UPDATE Records SET Author = null, Title = null";
                            break;
                        case "260":
                            command.CommandText = "UPDATE Records SET CopyrightDate = null";
                            break;
                        case "264":
                            command.CommandText = "UPDATE Records SET CopyrightDate = null";
                            break;
                        case "852":
                        case "949":
                            command.CommandText = "UPDATE Records SET Classification = null, MainEntry = null, Barcode = null";
                            break;
                        default:
                            if (customFieldsForm.TagNumber1 == tagNumber)
                                command.CommandText = "UPDATE Records SET Custom1 = null";
                            else if (customFieldsForm.TagNumber2 == tagNumber)
                                command.CommandText = "UPDATE Records SET Custom2 = null";
                            else if (customFieldsForm.TagNumber3 == tagNumber)
                                command.CommandText = "UPDATE Records SET Custom3 = null";
                            else if (customFieldsForm.TagNumber4 == tagNumber)
                                command.CommandText = "UPDATE Records SET Custom4 = null";
                            else if (customFieldsForm.TagNumber5 == tagNumber)
                                command.CommandText = "UPDATE Records SET Custom5 = null";
                            else if (tagNumber == null)
                                command.CommandText = "UPDATE Records SET Author = null, Title = null, CopyrightDate = null, Barcode = null, Classification = null, MainEntry = null, Custom1 = null, Custom2 = null, Custom3 = null, Custom4 = null, Custom5 = null";
                            break;
                    }

                    if (command.CommandText != null)
                    {
                        if (recordID != null)
                            command.CommandText += " WHERE RecordID = " + recordID;

                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "100" || tagNumber == "245")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '100' and Code = 'a'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Author = (SELECT Data FROM TempUpdates
                                                              WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '245' and Code = 'c'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Author = (SELECT Data FROM TempUpdates
                                                              WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Author IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '245' and Code = 'a'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Title = (SELECT Data FROM TempUpdates
                                                              WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '245' and Code = 'b'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Title = Title || ' ' || (SELECT Data FROM TempUpdates
                                                                             WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "260")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '260' and Code = 'c'" + whereRecordID + @";

                                                UPDATE Records
                                                SET CopyrightDate = (SELECT CAST(SUBSTR(REGEXREPLACE(Data, '[^0-9]', ''), 1, 4) as 'integer') FROM TempUpdates
                                                                     WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "264")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '264' and Code = 'c'" + whereRecordID + @";

                                                UPDATE Records
                                                SET CopyrightDate = (SELECT CAST(SUBSTR(REGEXREPLACE(Data, '[^0-9]', ''), 1, 4) as 'integer') FROM TempUpdates
                                                                     WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE CopyrightDate IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "852")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '852' and Code = 'p'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Barcode = (SELECT Data FROM TempUpdates
                                                               WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "949")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '949' and Code = 'i'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Barcode = (SELECT Data FROM TempUpdates
                                                               WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Barcode IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '949' and Code = 'g'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Barcode = (SELECT Data FROM TempUpdates
                                                               WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Barcode IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '949' and Code = 'b'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Barcode = (SELECT Data FROM TempUpdates
                                                               WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Barcode IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "852")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '852' and Code = 'h'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Classification = (SELECT SPLITSUBSTRING(Data, ' ', 0) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (tagNumber == null || tagNumber == "949")
                    {
                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '949' and Code = 'a'" + whereRecordID + @"
                                                        AND (SELECT COUNT(*) 
                                                             FROM Fields f2
                                                             LEFT OUTER JOIN Subfields s2 ON s2.FieldID = f2.FieldID
                                                             WHERE f2.TagNumber = '949' AND s2.Code = 'i'" + whereRecordID + @") > 0;

                                                UPDATE Records
                                                SET Classification = (SELECT SPLITSUBSTRING(Data, ' ', 0) || ' ' || SPLITSUBSTRING(Data, ' ', 1) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Classification IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields
                                                  LEFT OUTER JOIN Fields f on f.FieldID = Subfields.FieldID
                                                  WHERE f.TagNumber = '949' and Code = 'a'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Classification = (SELECT SPLITSUBSTRING(Data, ' ', 0) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Classification IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, SPLITSUBSTRING(s.Data, ' ', 0) || ' ' || SPLITSUBSTRING(s2.Data, ' ', 0)
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  LEFT OUTER JOIN Subfields s2 on f.FieldID = s2.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'b' and s2.Code = 'c'" + whereRecordID + @";

                                                UPDATE Records
                                                SET Classification = (SELECT Data FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Classification IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'c' and SPLITCOUNT(s.Data, ' ') > 1 " + whereRecordID + @";

                                                UPDATE Records
                                                SET Classification = (SELECT SPLITSUBSTRING(Data, ' ', 0) || ' ' || SPLITSUBSTRING(Data, ' ', 1) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Classification IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'c' " + whereRecordID + @";

                                                UPDATE Records
                                                SET Classification = (SELECT SPLITSUBSTRING(Data, ' ', 0) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE Classification IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'h' " + whereRecordID + @";

                                                UPDATE Records
                                                SET MainEntry = (SELECT SPLITSUBSTRING(Data, ' ', 1) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'a' AND SPLITCOUNT(s.Data, ' ') > 2" + whereRecordID + @";

                                                UPDATE Records
                                                SET MainEntry = (SELECT SPLITSUBSTRING(Data, ' ', 2) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE MainEntry IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'a'" + whereRecordID + @";

                                                UPDATE Records
                                                SET MainEntry = (SELECT SPLITSUBSTRING(Data, ' ', 1) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE MainEntry IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'c' AND SPLITCOUNT(s.Data, ' ') > 2" + whereRecordID + @";

                                                UPDATE Records
                                                SET MainEntry = (SELECT SPLITSUBSTRING(Data, ' ', 2) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE MainEntry IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;"; 
                        command.ExecuteNonQuery();

                        command.CommandText = @"INSERT INTO TempUpdates
                                                  SELECT f.RecordID, Data
                                                  FROM Subfields s
                                                  LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                  WHERE f.TagNumber = '949' and s.Code = 'c'" + whereRecordID + @";

                                                UPDATE Records
                                                SET MainEntry = (SELECT SPLITSUBSTRING(Data, ' ', 1) FROM TempUpdates
                                                                      WHERE TempUpdates.RecordID = Records.RecordID)
                                                WHERE MainEntry IS NULL AND RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                DELETE FROM TempUpdates;";
                        command.ExecuteNonQuery();
                    }

                    if (customFieldsForm.TagNumber1 != "" && (tagNumber == "customOnly" || tagNumber == customFieldsForm.TagNumber1))
                    {
                        if (customFieldsForm.Data1 != "")
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, REGEXMATCH(Data, @Data)
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom1 = (SELECT Data FROM TempUpdates
                                                                          WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;";
                            command.Parameters.Add("@TagNumber", DbType.String).Value = customFieldsForm.TagNumber1;
                            command.Parameters.Add("@Code", DbType.String).Value = customFieldsForm.Code1;
                            command.Parameters.Add("@Data", DbType.String).Value = customFieldsForm.Data1;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, Data
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom1 = (SELECT Data FROM TempUpdates
                                                                        WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("TagNumber", DbType.String).Value = customFieldsForm.TagNumber1;
                            command.Parameters.Add("Code", DbType.String).Value = customFieldsForm.Code1;
                            command.ExecuteNonQuery();
                        }
                    }

                    if (customFieldsForm.TagNumber2 != "" && (tagNumber == "customOnly" || tagNumber == customFieldsForm.TagNumber2))
                    {
                        if (customFieldsForm.Data2 != "")
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, REGEXMATCH(Data, @Data)
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom2 = (SELECT Data FROM TempUpdates
                                                                          WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("@TagNumber", DbType.String).Value = customFieldsForm.TagNumber2;
                            command.Parameters.Add("@Code", DbType.String).Value = customFieldsForm.Code2;
                            command.Parameters.Add("@Data", DbType.String).Value = customFieldsForm.Data2;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, Data
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom2 = (SELECT Data FROM TempUpdates
                                                                        WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("TagNumber", DbType.String).Value = customFieldsForm.TagNumber2;
                            command.Parameters.Add("Code", DbType.String).Value = customFieldsForm.Code2;
                            command.ExecuteNonQuery();
                        }
                    }

                    if (customFieldsForm.TagNumber3 != "" && (tagNumber == "customOnly" || tagNumber == customFieldsForm.TagNumber3))
                    {
                        if (customFieldsForm.Data3 != "")
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, REGEXMATCH(Data, @Data)
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom3 = (SELECT Data FROM TempUpdates
                                                                          WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("@TagNumber", DbType.String).Value = customFieldsForm.TagNumber3;
                            command.Parameters.Add("@Code", DbType.String).Value = customFieldsForm.Code3;
                            command.Parameters.Add("@Data", DbType.String).Value = customFieldsForm.Data3;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, Data
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom3 = (SELECT Data FROM TempUpdates
                                                                        WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("TagNumber", DbType.String).Value = customFieldsForm.TagNumber3;
                            command.Parameters.Add("Code", DbType.String).Value = customFieldsForm.Code3;
                            command.ExecuteNonQuery();
                        }
                    }

                    if (customFieldsForm.TagNumber4 != "" && (tagNumber == "customOnly" || tagNumber == customFieldsForm.TagNumber4))
                    {
                        if (customFieldsForm.Data4 != "")
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, REGEXMATCH(Data, @Data)
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom4 = (SELECT Data FROM TempUpdates
                                                                          WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("@TagNumber", DbType.String).Value = customFieldsForm.TagNumber4;
                            command.Parameters.Add("@Code", DbType.String).Value = customFieldsForm.Code4;
                            command.Parameters.Add("@Data", DbType.String).Value = customFieldsForm.Data4;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, Data
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom4 = (SELECT Data FROM TempUpdates
                                                                        WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("TagNumber", DbType.String).Value = customFieldsForm.TagNumber4;
                            command.Parameters.Add("Code", DbType.String).Value = customFieldsForm.Code4;
                            command.ExecuteNonQuery();
                        }
                    }

                    if (customFieldsForm.TagNumber5 != "" && (tagNumber == "customOnly" || tagNumber == customFieldsForm.TagNumber5))
                    {
                        if (customFieldsForm.Data5 != "")
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, REGEXMATCH(Data, @Data)
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom5 = (SELECT Data FROM TempUpdates
                                                                          WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("@TagNumber", DbType.String).Value = customFieldsForm.TagNumber5;
                            command.Parameters.Add("@Code", DbType.String).Value = customFieldsForm.Code5;
                            command.Parameters.Add("@Data", DbType.String).Value = customFieldsForm.Data5;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"INSERT INTO TempUpdates
                                                      SELECT f.RecordID, Data
                                                      FROM Subfields s
                                                      LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                      WHERE f.TagNumber = @TagNumber and s.Code = @Code" + whereRecordID + @";

                                                    UPDATE Records
                                                    SET Custom5 = (SELECT Data FROM TempUpdates
                                                                        WHERE TempUpdates.RecordID = Records.RecordID)
                                                    WHERE RecordID IN (SELECT RecordID FROM TempUpdates); 

                                                    DELETE FROM TempUpdates;"; 
                            command.Parameters.Add("TagNumber", DbType.String).Value = customFieldsForm.TagNumber5;
                            command.Parameters.Add("Code", DbType.String).Value = customFieldsForm.Code5;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            Console.WriteLine("End rebuild: " + DateTime.Now.ToString());
        }

        /// <summary>
        /// Loads the options.
        /// </summary>
        private void LoadOptions()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Settings LIMIT 1", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["RecordListAtTop"] != DBNull.Value && !(bool)reader["RecordListAtTop"] && recordListAtTopToolStripMenuItem.Checked)
                                recordListAtTopToolStripMenuItem_Click(this, new EventArgs());
                            else
                                recordListAtTopToolStripMenuItem.Checked = true;

                            if (reader["ClearDatabaseOnExit"] != DBNull.Value && (bool)reader["ClearDatabaseOnExit"] && !clearDatabaseOnExitToolStripMenuItem.Checked)
                                clearDatabaseOnExitToolStripMenuItem_Click(this, new EventArgs());
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
        }

        /// <summary>
        /// Reloads the record row.
        /// </summary>
        /// <param name="row">The row.</param>
        private void ReloadRecordRow(DataGridViewRow row)
        {
            if (startEdit || rebuildingID != null)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Records WHERE RecordID = @RecordID", connection))
                    {
                        command.Parameters.Add("@RecordID", DbType.Int32).Value = row.Cells[0].Value;
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            row.Cells[0].Value = reader[0];
                            row.Cells[1].Value = reader[1];
                            row.Cells[2].Value = reader[2];
                            row.Cells[3].Value = reader[3];
                            row.Cells[4].Value = reader[4];
                            row.Cells[5].Value = reader[5];
                            row.Cells[6].Value = reader[6];
                            row.Cells[7].Value = reader[7];
                            row.Cells[8].Value = reader[8];
                            row.Cells[9].Value = reader[9];
                            row.Cells[10].Value = reader[10];
                            row.Cells[11].Value = reader[11];
                            row.Cells[12].Value = reader[12];
                            row.Cells[13].Value = reader[13];
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

        /// <summary>
        /// Enables the form.
        /// </summary>
        private void EnableForm()
        {
            menuStrip.Enabled = true;
            recordsDataGridView.Enabled = true;
            fieldsDataGridView.Enabled = true;
            subfieldsDataGridView.Enabled = true;
        }

        /// <summary>
        /// Disables the form.
        /// </summary>
        private void DisableForm()
        {
            menuStrip.Enabled = false;
            recordsDataGridView.Enabled = false;
            fieldsDataGridView.Enabled = false;
            subfieldsDataGridView.Enabled = false;
        }

        #endregion

        #region SQLite Addon functions

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

        [SQLiteFunction(Name = "SPLITSUBSTRING", Arguments = 3, FuncType = FunctionType.Scalar)]
        class SPLITSUBSTRING : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                string[] split = args[0].ToString().Split(args[1].ToString().ToCharArray());
                int args2 = -1;
                Int32.TryParse(args[2].ToString(), out args2);
                if (args2 != -1 && args2 < split.Length)
                    return split[args2];
                else
                    return null;
            }
        }

        [SQLiteFunction(Name = "SPLITCOUNT", Arguments = 2, FuncType = FunctionType.Scalar)]
        class SPLITCOUNT : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                string[] split = args[0].ToString().Split(args[1].ToString().ToCharArray());
                return split.Length;
            }
        }

        [SQLiteFunction(Name = "REGEXMATCH", Arguments = 2, FuncType = FunctionType.Scalar)]
        class REGEXMATCH : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                StringBuilder matches = new StringBuilder();

                foreach (Match match in Regex.Matches(args[0].ToString(), args[1].ToString()))
                {
                    matches.Append(match.Value);
                }

                return matches.ToString();
            }
        }

        #endregion

        #region Form Events

        #region Loading

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("MARC.db"))
            {
                ResetDatabase(true);
            }

            loading = true;
            DisableForm();
            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar.MarqueeAnimationSpeed = 30;
            toolStripProgressBar.Enabled = true;
            toolStripProgressBar.Visible = true;
            progressToolStripStatusLabel.Visible = true;
            recordsDataGridView.SuspendLayout();
            fieldsDataGridView.SuspendLayout();
            subfieldsDataGridView.SuspendLayout();
            recordsDataGridView.DataSource = null;
            fieldsDataGridView.DataSource = null;
            subfieldsDataGridView.DataSource = null;
            progressToolStripStatusLabel.Text = "Loading database..";
            loadingBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles the DoWork event of the loadingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void loadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                marcDataSet.Tables["Records"].Rows.Clear();
                marcDataSet.Tables["Fields"].Rows.Clear();
                marcDataSet.Tables["Subfields"].Rows.Clear();

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Records", connection))
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                        dataAdapter.Fill(marcDataSet, "Records");
                    }

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Fields WHERE 1 = 0", connection))
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                        dataAdapter.Fill(marcDataSet, "Fields");
                    }

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Subfields WHERE 1 = 0", connection))
                    {
                        SQLiteDataAdapter recordsDataAdapter = new SQLiteDataAdapter(command);
                        recordsDataAdapter.Fill(marcDataSet, "Subfields");
                    }
                }
            }
            catch (Exception ex)
            {
                errorLoading = ex;
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the loadingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void loadingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (errorLoading != null)
            {
                if (MessageBox.Show("Error loading database: " + errorLoading.GetType().ToString() + " - " + errorLoading.Message + Environment.NewLine + Environment.NewLine + "If you continue to see this message, it may be necessary to reset the database. Doing so will permanently delete all records from the database." + Environment.NewLine + Environment.NewLine + "Do you want to reset the database?", "Error loading database.", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    ResetDatabase(true);
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }

            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
            fieldsDataGridView.DataSource = marcDataSet.Tables["Fields"];
            subfieldsDataGridView.DataSource = marcDataSet.Tables["Subfields"];
            recordsDataGridView.ResumeLayout();
            fieldsDataGridView.ResumeLayout();
            subfieldsDataGridView.ResumeLayout();

            LoadOptions();

            if (recordsDataGridView.Rows.Count > 0)
            {
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(0, 0);
                recordsDataGridView_CellClick(this, args);
            }

            loading = false;
            EnableForm();
        }

        #endregion 

        #region Loading Records

        /// <summary>
        /// Handles the CellClick event of the recordsDataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void recordsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && recordsDataGridView.Rows.Count > 0)
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
                    if (rowClicked.Cells[2].Value.ToString().StartsWith("00") || rowClicked.Cells[2].Value.ToString() == "LDR")
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
                Console.WriteLine("Start Import: " + DateTime.Now.ToString());
                recordsDataGridView.Enabled = false;
                subfieldsDataGridView.Enabled = false;
                fieldsDataGridView.Enabled = false;
                menuStrip.Enabled = false;
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar.MarqueeAnimationSpeed = 30;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                cancelButtonToolStripStatusLabel.Visible = true;
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
            List<Record> importedSRU;

            if (e.Argument.GetType() == typeof(string))
            {
                marcRecords = new FileMARCReader(e.Argument.ToString());
                recordEnumerator = marcRecords;
            }
            else
            {
                importedSRU = (List<Record>)e.Argument;
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
                        if (importingBackgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            cancelButtonToolStripStatusLabel.Visible = false;
                            break;
                        }
                        DataRow newRow = GetRecordRow(record);

                        command.CommandText = "INSERT INTO Records (DateAdded, DateChanged, Author, Title, CopyrightDate, Barcode, Classification, MainEntry, Custom1, Custom2, Custom3, Custom4, Custom5, ImportErrors) VALUES (@DateAdded, @DateChanged, @Author, @Title, @CopyrightDate, @Barcode, @Classification, @MainEntry, @Custom1, @Custom2, @Custom3, @Custom4, @CUstom5, @ImportErrors)";

                        command.Parameters.Add("@DateAdded", DbType.DateTime).Value = DateTime.Now;
                        DateTime changed = new DateTime();
                        if (DateTime.TryParse(newRow["DateChanged"].ToString(), out changed))
                            command.Parameters.Add("@DateChanged", DbType.DateTime).Value = changed;
                        else
                            command.Parameters.Add("@DateChanged", DbType.DateTime).Value = null;
                        command.Parameters.Add("@Author", DbType.String).Value = newRow["Author"];
                        command.Parameters.Add("@Title", DbType.String).Value = newRow["Title"];
                        command.Parameters.Add("@CopyrightDate", DbType.String).Value = newRow["CopyrightDate"];
                        command.Parameters.Add("@Barcode", DbType.String).Value = newRow["Barcode"];
                        command.Parameters.Add("@Classification", DbType.String).Value = newRow["Classification"];
                        command.Parameters.Add("@MainEntry", DbType.String).Value = newRow["MainEntry"];
                        command.Parameters.Add("@Custom1", DbType.String).Value = newRow["Custom1"];
                        command.Parameters.Add("@Custom2", DbType.String).Value = newRow["Custom2"];
                        command.Parameters.Add("@Custom3", DbType.String).Value = newRow["Custom3"];
                        command.Parameters.Add("@Custom4", DbType.String).Value = newRow["Custom4"];
                        command.Parameters.Add("@Custom5", DbType.String).Value = newRow["Custom5"];

                        string errors = "";
                        foreach (string error in record.Warnings)
                            errors += error + Environment.NewLine;
                        
                        if (errors.Length > 1)
                            errors = errors.Substring(0, errors.Length - 1);

                        command.Parameters.Add("@ImportErrors", DbType.String).Value = errors;

                        command.ExecuteNonQuery();
                        
                        int recordID = (int)connection.LastInsertRowId;

                        command.CommandText = "INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, ControlData) VALUES (@RecordID, @TagNumber, @Ind1, @Ind2, @ControlData)";
                        command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                        command.Parameters.Add("@TagNumber", DbType.String).Value ="LDR";
                        command.Parameters.Add("@Ind1", DbType.String).Value = DBNull.Value;
                        command.Parameters.Add("@Ind2", DbType.String).Value = DBNull.Value;
                        command.Parameters.Add("@ControlData", DbType.String).Value = record.Leader;
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

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
                        i++;
                        importingBackgroundWorker.ReportProgress(i);
                    }

                    i = -2;
                    importingBackgroundWorker.ReportProgress(i);

                    Console.WriteLine("Start Commit: " + DateTime.Now.ToString());
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
                case -3:
                    progressToolStripStatusLabel.Text = rebuildingPreview;
                    break;
                default:
                    progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString();
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the cancelButtonToolStripStatusLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelButtonToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            importingBackgroundWorker.CancelAsync();
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
            cancelButtonToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
            recordsDataGridView.ResumeLayout();
            loading = false;
            recordsDataGridView.Enabled = true;
            subfieldsDataGridView.Enabled = true;
            fieldsDataGridView.Enabled = true;
            menuStrip.Enabled = true;
            Console.WriteLine("End Import: " + DateTime.Now.ToString());
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
                    importingBackgroundWorker.RunWorkerAsync(sru.SelectedRecords);
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

                DisableForm();
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
            using (SQLiteConnection fieldsConnection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY FieldID", fieldsConnection))
                {
                    fieldsCommand.Connection.Open();
                    fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);
                    using (SQLiteConnection subfieldsConnection = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY SubfieldID", subfieldsConnection))
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
                                        if (fieldsReader["TagNumber"].ToString() == "LDR")
                                            record.Leader = fieldsReader["ControlData"].ToString();
                                        else if (fieldsReader["TagNumber"].ToString().StartsWith("00"))
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
                                exportingBackgroundWorker.ReportProgress(Convert.ToInt32(((float)i / (float)max) * 100));

                                if (recordsPerFile != 0 && recordCounter > recordsPerFile)
                                {
                                    recordCounter = 1;
                                    fileCounter++;
                                    
                                    fileName = e.Argument.ToString().Substring(0, e.Argument.ToString().LastIndexOf('.')) + "." + fileCounter + "." + e.Argument.ToString().Substring(e.Argument.ToString().LastIndexOf('.') + 1);
                                    
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
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the exportingBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void exportingBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= 100)
            {
                progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString() + "%";
                toolStripProgressBar.Value = e.ProgressPercentage;
            }
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
            EnableForm();
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

                DisableForm();
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

            using (SQLiteConnection fieldsConnection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY FieldID", fieldsConnection))
                {
                    fieldsCommand.Connection.Open();
                    fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);

                    using (SQLiteConnection subfieldsConnection = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY SubfieldID", subfieldsConnection))
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
                                            
                                            if (fieldsReader["TagNumber"].ToString() == "LDR")
                                            	columns["LDR"] = fieldsReader["ControlData"].ToString();
                                            else if (fieldsReader["TagNumber"].ToString().StartsWith("00"))
                                            {
                                                columns[fieldsReader["TagNumber"].ToString()] = fieldsReader["ControlData"].ToString();
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
                                    csvExportBackgroundWorker.ReportProgress(Convert.ToInt32(((float)i / (float)max) * 100));
                                }
                            }
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
            if (e.ProgressPercentage <= 100)
            {
                progressToolStripStatusLabel.Text = e.ProgressPercentage.ToString() + "%";
                toolStripProgressBar.Value = e.ProgressPercentage;
            }
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
            EnableForm();
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
                            reloadFields = true;
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
                            }
                        }
                    }

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand("UPDATE Fields SET ControlData = @Data WHERE FieldID IN (SELECT FieldID FROM Fields WHERE RecordID = (SELECT RecordID FROM Fields WHERE FieldID = @FieldID) AND TagNumber = '005')", connection))
                        {
                            command.Parameters.Add("@Data", DbType.String).Value = DateTime.Now.ToString("yyyyMMddHHmmss.f");
                            command.Parameters.Add("@FieldID", DbType.Int32).Value = subfieldsDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                            command.ExecuteNonQuery();
                        }
                    }

                    reloadFields = true;
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
                string tagNumber = fieldsDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                switch (e.ColumnIndex)
                {
                    case 2:
                        if (tagNumber == "005")
                        {
                            MessageBox.Show("The 005 field is updated automatically when changes are made.", "Field 005 is locked.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            startEdit = false;
                            e.Cancel = true;
                        }
                        break;
                    case 3:
                    case 4:
                        if (tagNumber.StartsWith("00") || tagNumber == "")
                        {
                            MessageBox.Show("Control Fields do not have indicators.", "Indicators are locked.", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (fieldsDataGridView.SelectedCells[0].OwningRow.Cells[2].Value.ToString() == "005")
            {
                MessageBox.Show("The 005 field is updated automatically when changes are made.", "Field 005 is locked.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                startEdit = false;
                e.Cancel = true;
                return;
            }

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
            
            if (recordsDataGridView.SelectedCells.Count > 0)
                LoadPreview(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()), reloadFields);

            reloadFields = false;
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

            if (recordsDataGridView.SelectedCells.Count > 0)
                LoadPreview(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()), reloadFields);

            reloadFields = false;
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

                    this.OnLoad(new EventArgs());
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
                    DisableForm();

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
                    EnableForm();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the validateRecordsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void validateRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented, but is coming soon.", "Coming Soon!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles the Click event of the convertToRDAToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void convertToRDAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will attempt to convert AACR2 records to RDA records by making assumptions based on the data in the record. It is not intended to be a complete RDA conversion solution, and records should be verified after conversion." + Environment.NewLine + Environment.NewLine + "Are you sure you want to begin the conversion?", "Convert Database to RDA", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                DisableForm();
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar.MarqueeAnimationSpeed = 30;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                recordsDataGridView.SuspendLayout();
                recordsDataGridView.DataSource = null;
                rdaConversionBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Handles the DoWork event of the rdaConversionBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void rdaConversionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    rdaConversionBackgroundWorker.ReportProgress(0);
                    command.CommandText = @"UPDATE Fields SET ControlData = SUBSTR(ControlData, 1, 18) || 'i' || SUBSTR(ControlData, 20) WHERE TagNumber = @TagNumber";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "LDR";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(20);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT f.FieldID, SPLITSUBSTRING(Data, '(', 1)
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = @TagNumber and s.FieldID IS NOT NULL;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT RecordID, @Code, REPLACE(Data, ')', '')
                                                FROM TempUpdates WHERE Data is not null;

                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, LTRIM(RTRIM(SPLITSUBSTRING(Data, '(', 0)))
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = @TagNumber and s.Data is not null;

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates);

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "020";
                    command.Parameters.Add("Code", DbType.String).Value = "q";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(40);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT f.FieldID, ' '
                                                FROM Fields f
                                                WHERE f.TagNumber = @TagNumber;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT RecordID, @Code, 'rda'
                                                FROM TempUpdates;

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "040";
                    command.Parameters.Add("Code", DbType.String).Value = "e";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(100);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(s.Data, ' (ca.)', 'approximately'), 'ca.', 'approximately'), 'b.', 'born'), 'd.', 'died'), 'fl.', 'flourished')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data LIKE '% ca\.%' ESCAPE '\' or s.Data LIKE '%b\.' ESCAPE '\' or s.Data LIKE '%d\.' ESCAPE '\' or s.Data LIKE '%fl\.' ESCAPE '\');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "100";
                    command.Parameters.Add("Code", DbType.String).Value = "d";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(245);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(s.Data, '... [et al.]', '[and others]'), '[et al.]', '[and others]')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and s.Data LIKE '% et al\.%' ESCAPE '\';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "245";
                    command.Parameters.Add("Code", DbType.String).Value = "c";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(250);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Data, 'ed.', 'edition'), '1st', 'first'), '2nd', 'second'), '3rd', 'third'), '4th', 'fourth'), '5th', 'fifth'), '6th', 'sixth'), '7th', 'seventh'), '8th', 'eighth'), '9th', 'ninth'), '10th', 'tenth'), '12th', 'twelvth'), '13th', 'thirteenth'), '14th', 'fourteenth'), '15th', 'fifteenth'), '16th', 'sixteenth'), '17th', 'seventeenth'), '18th', 'eighteenth'), '19th', 'nineteenth'), '20th', 'twentieth')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code;

                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Data, 'Rev.', 'Revised'), 'rev.', 'revised'), 'pbk', 'paperback'), 'ill.', 'illustrated'), 'la edition', 'Primera edición'), 'Edicion en espanol', 'Edición en espanol'), 'izd.', 'izdája'), '2d', 'second'), '3d', 'third'), 'edition.,', 'edition,'), ' Second ', ' second '), 'edition. of:', 'edition of:')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code;

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "250";
                    command.Parameters.Add("Code", DbType.String).Value = "a";
                    command.ExecuteNonQuery();
                    
                    rdaConversionBackgroundWorker.ReportProgress(260);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(Data, 'c1', '©1'), 'c2', '©2')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code;

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;

                                            UPDATE Fields SET TagNumber = '264', Ind2 = '4' WHERE TagNumber = @TagNumber;

                                            INSERT INTO TempUpdates
                                                SELECT f.FieldID, ''
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on f.FieldID = s.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '264' and s.Code IS NOT NULL;
                        
                                            UPDATE Fields
                                            SET Ind2 = '1'
                                            WHERE FieldID IN (Select RecordID FROM TempUpdates);
                                            
                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "260";
                    command.Parameters.Add("Code", DbType.String).Value = "c";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(s.Data || ', ©' || SUBSTR(s2.Data, 12, 4), '.,', ',')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                LEFT OUTER JOIN Fields f2 on f2.RecordID = f.RecordID and f.TagNumber = '008'
                                                LEFT OUTER JOIN Subfields s2 on f2.FieldID = s2.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and SUBSTR(s2.Data, 12, 4) != '    ';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "260";
                    command.Parameters.Add("Code", DbType.String).Value = "c";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(300);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(Data, 'p.', 'pages'), 'v.', 'volumes'), '[', ''), ']', ' unnumbered')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data LIKE '%p\.%' ESCAPE '\' or s.Data LIKE '%v\.%' ESCAPE '\' or s.Data LIKE '%[%' ESCAPE '\');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "300";
                    command.Parameters.Add("Code", DbType.String).Value = "a";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Data, 'col.', 'color'), 'ill.', 'illustrations'), 'facsims', 'facsimiles'), 'geneal. tables', 'genealogical tables'), 'ports.', 'portraits')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data LIKE '%col\.%' ESCAPE '\' or s.Data LIKE '%ill\.%' ESCAPE '\');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "300";
                    command.Parameters.Add("Code", DbType.String).Value = "b";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(Data, 'CD', 'compact disc'), 'DVD', 'videodisc'), 'CD-ROM', 'computer disc')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data like '%CD%' or s.Data like '%DVD%');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "300";
                    command.Parameters.Add("Code", DbType.String).Value = "c";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(336);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT FieldID, SUBSTR(ControlData, 7, 1)
                                                FROM Fields
                                                WHERE TagNumber = 'LDR';

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2)
                                                SELECT RecordID, '336', ' ', ' '
                                                FROM TempUpdates;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'a',
                                                    CASE t.Data
                                                        WHEN 'e' THEN 'cartographic image'
                                                        WHEN 'f' THEN 'cartographic image'
                                                        WHEN 'm' THEN 'computer program'
                                                        WHEN 'a' THEN 'text'
                                                        WHEN 't' THEN 'text'
                                                        WHEN 'c' THEN 'notated music'
                                                        WHEN 'd' THEN 'notated music'
                                                        WHEN 'j' THEN 'performed music'
                                                        WHEN 'i' THEN 'spoken word'
                                                        WHEN 'k' THEN 'still image'
                                                        WHEN 'r' THEN 'three-dimensional form'
                                                        WHEN 'g' THEN 'two-dimensional moving image'
                                                        WHEN 'o' THEN 'other'
                                                        WHEN 'p' THEN 'other'
                                                        ELSE 'unspecified'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '336' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'b',
                                                    CASE t.Data
                                                        WHEN 'e' THEN 'cri'
                                                        WHEN 'f' THEN 'cri'
                                                        WHEN 'm' THEN 'cop'
                                                        WHEN 'a' THEN 'txt'
                                                        WHEN 't' THEN 'txt'
                                                        WHEN 'c' THEN 'ntm'
                                                        WHEN 'd' THEN 'ntm'
                                                        WHEN 'j' THEN 'prm'
                                                        WHEN 'i' THEN 'spw'
                                                        WHEN 'k' THEN 'sti'
                                                        WHEN 'r' THEN 'tdf'
                                                        WHEN 'g' THEN 'tdi'
                                                        WHEN 'o' THEN 'xxx'
                                                        WHEN 'p' THEN 'xxx'
                                                        ELSE 'zzz'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '367' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, '2', 'rdacontent'
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = '2'
                                                WHERE f.TagNumber = '336' and s.FieldID is null and t.RecordID is not null;

                                            DELETE FROM TempUpdates;";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(337);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT RecordID, SUBSTR(ControlData, 1, 1)
                                                FROM Fields
                                                WHERE TagNumber = '007';

                                            INSERT INTO TempUpdates
                                                SELECT DISTINCT f.RecordID, ' '
                                                FROM Fields f
                                                LEFT OUTER JOIN Fields f2 ON f.RecordID = f2.RecordID AND f2.TagNumber = '007'
                                                WHERE f2.RecordID IS NULL;

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2)
                                                SELECT RecordID, '337', ' ', ' '
                                                FROM TempUpdates;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'a',
                                                    CASE t.Data
                                                        WHEN 's' THEN 'audio'
                                                        WHEN 'c' THEN 'computer'
                                                        WHEN 'h' THEN 'microform'
                                                        WHEN 'g' THEN 'projected'
                                                        WHEN 'm' THEN 'projected'
                                                        WHEN 't' THEN 'unmediated'
                                                        WHEN 'k' THEN 'unmediated'
                                                        WHEN ' ' THEN 'unmediated'
                                                        WHEN 'v' THEN 'video'
                                                        WHEN 'z' THEN 'unspecified'
                                                        ELSE 'other'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '337' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'b',
                                                    CASE t.Data
                                                        WHEN 's' THEN 's'
                                                        WHEN 'c' THEN 'c'
                                                        WHEN 'h' THEN 'h'
                                                        WHEN 'p' THEN 'p'
                                                        WHEN 'g' THEN 'g'
                                                        WHEN 'm' THEN 'g'
                                                        WHEN 't' THEN 'n'
                                                        WHEN 'k' THEN 'n'
                                                        WHEN ' ' THEN 'n'
                                                        WHEN 'v' THEN 'v'
                                                        WHEN 'z' THEN 'z'
                                                        ELSE 'x'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '337' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, '2', 'rdamedia'
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = '2'
                                                WHERE f.TagNumber = '337' and s.FieldID is null and t.RecordID is not null;

                                            DELETE FROM TempUpdates;";
                    command.ExecuteNonQuery();
                    
                    rdaConversionBackgroundWorker.ReportProgress(338);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT RecordID, SUBSTR(ControlData, 1, 2)
                                                FROM Fields
                                                WHERE TagNumber = '007';

                                            INSERT INTO TempUpdates
                                                SELECT DISTINCT f.RecordID, ' '
                                                FROM Fields f
                                                LEFT OUTER JOIN Fields f2 ON f.RecordID = f2.RecordID AND f2.TagNumber = '007'
                                                WHERE f2.RecordID IS NULL;

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2)
                                                SELECT RecordID, '338', ' ', ' '
                                                FROM TempUpdates;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'a',
                                                    CASE t.Data
                                                        WHEN 'sg' THEN 'audio cartridge'
                                                        WHEN 'se' THEN 'audio cylinder'
                                                        WHEN 'sd' THEN 'audio disc'
                                                        WHEN 'si' THEN 'sound track reel'
                                                        WHEN 'sq' THEN 'audio roll'
                                                        WHEN 'sw' THEN 'audio wire reel'
                                                        WHEN 'ss' THEN 'audiocassette'
                                                        WHEN 'st' THEN 'audiotape reel'
                                                        WHEN 'sz' THEN 'other'
                                                        WHEN 'ck' THEN 'computer card'
                                                        WHEN 'cb' THEN 'computer chip cartridge'
                                                        WHEN 'cd' THEN 'computer disc'
                                                        WHEN 'ce' THEN 'computer disc cartridge'
                                                        WHEN 'ca' THEN 'computer tape cartridge'
                                                        WHEN 'cf' THEN 'computer tape cassette'
                                                        WHEN 'ch' THEN 'computer tape reel'
                                                        WHEN 'cr' THEN 'online resource'
                                                        WHEN 'cz' THEN 'other'
                                                        WHEN 'ha' THEN 'aperture card'
                                                        WHEN 'he' THEN 'microfiche'
                                                        WHEN 'hf' THEN 'microfiche cassette'
                                                        WHEN 'hb' THEN 'microfilm cartridge'
                                                        WHEN 'hc' THEN 'microfilm cassette'
                                                        WHEN 'hd' THEN 'microfilm reel'
                                                        WHEN 'hj' THEN 'microfilm roll'
                                                        WHEN 'hh' THEN 'microfilm slip'
                                                        WHEN 'hg' THEN 'microopaque'
                                                        WHEN 'hz' THEN 'other'
                                                        WHEN 'gd' THEN 'film cartridge'
                                                        WHEN 'gf' THEN 'film cassette'
                                                        WHEN 'gc' THEN 'film reel'
                                                        WHEN 'gt' THEN 'film roll'
                                                        WHEN 'gs' THEN 'filmslip'
                                                        WHEN 'mc' THEN 'filmstrip'
                                                        WHEN 'mf' THEN 'filmstrip cartridge'
                                                        WHEN 'mr' THEN 'overhead transparency'
                                                        WHEN 'mo' THEN 'slide'
                                                        WHEN 'mz' THEN 'other'
                                                        WHEN 'vc' THEN 'video cartridge'
                                                        WHEN 'vf' THEN 'videocassette'
                                                        WHEN 'vd' THEN 'videodisc'
                                                        WHEN 'vr' THEN 'videotape reel'
                                                        WHEN 'vz' THEN 'other'
                                                        ELSE 'unspecified'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, 'b',
                                                    CASE t.Data
                                                        WHEN 'sg' THEN 'sg'
                                                        WHEN 'se' THEN 'se'
                                                        WHEN 'sd' THEN 'sd'
                                                        WHEN 'si' THEN 'si'
                                                        WHEN 'sq' THEN 'sq'
                                                        WHEN 'sw' THEN 'sw'
                                                        WHEN 'ss' THEN 'ss'
                                                        WHEN 'st' THEN 'st'
                                                        WHEN 'sz' THEN 'sz'
                                                        WHEN 'ck' THEN 'ck'
                                                        WHEN 'cb' THEN 'cb'
                                                        WHEN 'cd' THEN 'cd'
                                                        WHEN 'ce' THEN 'ce'
                                                        WHEN 'ca' THEN 'ca'
                                                        WHEN 'cf' THEN 'cf'
                                                        WHEN 'ch' THEN 'ch'
                                                        WHEN 'cr' THEN 'cr'
                                                        WHEN 'cz' THEN 'cz'
                                                        WHEN 'ha' THEN 'ha'
                                                        WHEN 'he' THEN 'he'
                                                        WHEN 'hf' THEN 'hf'
                                                        WHEN 'hb' THEN 'hb'
                                                        WHEN 'hc' THEN 'hc'
                                                        WHEN 'hd' THEN 'hd'
                                                        WHEN 'hj' THEN 'hj'
                                                        WHEN 'hh' THEN 'hh'
                                                        WHEN 'hg' THEN 'hg'
                                                        WHEN 'hz' THEN 'hz'
                                                        WHEN 'gd' THEN 'gd'
                                                        WHEN 'gf' THEN 'gf'
                                                        WHEN 'gc' THEN 'gc'
                                                        WHEN 'gt' THEN 'gt'
                                                        WHEN 'gs' THEN 'gs'
                                                        WHEN 'mc' THEN 'mc'
                                                        WHEN 'mf' THEN 'mf'
                                                        WHEN 'mr' THEN 'mr'
                                                        WHEN 'mo' THEN 'mo'
                                                        WHEN 'mz' THEN 'mz'
                                                        WHEN 'vc' THEN 'vc'
                                                        WHEN 'vf' THEN 'vf'
                                                        WHEN 'vd' THEN 'vd'
                                                        WHEN 'vr' THEN 'vr'
                                                        WHEN 'vz' THEN 'vz'
                                                        ELSE 'zu'
                                                    END
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data)
                                                SELECT f.FieldID, '2', 'rdacarrier'
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = '2'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            DELETE FROM TempUpdates;";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(600);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(s.Data, ' (ca.)', 'approximately'), 'ca.', 'approximately'), 'b.', 'born'), 'd.', 'died'), 'fl.', 'flourished')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data LIKE '% ca\.%' ESCAPE '\' or s.Data LIKE '%b\.' ESCAPE '\' or s.Data LIKE '%d\.' ESCAPE '\' or s.Data LIKE '%fl\.' ESCAPE '\');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "600";
                    command.Parameters.Add("Code", DbType.String).Value = "d";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(700);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(s.Data, 'ill.', 'illustrator'), 'ed.', 'editor'), 'comp.', 'compiler'), 'auth.', 'author'), 'adapt.', 'author'), 'adp.', 'author'), 'arr.', 'arranger of music'), 'jt. ', '')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and s.Data LIKE '% et al\.%' ESCAPE '\';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "700";
                    command.Parameters.Add("Code", DbType.String).Value = "e";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(710);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(s.Data, 'ill.', 'illustrator')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and s.Data LIKE '% et al\.%' ESCAPE '\';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "710";
                    command.Parameters.Add("Code", DbType.String).Value = "e";
                    command.ExecuteNonQuery();
                    
                    rdaConversionBackgroundWorker.ReportProgress(-1);

                    RebuildRecordsPreviewInformation();
                }
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the rdaConversionBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void rdaConversionBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
                progressToolStripStatusLabel.Text = rebuildingPreview;
            else if (e.ProgressPercentage == 0)
                progressToolStripStatusLabel.Text = "Converting Leader...";
            else
                progressToolStripStatusLabel.Text = "Converting tag #" + e.ProgressPercentage.ToString().PadLeft(3, '0') + "...";
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the rdaConversionBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void rdaConversionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            cancelButtonToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            recordsDataGridView.DataSource = marcDataSet.Tables["Records"];
            recordsDataGridView.ResumeLayout();
            loading = false;
            EnableForm();
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

                    printDocument.PrinterSettings = printDialog.PrinterSettings;
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
                    
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
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

        #region Reports

        /// <summary>
        /// Handles the Click event of the recordSummaryToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void recordSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ReportForm form = new ReportForm())
            {
                form.Report = "CSharp_MARC_Editor.Reports.RecordsReport.rdlc";
                form.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("RecordsDataTable", marcDataSet.Tables["Records"]));
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the copyrightDateToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void copyrightDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ReportForm form = new ReportForm())
            {
                form.Report = "CSharp_MARC_Editor.Reports.CopyrightDateReport.rdlc";
                form.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("CopyrightDateDataTable", marcDataSet.Tables["Records"]));
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the classificationsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void classificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ReportForm form = new ReportForm())
            {
                form.Report = "CSharp_MARC_Editor.Reports.ClassificationReport.rdlc";
                form.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("ClassificationDataTable", marcDataSet.Tables["Records"]));
                form.ShowDialog();
            }
        }

        private void copyrightDateByDecadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ReportForm form = new ReportForm())
            {
                form.Report = "CSharp_MARC_Editor.Reports.CopyrightDateDecadesReport.rdlc";
                form.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("CopyrightDateDataTable", marcDataSet.Tables["Records"]));
                form.ShowDialog();
            }
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
                DisableForm();
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar.MarqueeAnimationSpeed = 30;
                toolStripProgressBar.Enabled = true;
                toolStripProgressBar.Visible = true;
                progressToolStripStatusLabel.Visible = true;
                recordsDataGridView.SuspendLayout();
                recordsDataGridView.DataSource = null;
                SaveOptions();
                object[] parameters = { null, "customOnly" };
                rebuildBackgroundWorker.RunWorkerAsync(parameters);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the rebuildBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void rebuildBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            rebuildBackgroundWorker.ReportProgress(0);
            string rebuildField = null;
            int? rebuildRecord = null;

            if (e.Argument != null)
            {
                rebuildRecord = (int?)((object[])e.Argument)[0];
                rebuildField = ((object[])e.Argument)[1].ToString();
                rebuildingID = rebuildRecord;
            }

            RebuildRecordsPreviewInformation(rebuildRecord, rebuildField);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the rebuildBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void rebuildBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressToolStripStatusLabel.Text = rebuildingPreview;
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the rebuildBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void rebuildBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (rebuildingID == null)
                this.OnLoad(new EventArgs());
            else
                ReloadRecordRow(recordsDataGridView.SelectedCells[0].OwningRow);

            rebuildingID = null;
            progressToolStripStatusLabel.Text = "";
            toolStripProgressBar.Visible = false;
            toolStripProgressBar.Enabled = false;
            progressToolStripStatusLabel.Visible = false;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;
            loading = false;
            EnableForm();
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
                DisableForm();

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
                EnableForm();
            }
        }

        /// <summary>
        /// Handles the Click event of the resetDatabaseToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisableForm();
            ResetDatabase();

            this.OnLoad(new EventArgs());
            EnableForm();
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
