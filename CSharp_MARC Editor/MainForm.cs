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
                string query = "SELECT * FROM Fields where RecordiD = @RecordID ORDER BY CASE WHEN TagNumber = 'LDR' THEN 0 ELSE 1 END, Sort, TagNumber, FieldID";
                
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
                string query = "SELECT * FROM Subfields where FieldID = @FieldID ORDER BY Sort, Code, SubfieldID";

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
                using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY CASE WHEN TagNumber = 'LDR' THEN 0 ELSE 1 END, Sort, TagNumber, FieldID", fieldsConnection))
                {
                    fieldsCommand.Connection.Open();
                    fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);

                    using (SQLiteConnection subfieldsConnection = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY Sort, Code, SubfieldID", subfieldsConnection))
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
                                        record.Fields.Add(controlField);
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
                                                dataField.Subfields.Add(new Subfield(subfieldReader["Code"].ToString()[0], subfieldReader["Data"].ToString()));
                                            }
                                        }

                                        record.Fields.Add(dataField);
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
                                                    [Sort] integer, 
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
                                                    [ImportErrors] nvarchar(2147483647), 
                                                    [ValidationErrors] nvarchar(2147483647));

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
                                                    [Sort] integer, 
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

        /// <summary>
        /// Adds the array parameters.
        /// Thanks to http://stackoverflow.com/questions/2377506/pass-array-parameter-in-sqlcommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        public static void AddArrayParameters<T>(SQLiteCommand cmd, string name, IEnumerable<T> values)
        {
            name = name.StartsWith("@") ? name : "@" + name;
            var names = string.Join(", ", values.Select((value, i) =>
            {
                var paramName = name + i;
                cmd.Parameters.AddWithValue(paramName, value);
                return paramName;
            }));
            cmd.CommandText = cmd.CommandText.Replace(name, names);
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

                        int fieldNumber = 1;
                        foreach (Field field in record.Fields)
                        {
                            command.CommandText = "INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, ControlData) VALUES (@RecordID, @TagNumber, @Ind1, @Ind2, @ControlData)";
                            command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;
                            command.Parameters.Add("@TagNumber", DbType.String).Value = field.Tag;

                            fieldNumber++;

                            if (field.IsDataField())
                            {
                                command.Parameters.Add("@Ind1", DbType.String).Value = ((DataField)field).Indicator1;
                                command.Parameters.Add("@Ind2", DbType.String).Value = ((DataField)field).Indicator2;
                                command.Parameters.Add("@ControlData", DbType.String).Value = DBNull.Value;
                                
                                command.ExecuteNonQuery();
                                
                                int fieldID = (int)connection.LastInsertRowId;

                                int subfieldNumber = 0;
                                foreach (Subfield subfield in ((DataField)field).Subfields)
                                {
                                    command.CommandText = "INSERT INTO Subfields (FieldID, Code, Data, Sort) VALUES (@FieldID, @Code, @Data, @Sort)";
                                    command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                                    command.Parameters.Add("@Code", DbType.String).Value = subfield.Code;
                                    command.Parameters.Add("@Data", DbType.String).Value = subfield.Data;
                                    command.Parameters.Add("@Sort", DbType.Int32).Value = subfieldNumber;
                                    command.ExecuteNonQuery();

                                    subfieldNumber++;
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
                using (SQLiteCommand fieldsCommand = new SQLiteCommand("SELECT * FROM Fields WHERE RecordID = @RecordID ORDER BY CASE WHEN TagNumber = 'LDR' THEN 0 ELSE 1 END, Sort, TagNumber. FieldID", fieldsConnection))
                {
                    fieldsCommand.Connection.Open();
                    fieldsCommand.Parameters.Add("@RecordID", DbType.Int32);
                    using (SQLiteConnection subfieldsConnection = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand subfieldsCommand = new SQLiteCommand("SELECT * FROM Subfields WHERE FieldID = @FieldID ORDER BY Sort, Code, SubfieldID", subfieldsConnection))
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
                                            record.Fields.Add(controlField);
                                        }
                                        else
                                        {
                                            DataField dataField = new DataField(fieldsReader["TagNumber"].ToString(), new List<Subfield>(), fieldsReader["Ind1"].ToString()[0], fieldsReader["Ind2"].ToString()[0]);
                                            subfieldsCommand.Parameters["@FieldID"].Value = fieldsReader["FieldID"];

                                            using (SQLiteDataReader subfieldReader = subfieldsCommand.ExecuteReader())
                                            {
                                                while (subfieldReader.Read())
                                                {
                                                    dataField.Subfields.Add(new Subfield(subfieldReader["Code"].ToString()[0], subfieldReader["Data"].ToString()));
                                                }
                                            }

                                            record.Fields.Add(dataField);
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

                using (SQLiteCommand command = new SQLiteCommand("SELECT DISTINCT TagNumber FROM Fields ORDER BY CASE WHEN TagNumber = 'LDR' THEN 0 ELSE 1 END, Sort, TagNumber, FieldID", connection))
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
                            MessageBox.Show("The 005 field is updated automatically WHEN t.Data = changes are made.", "Field 005 is locked.", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("The 005 field is updated automatically WHEN t.Data = changes are made.", "Field 005 is locked.", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    int sort = e.RowIndex;

                    if (e.RowIndex > 0)
                        sort = Int32.Parse(subfieldsDataGridView.Rows[e.RowIndex - 1].Cells[4].Value.ToString());

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Subfields (FieldID, Code, Data, Sort) VALUES (@FieldID, @Code, @Data, @Sort)";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                            command.Parameters.Add("@Code", DbType.String).Value = code;
                            command.Parameters.Add("@Data", DbType.String).Value = data;
                            command.Parameters.Add("@Sort", DbType.Int32).Value = sort;

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
            DisableForm();
            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar.MarqueeAnimationSpeed = 30;
            toolStripProgressBar.Enabled = true;
            toolStripProgressBar.Visible = true;
            progressToolStripStatusLabel.Visible = true;
            recordsDataGridView.SuspendLayout();
            recordsDataGridView.DataSource = null;
            validationBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles the DoWork event of the validationBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void validationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Records SET ValidationErrors = ''";
                    command.ExecuteNonQuery();

                    validationBackgroundWorker.ReportProgress(0, "Validating Tag Numbers...");
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT RecordID, 'Invalid Tag Number: ' || TagNumber || char(10)
                                                FROM Fields WHERE SUBSTR(TagNumber, 1, 1) != '9' AND TagNumber NOT IN (@list);

                                            UPDATE Records 
                                            SET ValidationErrors = ValidationErrors || (SELECT Data FROM TempUpdates WHERE Records.RecordID = TempUpdates.RecordID)
                                            WHERE RecordID IN (SELECT RecordID FROM TempUpdates);

                                            DELETE FROM TempUpdates;";
                    List<string> list = new List<string> {"LDR", "001", "003", "005", "006", "007", "008", "010", "013", "015", "016", "017", "018", "020", "022", "024", "025", "026", "027", "028", "030", "031", "032", "033", "034", "035", "036", "037", "038", "040", "041", "042", "043", "044", "045", "046", "047", "048", "050", "051", "052", "055", "060", "061", "066", "070", "071", "072", "074", "080", "082", "083", "084", "085", "086", "088", "091", "092", "093", "094", "095", "096", "097", "098", "099", "100", "110", "111", "130", "210", "222", "240", "242", "243", "245", "246", "247", "250", "254", "255", "256", "257", "258", "260", "263", "264", "270", "300", "306", "307", "310", "321", "336", "337", "338", "340", "342", "343", "344", "345", "346", "347", "348", "351", "352", "355", "357", "362", "363", "375", "366", "370", "377", "380", "381", "382", "383", "384", "385", "386", "388", "490", "500", "501", "502", "504", "505", "506", "507", "508", "510", "511", "513", "514", "515", "516", "518", "520", "521", "522", "524", "525", "526", "530", "533", "534", "535", "536", "538", "540", "541", "542", "544", "545", "546", "547", "550", "552", "555", "556", "561", "562", "563", "565", "567", "580", "581", "583", "584", "585", "586", "588", "590", "591", "592", "593", "594", "594", "596", "597", "598", "599", "600", "610", "611", "630", "648", "650", "651", "653", "654", "655", "656", "657", "658", "662", "691", "692", "693", "694", "695", "696", "697", "698", "699", "700", "710", "711", "720", "730", "740", "751", "752", "753", "754", "760", "762", "765", "767", "770", "772", "773", "774", "775", "776", "777", "780", "785", "786", "787", "800", "810", "811", "830", "841", "842", "843", "844", "845", "850", "852", "853", "854", "855", "856", "863", "864", "865", "866", "867", "868", "876", "877", "878", "880", "882", "883", "884", "886", "887" };
                    AddArrayParameters(command, "@list", list);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    
                    ValidateSubfieldCodes(command, "010", new List<string> { "a", "b", "z", "8" });
                    ValidateSubfieldCodes(command, "013", new List<string> { "a", "b", "c", "d", "e", "f", "6", "8" });
                    ValidateSubfieldCodes(command, "015", new List<string> { "a", "q", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "016", new List<string> { "a", "z", "2", "8" });
                    ValidateSubfieldCodes(command, "017", new List<string> { "a", "b", "d", "i", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "018", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "020", new List<string> { "a", "c", "q", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "022", new List<string> { "a", "l", "m", "y", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "024", new List<string> { "a", "c", "d", "q", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "025", new List<string> { "a", "8" });
                    ValidateSubfieldCodes(command, "026", new List<string> { "a", "b", "c", "d", "e", "2", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "027", new List<string> { "a", "q", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "028", new List<string> { "a", "b", "q", "6", "8" });
                    ValidateSubfieldCodes(command, "030", new List<string> { "a", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "031", new List<string> { "a", "b", "c", "d", "e", "g", "m", "n", "o", "p", "q", "r", "s", "t", "u", "y", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "032", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "033", new List<string> { "a", "b", "c", "p", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "034", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "r", "s", "t", "x", "z", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "035", new List<string> { "a", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "036", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "037", new List<string> { "a", "b", "c", "f", "g", "n", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "038", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "040", new List<string> { "a", "b", "c", "d", "e", "6", "8" });
                    ValidateSubfieldCodes(command, "041", new List<string> { "a", "b", "d", "e", "f", "g", "h", "j", "k", "m", "n", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "042", new List<string> { "a" });
                    ValidateSubfieldCodes(command, "043", new List<string> { "a", "b", "c", "0", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "044", new List<string> { "a", "b", "c", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "045", new List<string> { "a", "b", "c", "6", "8" });
                    ValidateSubfieldCodes(command, "046", new List<string> { "a", "b", "c", "d", "e", "j", "k", "l", "m", "n", "o", "p", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "047", new List<string> { "a", "2", "8" }); 
                    ValidateSubfieldCodes(command, "048", new List<string> { "a", "b", "2", "8" });
                    ValidateSubfieldCodes(command, "050", new List<string> { "a", "b", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "051", new List<string> { "a", "b", "c", "8" });
                    ValidateSubfieldCodes(command, "052", new List<string> { "a", "b", "d", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "055", new List<string> { "a", "b", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "060", new List<string> { "a", "b", "8" });
                    ValidateSubfieldCodes(command, "061", new List<string> { "a", "b", "c", "8" });
                    ValidateSubfieldCodes(command, "066", new List<string> { "a", "b", "c" });
                    ValidateSubfieldCodes(command, "070", new List<string> { "a", "b", "8" });
                    ValidateSubfieldCodes(command, "071", new List<string> { "a", "b", "c", "8" });
                    ValidateSubfieldCodes(command, "072", new List<string> { "a", "x", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "074", new List<string> { "a", "z", "8" });
                    ValidateSubfieldCodes(command, "080", new List<string> { "a", "b", "x", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "082", new List<string> { "a", "b", "m", "q", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "083", new List<string> { "a", "c", "m", "q", "y", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "084", new List<string> { "a", "b", "q", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "085", new List<string> { "a", "b", "c", "f", "r", "s", "t", "u", "v", "w", "y", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "086", new List<string> { "a", "z", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "088", new List<string> { "a", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "100", new List<string> { "a", "b", "x", "d", "e", "f", "g", "j", "k", "l", "n", "p", "q", "t", "u", "0", "4", "6", "8" });
                    ValidateSubfieldCodes(command, "110", new List<string> { "a", "b", "c", "d", "e", "f", "g", "k", "l", " n", "p", "t", "u", "0", "4", "6", "8" });
                    ValidateSubfieldCodes(command, "111", new List<string> { "a", "c", "d", "e", "f", "q", "j", "k", "l", "n", "p", "q", "t", "u", "0", "4", "6", "8" });
                    ValidateSubfieldCodes(command, "130", new List<string> { "a", "b", "f", "g", "k", "l", "m", "n", "o", "p", "r", "s", "t", "0", "6", "8" });
                    ValidateSubfieldCodes(command, "210", new List<string> { "a", "b", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "222", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "240", new List<string> { "a", "b", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "0", "6", "8" });
                    ValidateSubfieldCodes(command, "242", new List<string> { "a", "b", "c", "h", "n", "p", "y", "6", "8" });
                    ValidateSubfieldCodes(command, "243", new List<string> { "a", "d", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "6", "8" });
                    ValidateSubfieldCodes(command, "245", new List<string> { "a", "b", "c", "f", "g", "h", "k", "n", "p", "s", "6", "8" });
                    ValidateSubfieldCodes(command, "246", new List<string> { "a", "b", "f", "g", "h", "i", "n", "p", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "247", new List<string> { "a", "b", "f", "g", "h", "n", "p", "x", "6", "8" });
                    ValidateSubfieldCodes(command, "250", new List<string> { "a", "b", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "254", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "255", new List<string> { "a", "b", "c", "d", "e", "f", "g", "6", "8" });
                    ValidateSubfieldCodes(command, "256", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "257", new List<string> { "a", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "258", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "260", new List<string> { "a", "b", "c", "e", "f", "g", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "263", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "264", new List<string> { "a", "b", "c", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "270", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "z", "4", "6", "8" });
                    ValidateSubfieldCodes(command, "300", new List<string> { "a", "b", "c", "e", "f", "g", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "306", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "307", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "310", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "321", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "337", new List<string> { "a", "b", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "338", new List<string> { "a", "b", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "340", new List<string> { "a", "b", "c", "d", "e", "f", "h", "i", "j", "k", "m", "n", "o", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "342", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "343", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "6", "8" });
                    ValidateSubfieldCodes(command, "344", new List<string> { "a", "b", "c", "d", "", "f", "g", "h", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "345", new List<string> { "a", "b", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "346", new List<string> { "a", "b", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "347", new List<string> { "a", "b", "c", "d", "e", "f", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "348", new List<string> { "a", "b", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "351", new List<string> { "a", "b", "c", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "352", new List<string> { "a", "b", "c", "d", "e", "f", "g", "i", "q", "6", "8" });
                    ValidateSubfieldCodes(command, "355", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "j", "6", "8" });
                    ValidateSubfieldCodes(command, "357", new List<string> { "a", "b", "c", "g", "6", "8" });
                    ValidateSubfieldCodes(command, "362", new List<string> { "a", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "363", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "u", "v", "x", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "375", new List<string> { "a", "b", "c", "d", "e", "f", "g", "j", "k", "m", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "366", new List<string> { "a", "b", "c", "d", "e", "f", "g", "j", "k", "m", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "370", new List<string> { "c", "f", "g", "s", "t", "u", "v", "0", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "377", new List<string> { "a", "l", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "380", new List<string> { "a", "0", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "381", new List<string> { "a", "u", "v", "0", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "382", new List<string> { "a", "b", "d", "e", "n", "p", "r", "s", "t", "v", "0", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "383", new List<string> { "a", "b", "c", "d", "e", "2", "6", "8" });
                    ValidateSubfieldCodes(command, "384", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "385", new List<string> { "a", "b", "m", "n", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "386", new List<string> { "a", "b", "m", "n", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "388", new List<string> { "a", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "500", new List<string> { "a", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "501", new List<string> { "a", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "502", new List<string> { "a", "b", "c", "d", "g", "o", "6", "8" });
                    ValidateSubfieldCodes(command, "504", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "505", new List<string> { "a", "g", "r", "t", "u", "6", "8" });
                    ValidateSubfieldCodes(command, "506", new List<string> { "a", "b", "c", "d", "e", "f", "u", "2", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "507", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "508", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "510", new List<string> { "a", "b", "c", "u", "x", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "511", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "513", new List<string> { "a", "b", "6", "8" });
                    ValidateSubfieldCodes(command, "514", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "m", "u", "z", "6", "8"});
                    ValidateSubfieldCodes(command, "515", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "516", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "518", new List<string> { "a", "b", "o", "p", "0", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "520", new List<string> { "a", "b", "c", "u", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "521", new List<string> { "a", "b", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "522", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "524", new List<string> { "a", "2", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "525", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "526", new List<string> { "a", "b", "c", "d", "i", "x", "z", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "530", new List<string> { "a", "b", "c", "d", "u", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "533", new List<string> { "a", "b", "c", "d", "e", "f", "m", "n", "3", "5", "7", "6", "8" });
                    ValidateSubfieldCodes(command, "534", new List<string> { "a", "b", "c", "e", "f", "k", "l", "m", "n", "o", "p", "t", "x", "z", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "535", new List<string> { "a", "b", "c", "d", "g", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "536", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "6", "8" });
                    ValidateSubfieldCodes(command, "538", new List<string> { "a", "i", "u", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "540", new List<string> { "a", "b", "c", "d", "u", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "541", new List<string> { "a", "b", "c", "d", "e", "f", "h", "n", "o", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "542", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "u", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "544", new List<string> { "a", "b", "c", "d", "e", "n", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "545", new List<string> { "a", "b", "u", "6", "8" });
                    ValidateSubfieldCodes(command, "546", new List<string> { "a", "b", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "547", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "550", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "552", new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "u", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "555", new List<string> { "a", "b", "c", "d", "u", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "556", new List<string> { "a", "z", "6", "8" });
                    ValidateSubfieldCodes(command, "561", new List<string> { "a", "u", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "562", new List<string> { "a", "b", "c", "d", "e", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "563", new List<string> { "a", "u", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "565", new List<string> { "a", "b", "c", "d", "e", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "567", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "580", new List<string> { "a", "6", "8" });
                    ValidateSubfieldCodes(command, "581", new List<string> { "a", "z", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "583", new List<string> { "a", "b", "c", "d", "e", "f", "h", "i", "j", "k", "l", "n", "o", "u", "x", "z", "2", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "584", new List<string> { "a", "b", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "585", new List<string> { "a", "3", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "586", new List<string> { "a", "3", "6", "8" });
                    ValidateSubfieldCodes(command, "588", new List<string> { "a", "5", "6", "8" });
                    ValidateSubfieldCodes(command, "600", new List<string> {  });
                    ValidateSubfieldCodes(command, "610", new List<string> {  });
                    ValidateSubfieldCodes(command, "611", new List<string> {  });
                    ValidateSubfieldCodes(command, "630", new List<string> {  });
                    ValidateSubfieldCodes(command, "648", new List<string> {  });
                    ValidateSubfieldCodes(command, "650", new List<string> {  });
                    ValidateSubfieldCodes(command, "651", new List<string> {  });
                    ValidateSubfieldCodes(command, "653", new List<string> {  });
                    ValidateSubfieldCodes(command, "654", new List<string> {  });
                    ValidateSubfieldCodes(command, "655", new List<string> {  });
                    ValidateSubfieldCodes(command, "656", new List<string> {  });
                    ValidateSubfieldCodes(command, "657", new List<string> {  });
                    ValidateSubfieldCodes(command, "658", new List<string> {  });
                    ValidateSubfieldCodes(command, "662", new List<string> {  });
                    ValidateSubfieldCodes(command, "691", new List<string> {  });
                    ValidateSubfieldCodes(command, "692", new List<string> {  });
                    ValidateSubfieldCodes(command, "693", new List<string> {  });
                    ValidateSubfieldCodes(command, "694", new List<string> {  });
                    ValidateSubfieldCodes(command, "695", new List<string> {  });
                    ValidateSubfieldCodes(command, "696", new List<string> {  });
                    ValidateSubfieldCodes(command, "697", new List<string> {  });
                    ValidateSubfieldCodes(command, "698", new List<string> {  });
                    ValidateSubfieldCodes(command, "699", new List<string> {  });
                    ValidateSubfieldCodes(command, "700", new List<string> {  });
                    ValidateSubfieldCodes(command, "710", new List<string> {  });
                    ValidateSubfieldCodes(command, "711", new List<string> {  });
                    ValidateSubfieldCodes(command, "720", new List<string> {  });
                    ValidateSubfieldCodes(command, "730", new List<string> {  });
                    ValidateSubfieldCodes(command, "740", new List<string> {  });
                    ValidateSubfieldCodes(command, "751", new List<string> {  });
                    ValidateSubfieldCodes(command, "752", new List<string> {  });
                    ValidateSubfieldCodes(command, "753", new List<string> {  });
                    ValidateSubfieldCodes(command, "754", new List<string> {  });
                    ValidateSubfieldCodes(command, "760", new List<string> {  });
                    ValidateSubfieldCodes(command, "762", new List<string> {  });
                    ValidateSubfieldCodes(command, "765", new List<string> {  });
                    ValidateSubfieldCodes(command, "767", new List<string> {  });
                    ValidateSubfieldCodes(command, "770", new List<string> {  });
                    ValidateSubfieldCodes(command, "772", new List<string> {  });
                    ValidateSubfieldCodes(command, "773", new List<string> {  });
                    ValidateSubfieldCodes(command, "774", new List<string> {  });
                    ValidateSubfieldCodes(command, "775", new List<string> {  });
                    ValidateSubfieldCodes(command, "776", new List<string> {  });
                    ValidateSubfieldCodes(command, "777", new List<string> {  });
                    ValidateSubfieldCodes(command, "780", new List<string> {  });
                    ValidateSubfieldCodes(command, "785", new List<string> {  });
                    ValidateSubfieldCodes(command, "786", new List<string> {  });
                    ValidateSubfieldCodes(command, "787", new List<string> {  });
                    ValidateSubfieldCodes(command, "800", new List<string> {  });
                    ValidateSubfieldCodes(command, "810", new List<string> {  });
                    ValidateSubfieldCodes(command, "811", new List<string> {  });
                    ValidateSubfieldCodes(command, "830", new List<string> {  });
                    ValidateSubfieldCodes(command, "841", new List<string> {  });
                    ValidateSubfieldCodes(command, "842", new List<string> {  });
                    ValidateSubfieldCodes(command, "843", new List<string> {  });
                    ValidateSubfieldCodes(command, "844", new List<string> {  });
                    ValidateSubfieldCodes(command, "845", new List<string> {  });
                    ValidateSubfieldCodes(command, "850", new List<string> {  });
                    ValidateSubfieldCodes(command, "852", new List<string> {  });
                    ValidateSubfieldCodes(command, "853", new List<string> {  });
                    ValidateSubfieldCodes(command, "854", new List<string> {  });
                    ValidateSubfieldCodes(command, "855", new List<string> {  });
                    ValidateSubfieldCodes(command, "856", new List<string> {  });
                    ValidateSubfieldCodes(command, "863", new List<string> {  });
                    ValidateSubfieldCodes(command, "864", new List<string> {  });
                    ValidateSubfieldCodes(command, "865", new List<string> {  });
                    ValidateSubfieldCodes(command, "866", new List<string> {  });
                    ValidateSubfieldCodes(command, "867", new List<string> {  });
                    ValidateSubfieldCodes(command, "868", new List<string> {  });
                    ValidateSubfieldCodes(command, "876", new List<string> {  });
                    ValidateSubfieldCodes(command, "877", new List<string> {  });
                    ValidateSubfieldCodes(command, "878", new List<string> {  });
                    ValidateSubfieldCodes(command, "880", new List<string> {  });
                    ValidateSubfieldCodes(command, "882", new List<string> {  });
                    ValidateSubfieldCodes(command, "883", new List<string> {  });
                    ValidateSubfieldCodes(command, "884", new List<string> {  });
                    ValidateSubfieldCodes(command, "886", new List<string> {  });
                    ValidateSubfieldCodes(command, "887", new List<string> {  });

                    command.CommandText = "UPDATE Records SET ValidationErrors = SUBSTR(ValidationErrors, 1, LENGTH(ValidationErrors) - 1)";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Validates the subfield codes.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tagNumber">The tag number.</param>
        /// <param name="list">The list.</param>
        private void ValidateSubfieldCodes(SQLiteCommand command, string tagNumber, List<string> list)
        {
            validationBackgroundWorker.ReportProgress(0, "Validating " + tagNumber + " Subfield Codes...");

            command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT f.RecordID, 'Invalid Subfields: ' || s.Code || ' in tag ' || f.TagNumber || char(10)
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber AND Code NOT IN (@list);

                                            UPDATE Records
                                            SET ValidationErrors = ValidationErrors || (SELECT Data FROM TempUpdates WHERE Records.RecordID = TempUpdates.RecordID)
                                            WHERE RecordID IN (SELECT RecordID FROM TempUpdates);

                                            DELETE FROM TempUpdates;";

            command.Parameters.Add("@TagNumber", DbType.String).Value = tagNumber;
            AddArrayParameters(command, "@list", list);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        /// <summary>
        /// Handles the ProgressChanged event of the validationBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void validationBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressToolStripStatusLabel.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the validationBackgroundWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void validationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            this.OnLoad(new EventArgs());
            EnableForm();
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

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT RecordID, @Code, REPLACE(Data, ')', ''), (SELECT MAX(Sort) + 1 FROM Subfields WHERE FieldID = RecordID)
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
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'e'
                                                WHERE f.TagNumber = @TagNumber AND s.Data IS NULL;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT RecordID, @Code, 'rda', (SELECT MAX(Sort) + 1 FROM Subfields WHERE FieldID = RecordID AND (Code = 'a' OR Code = 'b'))
                                                FROM TempUpdates;

                                            UPDATE Subfields SET Sort = Sort + 1 
                                            WHERE FieldID IN (SELECT RecordID FROM TempUpdates) AND Code != 'a' AND Code != 'b' AND Code != 'e';

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

                                            DELETE FROM TempUpdates;
                                            
                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REGEXREPLACE(Data, '\[.*\]', '')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '245' and s.Code = 'h';

                                            UPDATE Subfields SET Data = Data || (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SUbfieldID)
                                            WHERE SubfieldID IN (SELECT SubfieldID FROM TempUpdates);

                                            DELETE FROM TempUpdates;

                                            DELETE FROM Subfields WHERE SubfieldID IN (SELECT SubfieldID FROM Subfields s LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID WHERE f.TagNumber = '245' and s.Code = 'h');";
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
                                                WHERE f.TagNumber = '260' and s.Code = 'c';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;

                                            UPDATE Fields SET TagNumber = '264', Ind2 = '4' WHERE TagNumber = '260';

                                            INSERT INTO TempUpdates
                                                SELECT f.FieldID, ''
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on f.FieldID = s.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '264' and s.Code IS NOT NULL;
                        
                                            UPDATE Fields
                                            SET Ind2 = '1'
                                            WHERE FieldID IN (Select RecordID FROM TempUpdates);
                                            
                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, s.Data
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.code = 'c'
                                                WHERE f.TagNumber = '264' and f.Ind2 = '1';

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, ControlData, Sort)
                                                SELECT f.RecordID, '264', ' ', '4', s.SubfieldID, (SELECT MAX(Sort) FROM Fields f2 WHERE f2.RecordID = f.RecordID AND f2.TagNumber = '264')
                                                FROM TempUpdates t
                                                LEFT OUTER JOIN Subfields s ON s.SubfieldID = t.RecordID
                                                LEFT OUTER JOIN Fields f ON f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' and f.Ind2 = '1';

                                            UPDATE Subfields SET Data = REPLACE(Data, '.', ''), FieldID = (SELECT FieldID FROM Fields WHERE TagNumber = '264' AND Ind1 = ' ' AND Ind2 = '4' AND ControlData = Subfields.SubfieldID)
                                                WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates);

                                            UPDATE Fields SET ControlData = null WHERE TagNumber = '264' AND Ind1 = ' ' AND Ind2 = '4';

                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, SUBSTR(s.Data, 1, LENGTH(s.Data) - 1)
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' and s.Code = 'b' and SUBSTR(s.Data, 1, LENGTH(s.Data) - 1) == ',';

                                            UPDATE Subfields SET Data = (SELECT Data FROM TempUpdates WHERE RecordID = SubfieldID)
                                                WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates);

                                            DELETE FROM TempUpdates;
                                            
                                            INSERT INTO TempUpdates
                                                SELECT f.RecordID, '©' || SUBSTR(f.ControlData, 12, 4)
                                                FROM Fields f
                                                WHERE f.TagNumber = '008' and SUBSTR(f.ControlData, 12, 4) != '    ';

                                            DELETE FROM TempUpdates WHERE RecordID IN (
                                                SELECT RecordID 
                                                FROM Fields f
                                                LEFT OUTER JOIN Subfields s on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' and s.Code = 'c' and s.Data LIKE '%' || TempUpdates.Data || '%'
                                            );

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, Sort)
                                                SELECT RecordID, '264', ' ', '4', (SELECT MAX(Sort) FROM Fields f WHERE f.RecordID = t.RecordID AND f.TagNumber = '264')
                                                FROM TempUpdates t;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'c', t.Data, 0
                                                FROM TempUpdates t
                                                LEFT OUTER JOIN Fields f on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' AND f.Ind1 = ' ' AND f.Ind2 = '4' AND s.Data IS NULL;

                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT SubfieldID, REPLACE(Data, 'S.l.', '[Place of publication not identified]')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' and s.Code = 'a';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT SubfieldID, REPLACE(Data, 's.n.', '[Publisher not identified]')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = '264' and s.Code = 'b';

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
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

                                            DELETE FROM TempUpdates;

                                            INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(Data, 'cm.', 'cm'), 'mm.', 'mm')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and s.Code = @Code and (s.Data LIKE '%cm\.' ESCAPE '\' or s.Data LIKE '%mm\.') and (SELECT COUNT(*) FROM Fields WHERE RecordID = f.RecordID and TagNumber = '490') > 0;

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
                                                SELECT RecordID, SUBSTR(ControlData, 7, 1)
                                                FROM Fields
                                                WHERE TagNumber = 'LDR';

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, Sort)
                                                SELECT t.RecordID, '336', ' ', ' ', (SELECT MAX(Sort) FROM Fields f2 WHERE f2.RecordID = f.RecordID AND f2.TagNumber < '336')
                                                FROM TempUpdates t
                                                LEFT OUTER JOIN Fields f on f.RecordID = t.RecordID AND f.TagNumber = '336'
                                                WHERE f.TagNumber IS NULL;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'a',
                                                    CASE t.Data
                                                        WHEN t.Data = 'e' THEN 'cartographic image'
                                                        WHEN t.Data = 'f' THEN 'cartographic image'
                                                        WHEN t.Data = 'm' THEN 'computer program'
                                                        WHEN t.Data = 'a' THEN 'text'
                                                        WHEN t.Data = 't' THEN 'text'
                                                        WHEN t.Data = 'c' THEN 'notated music'
                                                        WHEN t.Data = 'd' THEN 'notated music'
                                                        WHEN t.Data = 'j' THEN 'performed music'
                                                        WHEN t.Data = 'i' THEN 'spoken word'
                                                        WHEN t.Data = 'k' THEN 'still image'
                                                        WHEN t.Data = 'r' THEN 'three-dimensional form'
                                                        WHEN t.Data = 'g' THEN 'two-dimensional moving image'
                                                        WHEN t.Data = 'o' THEN 'other'
                                                        WHEN t.Data = 'p' THEN 'other'
                                                        ELSE 'unspecified'
                                                    END, 0
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '336' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'b',
                                                    CASE t.Data
                                                        WHEN t.Data = 'e' THEN 'cri'
                                                        WHEN t.Data = 'f' THEN 'cri'
                                                        WHEN t.Data = 'm' THEN 'cop'
                                                        WHEN t.Data = 'a' THEN 'txt'
                                                        WHEN t.Data = 't' THEN 'txt'
                                                        WHEN t.Data = 'c' THEN 'ntm'
                                                        WHEN t.Data = 'd' THEN 'ntm'
                                                        WHEN t.Data = 'j' THEN 'prm'
                                                        WHEN t.Data = 'i' THEN 'spw'
                                                        WHEN t.Data = 'k' THEN 'sti'
                                                        WHEN t.Data = 'r' THEN 'tdf'
                                                        WHEN t.Data = 'g' THEN 'tdi'
                                                        WHEN t.Data = 'o' THEN 'xxx'
                                                        WHEN t.Data = 'p' THEN 'xxx'
                                                        ELSE 'zzz'
                                                    END, 1
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '336' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, '2', 'rdacontent', 2
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
                                                WHERE RecordID NOT IN (SELECT RecordID FROM TempUpdates);

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, Sort)
                                                SELECT t.RecordID, '337', ' ', ' ', (SELECT MAX(Sort) FROM Fields f2 WHERE f2.RecordID = f.RecordID AND f2.TagNumber < '337')
                                                FROM TempUpdates t
                                                LEFT OUTER JOIN Fields f on f.TagNumber = '337'
                                                WHERE f.TagNumber IS NULL;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'a',
                                                    CASE t.Data
                                                        WHEN t.Data = 's' THEN 'audio'
                                                        WHEN t.Data = 'c' THEN 'computer'
                                                        WHEN t.Data = 'h' THEN 'microform'
                                                        WHEN t.Data = 'g' THEN 'projected'
                                                        WHEN t.Data = 'm' THEN 'projected'
                                                        WHEN t.Data = 't' THEN 'unmediated'
                                                        WHEN t.Data = 'k' THEN 'unmediated'
                                                        WHEN t.Data = ' ' THEN 'unmediated'
                                                        WHEN t.Data = 'v' THEN 'video'
                                                        WHEN t.Data = 'z' THEN 'unspecified'
                                                        ELSE 'other'
                                                    END, 0
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '337' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'b',
                                                    CASE t.Data
                                                        WHEN t.Data = 's' THEN 's'
                                                        WHEN t.Data = 'c' THEN 'c'
                                                        WHEN t.Data = 'h' THEN 'h'
                                                        WHEN t.Data = 'p' THEN 'p'
                                                        WHEN t.Data = 'g' THEN 'g'
                                                        WHEN t.Data = 'm' THEN 'g'
                                                        WHEN t.Data = 't' THEN 'n'
                                                        WHEN t.Data = 'k' THEN 'n'
                                                        WHEN t.Data = ' ' THEN 'n'
                                                        WHEN t.Data = 'v' THEN 'v'
                                                        WHEN t.Data = 'z' THEN 'z'
                                                        ELSE 'x'
                                                    END, 1
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '337' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, '2', 'rdamedia', 2
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

                                            INSERT INTO Fields (RecordID, TagNumber, Ind1, Ind2, Sort)
                                                SELECT t.RecordID, '338', ' ', ' ', (SELECT MAX(Sort) FROM Fields f2 WHERE f2.RecordID = f.RecordID AND f2.TagNumber < '338')
                                                FROM TempUpdates t
                                                LEFT OUTER JOIN Fields f on f.TagNumber = '338'
                                                WHERE f.TagNumber IS NULL;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'a',
                                                    CASE
                                                        WHEN (SELECT Data FROM Subfields s LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID WHERE f.TagNumber = '337' and s.Code = 'b') = 'n' THEN 'volume'
                                                        WHEN t.Data = 'sg' THEN 'audio cartridge'
                                                        WHEN t.Data = 'se' THEN 'audio cylinder'
                                                        WHEN t.Data = 'sd' THEN 'audio disc'
                                                        WHEN t.Data = 'si' THEN 'sound track reel'
                                                        WHEN t.Data = 'sq' THEN 'audio roll'
                                                        WHEN t.Data = 'sw' THEN 'audio wire reel'
                                                        WHEN t.Data = 'ss' THEN 'audiocassette'
                                                        WHEN t.Data = 'st' THEN 'audiotape reel'
                                                        WHEN t.Data = 'sz' THEN 'other'
                                                        WHEN t.Data = 'ck' THEN 'computer card'
                                                        WHEN t.Data = 'cb' THEN 'computer chip cartridge'
                                                        WHEN t.Data = 'cd' THEN 'computer disc'
                                                        WHEN t.Data = 'ce' THEN 'computer disc cartridge'
                                                        WHEN t.Data = 'ca' THEN 'computer tape cartridge'
                                                        WHEN t.Data = 'cf' THEN 'computer tape cassette'
                                                        WHEN t.Data = 'ch' THEN 'computer tape reel'
                                                        WHEN t.Data = 'cr' THEN 'online resource'
                                                        WHEN t.Data = 'cz' THEN 'other'
                                                        WHEN t.Data = 'ha' THEN 'aperture card'
                                                        WHEN t.Data = 'he' THEN 'microfiche'
                                                        WHEN t.Data = 'hf' THEN 'microfiche cassette'
                                                        WHEN t.Data = 'hb' THEN 'microfilm cartridge'
                                                        WHEN t.Data = 'hc' THEN 'microfilm cassette'
                                                        WHEN t.Data = 'hd' THEN 'microfilm reel'
                                                        WHEN t.Data = 'hj' THEN 'microfilm roll'
                                                        WHEN t.Data = 'hh' THEN 'microfilm slip'
                                                        WHEN t.Data = 'hg' THEN 'microopaque'
                                                        WHEN t.Data = 'hz' THEN 'other'
                                                        WHEN t.Data = 'gd' THEN 'film cartridge'
                                                        WHEN t.Data = 'gf' THEN 'film cassette'
                                                        WHEN t.Data = 'gc' THEN 'film reel'
                                                        WHEN t.Data = 'gt' THEN 'film roll'
                                                        WHEN t.Data = 'gs' THEN 'filmslip'
                                                        WHEN t.Data = 'mc' THEN 'filmstrip'
                                                        WHEN t.Data = 'mf' THEN 'filmstrip cartridge'
                                                        WHEN t.Data = 'mr' THEN 'overhead transparency'
                                                        WHEN t.Data = 'mo' THEN 'slide'
                                                        WHEN t.Data = 'mz' THEN 'other'
                                                        WHEN t.Data = 'vc' THEN 'video cartridge'
                                                        WHEN t.Data = 'vf' THEN 'videocassette'
                                                        WHEN t.Data = 'vd' THEN 'videodisc'
                                                        WHEN t.Data = 'vr' THEN 'videotape reel'
                                                        WHEN t.Data = 'vz' THEN 'other'
                                                        ELSE 'unspecified'
                                                    END, 0
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'a'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, 'b',
                                                    CASE
                                                        WHEN (SELECT Data FROM Subfields s LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID WHERE f.TagNumber = '337' and s.Code = 'b') = 'n' THEN 'nc'
                                                        WHEN t.Data = 'sg' THEN 'sg'
                                                        WHEN t.Data = 'se' THEN 'se'
                                                        WHEN t.Data = 'sd' THEN 'sd'
                                                        WHEN t.Data = 'si' THEN 'si'
                                                        WHEN t.Data = 'sq' THEN 'sq'
                                                        WHEN t.Data = 'sw' THEN 'sw'
                                                        WHEN t.Data = 'ss' THEN 'ss'
                                                        WHEN t.Data = 'st' THEN 'st'
                                                        WHEN t.Data = 'sz' THEN 'sz'
                                                        WHEN t.Data = 'ck' THEN 'ck'
                                                        WHEN t.Data = 'cb' THEN 'cb'
                                                        WHEN t.Data = 'cd' THEN 'cd'
                                                        WHEN t.Data = 'ce' THEN 'ce'
                                                        WHEN t.Data = 'ca' THEN 'ca'
                                                        WHEN t.Data = 'cf' THEN 'cf'
                                                        WHEN t.Data = 'ch' THEN 'ch'
                                                        WHEN t.Data = 'cr' THEN 'cr'
                                                        WHEN t.Data = 'cz' THEN 'cz'
                                                        WHEN t.Data = 'ha' THEN 'ha'
                                                        WHEN t.Data = 'he' THEN 'he'
                                                        WHEN t.Data = 'hf' THEN 'hf'
                                                        WHEN t.Data = 'hb' THEN 'hb'
                                                        WHEN t.Data = 'hc' THEN 'hc'
                                                        WHEN t.Data = 'hd' THEN 'hd'
                                                        WHEN t.Data = 'hj' THEN 'hj'
                                                        WHEN t.Data = 'hh' THEN 'hh'
                                                        WHEN t.Data = 'hg' THEN 'hg'
                                                        WHEN t.Data = 'hz' THEN 'hz'
                                                        WHEN t.Data = 'gd' THEN 'gd'
                                                        WHEN t.Data = 'gf' THEN 'gf'
                                                        WHEN t.Data = 'gc' THEN 'gc'
                                                        WHEN t.Data = 'gt' THEN 'gt'
                                                        WHEN t.Data = 'gs' THEN 'gs'
                                                        WHEN t.Data = 'mc' THEN 'mc'
                                                        WHEN t.Data = 'mf' THEN 'mf'
                                                        WHEN t.Data = 'mr' THEN 'mr'
                                                        WHEN t.Data = 'mo' THEN 'mo'
                                                        WHEN t.Data = 'mz' THEN 'mz'
                                                        WHEN t.Data = 'vc' THEN 'vc'
                                                        WHEN t.Data = 'vf' THEN 'vf'
                                                        WHEN t.Data = 'vd' THEN 'vd'
                                                        WHEN t.Data = 'vr' THEN 'vr'
                                                        WHEN t.Data = 'vz' THEN 'vz'
                                                        ELSE 'zu'
                                                    END, 1
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = 'b'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            INSERT INTO Subfields (FieldID, Code, Data, Sort)
                                                SELECT f.FieldID, '2', 'rdacarrier', 2
                                                FROM Fields f
                                                LEFT OUTER JOIN TempUpdates t on t.RecordID = f.RecordID
                                                LEFT OUTER JOIN Subfields s on s.FieldID = f.FieldID and s.Code = '2'
                                                WHERE f.TagNumber = '338' and s.FieldID is null and t.RecordID is not null;

                                            DELETE FROM TempUpdates;";
                    command.ExecuteNonQuery();

                    rdaConversionBackgroundWorker.ReportProgress(500);
                    command.CommandText = @"INSERT INTO TempUpdates
                                                SELECT s.SubfieldID, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(s.Data, 'T.p.', 'Title page'), 't.p.', 'title page'), 'P.', 'Page'), 'p.', 'page'), 'Vol.', 'Volume'), 'vol.', 'volume')
                                                FROM Subfields s
                                                LEFT OUTER JOIN Fields f on f.FieldID = s.FieldID
                                                WHERE f.TagNumber = @TagNumber and (s.Data LIKE '%t\.p\.%' ESCAPE '\' or s.Data LIKE '%p\.%' ESCAPE '\' or s.Data LIKE '%vol\.%' ESCAPE '\');

                                            UPDATE Subfields
                                            SET Data = (SELECT Data FROM TempUpdates WHERE TempUpdates.RecordID = Subfields.SubfieldID)
                                            WHERE SubfieldID IN (SELECT RecordID FROM TempUpdates); 

                                            DELETE FROM TempUpdates;";
                    command.Parameters.Add("TagNumber", DbType.String).Value = "500";
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

        #region Sorting rows

        /// <summary>
        /// Handles the Click event of the fieldUpButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void fieldUpButton_Click(object sender, EventArgs e)
        {
            if (fieldsDataGridView.SelectedCells.Count > 0)
            {
                int index = fieldsDataGridView.SelectedCells[0].OwningRow.Index;
                int fieldID = Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                int otherFieldID = Int32.Parse(fieldsDataGridView.Rows[index - 1].Cells[0].Value.ToString());

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Fields SET Sort = Sort - 1 WHERE FieldID = @FieldID";
                        command.Parameters.Add("@SubfieldID", DbType.Int32).Value = fieldID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Fields SET Sort = Sort + 1 WHERE FieldID = @FieldID";
                        command.Parameters["@FieldID"].Value = otherFieldID;
                        command.ExecuteNonQuery();

                        LoadSubfields(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));

                        fieldsDataGridView.ClearSelection();
                        fieldsDataGridView.Rows[index - 1].Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the fieldDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void fieldDownButton_Click(object sender, EventArgs e)
        {
            int index = fieldsDataGridView.SelectedCells[0].OwningRow.Index;
            int fieldID = Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
            int otherFieldID = Int32.Parse(fieldsDataGridView.Rows[index - 1].Cells[0].Value.ToString());

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Fields SET Sort = Sort + 1 WHERE FieldID = @FieldID";
                    command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE Fields SET Sort = Sort - 1 WHERE FieldID = @FieldID";
                    command.Parameters["@FieldID"].Value = otherFieldID;
                    command.ExecuteNonQuery();

                    LoadSubfields(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));

                    fieldsDataGridView.ClearSelection();
                    fieldsDataGridView.Rows[index + 1].Selected = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the fieldsSortButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void fieldsSortButton_Click(object sender, EventArgs e)
        {
            if (recordsDataGridView.SelectedCells.Count > 0)
            {
                int recordID = Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                List<int> fields = new List<int>();
                int i = 0;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT RecordID FROM Fields WHERE RecordID = @RecordID ORDER BY CASE WHEN TagNumber = 'LDR' THEN 0 ELSE 1 END, Sort, TagNumber, FieldID";
                        command.Parameters.Add("@RecordID", DbType.Int32).Value = recordID;

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fields.Add(Int32.Parse(reader["FieldID"].ToString()));
                            }
                        }

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE Fields SET Sort = @Sort WHERE FieldID = @FieldID";
                        command.Parameters.Add("@FieldID", DbType.Int32);
                        command.Parameters.Add("@Sort", DbType.Int32);

                        foreach (int fieldID in fields)
                        {
                            command.Parameters["@FieldID"].Value = fieldID;
                            command.Parameters["@Sort"].Value = i;
                            command.ExecuteNonQuery();

                            i++;
                        }

                        LoadFields(Int32.Parse(recordsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the subfieldUpButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void subfieldUpButton_Click(object sender, EventArgs e)
        {
            if (subfieldsDataGridView.SelectedCells.Count > 0)
            {
                int index = subfieldsDataGridView.SelectedCells[0].OwningRow.Index;
                int subfieldID = Int32.Parse(subfieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                int otherSubfieldID = Int32.Parse(subfieldsDataGridView.Rows[index - 1].Cells[0].Value.ToString());

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Subfields SET Sort = Sort - 1 WHERE SubfieldID = @SubfieldID";
                        command.Parameters.Add("@SubfieldID", DbType.Int32).Value = subfieldID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Subfields SET Sort = Sort + 1 WHERE SubfieldID = @SubfieldID";
                        command.Parameters["@SubfieldID"].Value = otherSubfieldID;
                        command.ExecuteNonQuery();

                        LoadSubfields(Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));

                        subfieldsDataGridView.ClearSelection();
                        subfieldsDataGridView.Rows[index - 1].Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the subfieldDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void subfieldDownButton_Click(object sender, EventArgs e)
        {
            if (subfieldsDataGridView.SelectedCells.Count > 0)
            {
                int index = subfieldsDataGridView.SelectedCells[0].OwningRow.Index;
                int fieldID = Int32.Parse(subfieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                int otherFieldID = Int32.Parse(subfieldsDataGridView.Rows[index + 1].Cells[0].Value.ToString());

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Subfields SET Sort = Sort + 1 WHERE SubfieldID = @SubfieldID";
                        command.Parameters.Add("@SubfieldID", DbType.Int32).Value = fieldID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Subfields SET Sort = Sort - 1 WHERE SubfieldID = @SubfieldID";
                        command.Parameters["@SubfieldID"].Value = otherFieldID;
                        command.ExecuteNonQuery();

                        LoadSubfields(Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));

                        subfieldsDataGridView.ClearSelection();
                        subfieldsDataGridView.Rows[index + 1].Selected = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles the Click event of the subfieldSortButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void subfieldSortButton_Click(object sender, EventArgs e)
        {
            if (fieldsDataGridView.SelectedCells.Count > 0)
            {
                int fieldID = Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                List<int> subfields = new List<int>();
                int i = 0;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT SubfieldID FROM Subfields WHERE FieldID = @FieldID ORDER BY CASE WHEN Code >= '0' AND Code < '9' THEN 1 ELSE 0 END, Code";
                        command.Parameters.Add("@FieldID", DbType.Int32).Value = fieldID;
                        
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subfields.Add(Int32.Parse(reader["SubfieldID"].ToString()));
                            }
                        }

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE Subfields SET Sort = @Sort WHERE SubfieldID = @SubfieldID";
                        command.Parameters.Add("@SubfieldID", DbType.Int32);
                        command.Parameters.Add("@Sort", DbType.Int32);

                        foreach (int subfieldID in subfields)
                        {
                            command.Parameters["@SubfieldID"].Value = subfieldID;
                            command.Parameters["@Sort"].Value = i;
                            command.ExecuteNonQuery();

                            i++;
                        }

                        LoadSubfields(Int32.Parse(fieldsDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString()));
                    }
                }
            }
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
