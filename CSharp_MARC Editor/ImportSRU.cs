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
 * @author    Matt Schraeder <mschraeder@csharpmarc.net> <mschraeder@btsb.com>
 * @copyright 2016 Matt Schraeder and Bound to Stay Bound Books
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
using System.IO;
using System.Xml;
using System.Collections;
using System.Net;
using System.Xml.Linq;
using System.Diagnostics;

namespace CSharp_MARC_Editor
{
	public partial class ImportSRU : Form
	{
		#region Public Members

		/// <summary>
		/// Gets the selected record.
		/// </summary>
		public List<Record> SelectedRecords
		{
			get 
            {
                List<Record> selectedRecords = new List<Record>();
                
                foreach (DataGridViewRow row in searchResultsDataGridView.SelectedRows)
                    selectedRecords.Add((Record)row.Tag);

                return selectedRecords;
            }
		}

		#endregion

		public ImportSRU()
		{
			InitializeComponent();
		}

		#region Utility Functions

		/// <summary>
		/// Adds the result.
		/// </summary>
		/// <param name="importedRecord">The imported record.</param>
		private void AddResult(Record importedRecord)
		{
			AddResult(importedRecord, false);
		}

		/// <summary>
		/// Adds the result.
		/// </summary>
		/// <param name="importedRecord">The imported record.</param>
		/// <param name="colorRow">if set to <c>true</c> [color row].</param>
		private void AddResult(Record importedRecord, bool colorRow)
		{
			//Build the title/author display for the search results
			string title = string.Empty;
			string author = string.Empty;

			DataField titleField = (DataField)importedRecord["245"];
			DataField authorField = (DataField)importedRecord["100"];

			if (titleField != null)
			{
				foreach (Subfield subfield in titleField.Subfields)
				{
					title += subfield.Data;
				}
			}

			if (authorField != null)
			{
				foreach (Subfield subfield in authorField.Subfields)
				{
					author += subfield.Data;
				}
			}

			//Add the search result
			int rowNum = searchResultsDataGridView.Rows.Add(title, author);
			searchResultsDataGridView.Rows[rowNum].Tag = importedRecord;

			if (colorRow)
				searchResultsDataGridView.Rows[rowNum].DefaultCellStyle.BackColor = Color.LightBlue;
		}

		/// <summary>
		/// Sorts the results.
		/// </summary>
		private void SortResults()
		{
			//Sort the results and select the top one
			if (searchResultsDataGridView.Rows.Count > 0)
			{
				searchResultsDataGridView.Sort(titleColumn, ListSortDirection.Ascending);
				searchResultsDataGridView.Rows[0].Selected = true;
				searchResultsDataGridView.CurrentCell = searchResultsDataGridView.SelectedCells[0];
				EnableImport();
			}
		}

		/// <summary>
		/// Disables the import group box.
		/// </summary>
		private void DisableImport()
		{
			importRichTextBox.Text = string.Empty;
			importGroupBox.Enabled = false;
		}

		/// <summary>
		/// Attempts to enable the import group box and loads the selected search result.
		/// </summary>
		private void EnableImport()
		{
			if (searchResultsDataGridView.SelectedRows.Count > 0 && searchResultsDataGridView.SelectedRows[0].Tag != null)
			{
				importGroupBox.Enabled = true;
				importRichTextBox.Tag = searchResultsDataGridView.SelectedRows[0].Tag;
				importRichTextBox.Text = ((Record)searchResultsDataGridView.SelectedRows[0].Tag).ToString();
			}
			else
				DisableImport();
		}

		#endregion

		#region Form Events

