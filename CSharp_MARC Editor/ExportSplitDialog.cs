using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSharp_MARC_Editor
{
    public partial class ExportSplitDialog : Form
    {
        /// <summary>
        /// Gets the records per file.
        /// </summary>
        /// <value>
        /// The records per file.
        /// </value>
        public decimal RecordsPerFile
        {
            get
            {
                return this.recordsPerFileNumericUpDown.Value;
            }
        }

        public ExportSplitDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the okButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
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
    }
}
