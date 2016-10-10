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
 * @author    Matt Schraeder-Urbanowicz <matt@csharpmarc.net>
 * @copyright 2016 Matt Schraeder-Urbanowicz
 * @license   http://www.gnu.org/licenses/gpl-3.0.html  GPL License 3
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CSharp_MARC_Editor
{
    public partial class CustomFieldsForm : Form
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the tag number1.
        /// </summary>
        /// <value>
        /// The tag number1.
        /// </value>
        public string TagNumber1
        {
            get { return tagNumberTextBox1.Text; }
            set { tagNumberTextBox1.Text = value; }
        }

        /// <summary>
        /// Gets or sets the code1.
        /// </summary>
        /// <value>
        /// The code1.
        /// </value>
        public string Code1
        {
            get { return codeTextBox1.Text; }
            set { codeTextBox1.Text = value; }
        }

        /// <summary>
        /// Gets or sets the data1.
        /// </summary>
        /// <value>
        /// The data1.
        /// </value>
        public string Data1
        {
            get { return dataTextBox1.Text; }
            set { dataTextBox1.Text = value; }
        }

        /// <summary>
        /// Gets or sets the tag number2.
        /// </summary>
        /// <value>
        /// The tag number2.
        /// </value>
        public string TagNumber2
        {
            get { return tagNumberTextBox2.Text; }
            set { tagNumberTextBox2.Text = value; }
        }

        /// <summary>
        /// Gets or sets the code2.
        /// </summary>
        /// <value>
        /// The code2.
        /// </value>
        public string Code2
        {
            get { return codeTextBox2.Text; }
            set { codeTextBox2.Text = value; }
        }

        /// <summary>
        /// Gets or sets the data2.
        /// </summary>
        /// <value>
        /// The data2.
        /// </value>
        public string Data2
        {
            get { return dataTextBox2.Text; }
            set { dataTextBox2.Text = value; }
        }

        /// <summary>
        /// Gets or sets the tag number3.
        /// </summary>
        /// <value>
        /// The tag number3.
        /// </value>
        public string TagNumber3
        {
            get { return tagNumberTextBox3.Text; }
            set { tagNumberTextBox3.Text = value; }
        }

        /// <summary>
        /// Gets or sets the code3.
        /// </summary>
        /// <value>
        /// The code3.
        /// </value>
        public string Code3
        {
            get { return codeTextBox3.Text; }
            set { codeTextBox3.Text = value; }
        }

        /// <summary>
        /// Gets or sets the data3.
        /// </summary>
        /// <value>
        /// The data3.
        /// </value>
        public string Data3
        {
            get { return dataTextBox3.Text; }
            set { dataTextBox3.Text = value; }
        }

        /// <summary>
        /// Gets or sets the tag number4.
        /// </summary>
        /// <value>
        /// The tag number4.
        /// </value>
        public string TagNumber4
        {
            get { return tagNumberTextBox4.Text; }
            set { tagNumberTextBox4.Text = value; }
        }

        /// <summary>
        /// Gets or sets the code4.
        /// </summary>
        /// <value>
        /// The code4.
        /// </value>
        public string Code4
        {
            get { return codeTextBox4.Text; }
            set { codeTextBox4.Text = value; }
        }

        /// <summary>
        /// Gets or sets the data4.
        /// </summary>
        /// <value>
        /// The data4.
        /// </value>
        public string Data4
        {
            get { return dataTextBox4.Text; }
            set { dataTextBox4.Text = value; }
        }

        /// <summary>
        /// Gets or sets the tag number5.
        /// </summary>
        /// <value>
        /// The tag number5.
        /// </value>
        public string TagNumber5
        {
            get { return tagNumberTextBox5.Text; }
            set { tagNumberTextBox5.Text = value; }
        }

        /// <summary>
        /// Gets or sets the code5.
        /// </summary>
        /// <value>
        /// The code5.
        /// </value>
        public string Code5
        {
            get { return codeTextBox5.Text; }
            set { codeTextBox5.Text = value; }
        }

        /// <summary>
        /// Gets or sets the data5.
        /// </summary>
        /// <value>
        /// The data5.
        /// </value>
        public string Data5
        {
            get { return dataTextBox5.Text; }
            set { dataTextBox5.Text = value; }
        }

        #endregion

        public CustomFieldsForm()
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
            DialogResult = DialogResult.OK;
            Close();
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
        /// Handles the HelpButtonClicked event of the CustomFieldsForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void CustomFieldsForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Fill in the tag number, code, and data for the field you want to appear in a custom field." + Environment.NewLine + Environment.NewLine + "Leave code empty only for control fields." + Environment.NewLine + "Leave data empty to show whole field, otherwise use Regular Expressions to filter data.", "Custom Field Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
