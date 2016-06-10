using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MARC;

namespace CSharp_MARC_Editor
{
    public partial class AdvancedBatchEditForm : Form
    {
        #region Public Properties

        /// <summary>
        /// Gets the selected tags.
        /// </summary>
        /// <value>
        /// The selected tags.
        /// </value>
        public Collection<string> SelectedTags
        {
            get
            {
                Collection<string> selectedTags = new Collection<string>();

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
        public Collection<string> SelectedIndicator1s
        {
            get
            {
                Collection<string> selectedIndicators = new Collection<string>();

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
        public Collection<string> SelectedIndicator2s
        {
            get
            {
                Collection<string> selectedIndicators = new Collection<string>();

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
        public Collection<string> SelectedCodes
        {
            get
            {
                Collection<string> selectedCodes = new Collection<string>();

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

        /// <summary>
        /// Gets a value indicating whether [regex].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [regex]; otherwise, <c>false</c>.
        /// </value>
        public bool Regex
        {
            get
            {
                return regexCheckBox.Checked;
            }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action
        {
            get
            {
                return actionComboBox.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Gets the tag modification.
        /// </summary>
        /// <value>
        /// The tag modification.
        /// </value>
        public string TagModification
        {
            get
            {
                return tagTextBox.Text;
            }
        }

        /// <summary>
        /// Gets the ind1.
        /// </summary>
        /// <value>
        /// The ind1.
        /// </value>
        public string Ind1
        {
            get
            {
                return ind1TextBox.Text;
            }
        }

        /// <summary>
        /// Gets the ind2.
        /// </summary>
        /// <value>
        /// The ind2.
        /// </value>
        public string Ind2
        {
            get
            {
                return ind2TextBox.Text;
            }
        }

        /// <summary>
        /// Gets the subfields.
        /// </summary>
        /// <value>
        /// The subfields.
        /// </value>
        public DataGridViewRowCollection Subfields
        {
            get
            {
                return modificationsDataGridView.Rows;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedBatchEditForm"/> class.
        /// </summary>
        public AdvancedBatchEditForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the AdvancedBatchEditForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AdvancedBatchEditForm_Load(object sender, EventArgs e)
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

            using (SQLiteConnection connection = new SQLiteConnection(MainForm.ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    connection.Open();

                    command.CommandText = "SELECT DISTINCT TagNumber FROM Fields ORDER BY ABS(TagNumber), TagNumber";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            tagsListBox.Items.Add(reader["TagNumber"]);
                    }

                    command.CommandText = "SELECT DISTINCT Code FROM Subfields ORDER BY Code";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            codesListBox.Items.Add(reader["Code"]);
                    }

                    command.CommandText = "SELECT DISTINCT Ind1 as Ind FROM Fields WHERE Ind1 IS NOT NULL UNION SELECT DISTINCT Ind2 as Ind FROM Fields WHERE Ind2 IS NOT NULL ORDER BY Ind";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ind1ListBox.Items.Add(reader["Ind"]);
                            ind2ListBox.Items.Add(reader["Ind"]);
                        }
                    }
                }
            }

            actionComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the okButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            switch (this.Action)
            {
                case "Add":
                    if (string.IsNullOrEmpty(TagModification))
                    {
                        MessageBox.Show("Adding new fields requires a tag number.", "Missing Tag Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case "Delete":
                    foreach (DataGridViewRow row in Subfields)
                    {
                        if (!row.IsNewRow)
                        {
                            if (!String.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                            {
                                MessageBox.Show("The data column is not used when deleting subfields. Clear the data columns before continuing.", "Data column is not used.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (!String.IsNullOrEmpty(Ind1) || !String.IsNullOrEmpty(Ind2))
                            {
                                MessageBox.Show("Indicators are not used when deleting subfields. Clear the indicators before continuing.", "Indicators are not used.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(TagModification) && !Field.ValidateTag(TagModification))
            {
                MessageBox.Show("The field to add has an invalid tag number.", "Invalid tag number.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow row in Subfields)
            {
                if (row.IsNewRow)
                    break;

                if (string.IsNullOrEmpty(row.Cells[0].Value.ToString()) || !DataField.ValidateIndicator(row.Cells[0].Value.ToString()[0]))
                {
                    MessageBox.Show("The field to add has an invalid tag number.", "Invalid tag number.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ListBox controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (box.SelectedItems.Count > 1 && box.SelectedItems.Contains("Any"))
                box.SelectedItems.Remove("Any");
        }

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the HelpButtonClicked event of the AdvancedBatchEditForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void AdvancedBatchEditForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Select tags, indicators, codes, and data to match in the Conditions box." +
                            Environment.NewLine + Environment.NewLine + Environment.NewLine +
                            "Add: If conditions are met, create a new tag as designated." + Environment.NewLine + Environment.NewLine +
                            "Delete: If conditions are met, delete the given tag's subfields. Subfield data column is ignored. If tag is left blank, delete the tags that match the conditions. If subfields are left blank, delete the entire tag." + Environment.NewLine + Environment.NewLine +
                            "Edit: If conditions are met, edit the given tag's subfields. Subfields that do not yet exist will be added. If tag is left blank, edit the subfields of the tags that match the conditions." + Environment.NewLine + Environment.NewLine +
                            "Replace: If conditions are met, completely replace the given tag and its subfields with those specified. If tag is left blank, replace the subfields of the tags taht match the conditions." + Environment.NewLine + Environment.NewLine +
                            Environment.NewLine +
                            "If Ind1 and Ind2 are filled in, the given tag's indicators will be changed. If tag is left blank, any tag that matches the conditions will have its indicators changed.",
                            "How to use Advanced Batch Edit.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