		/// <summary>
		/// Handles the Click event of the searchButton control.
		///
		/// We're no longer using the acutal Z39.50 interface and instead querying using the new fancy Search/Retrieval via URL interface
		/// The specifications for the new interface can be found at http://www.loc.gov/standards/sru/index.html
		/// Information on the query langauge can be found at http://www.loc.gov/standards/sru/specs/cql.html
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void searchButton_Click(object sender, EventArgs e)
		{
			searchResultsDataGridView.Rows.Clear();

			if (isbnTextBox.Text != string.Empty || lccnTextBox.Text != string.Empty || titleTextBox.Text != string.Empty || authorTextBox.Text != string.Empty)
			{
				searchGroupBox.Enabled = false;
				searchButton.Text = "Searching ...";
				Application.DoEvents();

				searchResultsDataGridView.Rows.Clear();
				DisableImport();

				//The default ZING SRW namespace
                XNamespace zingSRW = namespaceTextBox.Text;

				//Build the SRU query.
				//The address, version, and operation will always be the same
				string sruServer = serverTextBox.Text;

				string sruQuery = string.Empty;

				//The actual query in CQL syntax
				if (titleTextBox.Text.Trim() != string.Empty)
					sruQuery += " and dc.title%20=%20%22" + titleTextBox.Text.Trim() + "%22";

				if (authorTextBox.Text.Trim() != string.Empty)
					sruQuery += " and dc.author%20=%20%22" + authorTextBox.Text.Trim() + "%22";

				if (lccnTextBox.Text.Trim() != string.Empty)
					sruQuery += " and bath.lccn%20=%20" + lccnTextBox.Text.Trim();

				if (isbnTextBox.Text.Trim() != string.Empty)
					sruQuery += " and bath.isbn%20=%20" + isbnTextBox.Text.Trim();

				//Maximum records and record schema will always be the same. We get records back in actual MARCXML format making it easy to parse into a BookMaster record
				string sruPostfix = "&maximumRecords=200&recordSchema=marcxml";

				if (sruQuery != string.Empty)
				{
					sruQuery = sruQuery.Substring(5);

					try
					{
						//This actually queries the server. No more ZOOM. No more VB wrapping C++. It's that easy.
						XDocument results = XDocument.Load(sruServer + sruQuery + sruPostfix);

						//The actual search results that we want to search for are 3 levels down.
						//<zs:searchRetrieveResponse><zs:records><zs:record>Actual result information is here</zs:record></zs:records></zs:searchRetrieveResponse>
						foreach (XElement result in results.Elements(zingSRW + "searchRetrieveResponse").Elements(zingSRW + "records").Elements(zingSRW + "record"))
						{
							//Build an XDocument for each result
							//The actual data is 2 levesl down from the search result item.
							//<zs:recordData><record>Actual MARCXML</record></zs:recordData>
							XDocument importMARCXML = new XDocument();
							importMARCXML.Add(result.Element(zingSRW + "recordData").Element(FileMARCXML.Namespace + "record"));

							//Convert the new MARCXML document to a CSharp_MARC Record
							FileMARCXML imported = new FileMARCXML(importMARCXML);
							Record importedRecord = imported[0];

							AddResult(importedRecord);
						}
					}
					catch (WebException)
					{
						MessageBox.Show("Unable to access the Library of Congress. Please try again later or click \"Help\" to search the LOC website manually.", "Error accessing LOC", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0, "http://z3950.loc.gov", "keyword");
					}
				}
			}

			SortResults();

			searchButton.Text = "Search";
			searchGroupBox.Enabled = true;
		}

		/// <summary>
		/// Handles the Click event of the fromFileButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void fromFileButton_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				IEnumerable loopy = null;

				if (openFileDialog.FileName.EndsWith(".mrc", StringComparison.OrdinalIgnoreCase) || openFileDialog.FileName.EndsWith(".usm", StringComparison.OrdinalIgnoreCase) || openFileDialog.FileName.EndsWith(".001", StringComparison.OrdinalIgnoreCase))
				{
					FileMARC import = new FileMARC();
					import.ImportMARC(openFileDialog.FileName);

					loopy = import;
				}
				else if (openFileDialog.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
				{
					FileMARCXML import = new FileMARCXML(XDocument.Load(openFileDialog.FileName));

					loopy = import;
				}
				else
				{
					switch(MessageBox.Show("Unable to import records from the selected file." + Environment.NewLine + Environment.NewLine + "If this is a MARC21 File click 'YES'" + Environment.NewLine + "If this is a MARCXML file click 'NO'" + Environment.NewLine + "Otherwise hit 'CANCEL'", "Unkonwn filetype.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk))
					{
						case DialogResult.Yes:
							FileMARC import = new FileMARC();
							import.ImportMARC(openFileDialog.FileName);

							loopy = import;
							break;
						case DialogResult.No:
							FileMARCXML importXML = new FileMARCXML(XDocument.Load(openFileDialog.FileName));

							loopy = importXML;
							break;
						default:
							return;
					}
				}

				foreach (Record record in loopy)
					AddResult(record);

				SortResults();
			}
		}

		/// <summary>
		/// Handles the SelectionChanged event of the searchResultsDataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void searchResultsDataGridView_SelectionChanged(object sender, EventArgs e)
		{
			EnableImport();
		}

		/// <summary>
		/// Handles the Click event of the resetButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void resetButton_Click(object sender, EventArgs e)
		{
			DisableImport();
			searchResultsDataGridView.Rows.Clear();
			isbnTextBox.Text = string.Empty;
			lccnTextBox.Text = string.Empty;
			titleTextBox.Text = string.Empty;
			authorTextBox.Text = string.Empty;

			isbnTextBox.Focus();
		}

		/// <summary>
		/// Handles the Click event of the importButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void importButton_Click(object sender, EventArgs e)
		{
            this.DialogResult = DialogResult.OK;
            Close();
		}

		/// <summary>
		/// Handles the FormClosing event of the ImportMARC control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		private void ImportMARC_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult == DialogResult.None)
				DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Handles the KeyPress event of the searchFieldTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
		private void searchFieldTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
				searchButton_Click(sender, e);
		}

		/// <summary>
		/// Handles the KeyPress event of the importTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
		private void importTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
				importButton_Click(sender, e);
		}

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the HelpButtonClicked event of the ImportSRU control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void ImportSRU_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("The Search/Retrieval via URL standard is used instead of using the standard Z39.50 interface." + Environment.NewLine + Environment.NewLine + "Would you like more information on the SRU standard?", "SRU: Search and Retrieval via URL", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start("http://www.loc.gov/standards/sru/index.html");                
            }
        }

		#endregion
	}
}
