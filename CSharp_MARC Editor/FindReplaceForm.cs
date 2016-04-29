using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mono.Data.Sqlite;

namespace CSharp_MARC_Editor
{
    public partial class FindReplaceForm : Form
    {
        #region Public Properties

        /// <summary>
        /// Gets the selected tags.
        /// </summary>
        /// <value>
        /// The selected tags.
        /// </value>
        public List<string> SelectedTags
        {
            get
            {
                List<string> selectedTags = new List<string>();
                
                foreach (string item in tagsListBox.SelectedItems)
                    selectedTags.Add(item);
                
                return selectedTags;
            }
        }

        /// <summary>
        /// Gets the selected indicator1s.
        /// </summary>
        /// <value>
        /// The selected indicator1s.
        /// </value>
        public List<string> SelectedIndicator1s
        {
            get
            {
                List<string> selectedIndicators = new List<string>();

                foreach (object item in ind1ListBox.Items)
                {
                    if (item != null)
                        selectedIndicators.Add(item.ToString());
                }

                return selectedIndicators;
            }
        }

        /// <summary>
        /// Gets the selected indicator2s.
        /// </summary>
        /// <value>
        /// The selected indicator2s.
        /// </value>
        public List<string> SelectedIndicator2s
        {
            get
            {
                List<string> selectedIndicators = new List<string>();

                foreach (object item in ind2ListBox.Items)
                {
                    if (item != null)
                        selectedIndicators.Add(item.ToString());
                }

                return selectedIndicators;
            }
        }

        /// <summary>
        /// Gets the selected codes.
        /// </summary>
        /// <value>
        /// The selected codes.
        /// </value>
        public List<string> SelectedCodes
        {
            get
            {
                List<string> selectedCodes = new List<string>();

                foreach (string item in codesListBox.Items)
                    selectedCodes.Add(item);

                return selectedCodes;
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get
            {
                return dataTextBox.Text;
            }
        }

        /// <summary>
        /// Gets the replace with.
        /// </summary>
        /// <value>
        /// The replace with.
        /// </value>
        public string ReplaceWith
        {
            get
            {
                return replaceWithTextBox.Text;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [case sensitive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [case sensitive]; otherwise, <c>false</c>.
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return caseSensitiveCheckBox.Checked;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FindReplaceForm"/> class.
        /// </summary>
        public FindReplaceForm()
        {
            InitializeComponent();
        }

        #region Form Events

        /// <summary>
        /// Handles the Load event of the FindReplaceForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FindReplaceForm_Load(object sender, EventArgs e)
        {
            tagsListBox.Items.Clear();
            ind1ListBox.Items.Clear();
            ind2ListBox.Items.Clear();
            codesListBox.Items.Clear();

            tagsListBox.Items.Add("Any");
            ind1ListBox.Items.Add("Any");
            ind2ListBox.Items.Add("Any");
            codesListBox.Items.Add("Any");

            tagsListBox.SelectedIndex = 0;
            ind1ListBox.SelectedIndex = 0;
            ind2ListBox.SelectedIndex = 0;
            codesListBox.SelectedIndex = 0;

            using (SqliteConnection connection = new SqliteConnection(MainForm.connectionString))
            {
                using (SqliteCommand command = new SqliteCommand(connection))
                {
                    connection.Open();

                    command.CommandText = "SELECT DISTINCT TagNumber FROM Fields ORDER BY ABS(TagNumber), TagNumber";
                    
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            tagsListBox.Items.Add(reader["TagNumber"]);
                    }

                    command.CommandText = "SELECT DISTINCT Code FROM Subfields ORDER BY Code";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            codesListBox.Items.Add(reader["Code"]);
                    }

                    command.CommandText = "SELECT DISTINCT Ind1 as Ind FROM Fields WHERE Ind1 IS NOT NULL UNION SELECT DISTINCT Ind2 as Ind FROM Fields WHERE Ind2 IS NOT NULL ORDER BY Ind";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ind1ListBox.Items.Add(reader["Ind"]);
                            ind2ListBox.Items.Add(reader["Ind"]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the replaceAllButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedTags.Count == 0)
                MessageBox.Show("Select at least one tag, or choose the \"Any\" option");
            else if (this.SelectedIndicator1s.Count == 0 || this.SelectedIndicator2s.Count == 0)
                MessageBox.Show("Select at least one of each indicator, or choose the \"Any\" option");
            else if (this.SelectedCodes.Count == 0)
                MessageBox.Show("Select at least one of each subfield code, or choose the \"Any\" option");
            else if (this.Data.Length == 0)
                MessageBox.Show("The data to find cannot be empty.");
            else {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the tagsListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ListBox)sender).SelectedItems.Count > 1 && ((ListBox)sender).SelectedItems.Contains("Any"))
                ((ListBox)sender).SelectedItems.Remove("Any");
        }

        #endregion
    }
}
