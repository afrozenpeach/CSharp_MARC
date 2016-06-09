using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSharp_MARC_Editor
{
    public partial class AdvancedBatchEditForm : Form
    {
        public AdvancedBatchEditForm()
        {
            InitializeComponent();
        }

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
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
