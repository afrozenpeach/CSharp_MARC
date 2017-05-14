/**
 * Parser for MARC records
 *
 * This project is based on the File_MARC package
 * (http://pear.php.net/package/File_MARC) by Dan Scott , which was based on PHP
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
 * @author    Mattie Schraeder-Urbanowicz <mattie@csharpmarc.net>
 * @copyright 2009-2017 Mattie Schraeder-Urbanowicz and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using System.Xml.Linq;

namespace MARC
{
    /// <summary>
    /// The MARC ControlField class represents a single control field in a MARC
    /// record.  A MARC control field consists of a tag name and control data.
    /// </summary>
    public class ControlField : Field
    {
        //Private member variables and properties
        #region Private member variables and properties

        private string data;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlField"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="data">The data.</param>
        public ControlField(string tag, string data) : base(tag)
        {
            this.data = data;
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsEmpty()
        {
            return (data == string.Empty);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
        {
            return tag.PadRight(3) + "     " + data;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>
        /// in raw USMARC format.
        /// </summary>
        /// <returns></returns>
        public override string ToRaw()
        {
            return data + FileMARC.END_OF_FIELD.ToString();
        }

        /// <summary>
        /// Returns a <see cref="T:XElement" /> that represents the current <see cref="T:System.Object" />
        /// </summary>
        /// <returns></returns>
        public override XElement ToXML()
        {
            return new XElement(FileMARCXML.Namespace + "controlfield", new XAttribute("tag", tag), data);
        }

        /// <summary>
        /// Print a MARC Field object without tags, indicators, etc
        /// </summary>
        /// <returns></returns>
        public override string FormatField()
        {
            return data;
        }

		/// <summary>
		/// Makes a deep clone of this instance.
		/// </summary>
		/// <returns></returns>
		public override Field Clone()
		{
			Field clone = new ControlField(tag, data);
			return clone;
		}
    }
}
