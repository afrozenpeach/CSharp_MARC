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
 * @author    Mattie Schraeder <mattie@csharpmarc.net>
 * @copyright 2009-2022 Mattie Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MARC
{
    /// <summary>
    /// The MARC Field class is a bare bones field that simply stores a tag
    /// name. It is extended by the DataField and ControlField classes and
    /// should never be instantiated on its own.
    /// </summary>
    public abstract class Field
    {
        //Private member variables and properties
        #region Private member variables and properties

        protected string tag;

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag
        {
            get { return tag; }
            set
			{
				if (ValidateTag(value.PadLeft(3)))
					tag = value.PadLeft(3);
				else
					throw new ArgumentException("Invalid tag.");
			}
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Field" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected Field(string tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Determines whether [is control field].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is control field]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsControlField()
        {
            if (GetType() == typeof(ControlField))
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether [is data field].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is data field]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDataField()
        {
            if (GetType() == typeof(DataField))
                return true;
            return false;
        }

		/// <summary>
		/// Validates the tag.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <returns></returns>
		public static bool ValidateTag(string tag)
		{
			Match match = Regex.Match(tag, "^[0-9A-Za-z]{3}$");
			if (match.Captures.Count == 0)
				return false;
			return true;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public abstract override string ToString();

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
		public abstract bool IsEmpty();

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>
        /// in raw USMARC format.
        ///
        /// This stub method is extended by child classes.
        /// </summary>
        /// <returns></returns>
        public abstract string ToRaw();

        /// <summary>
        /// Returns a <see cref="T:XElement"/> that represents the current <see cref="T:System.Object"/>
        ///
        /// This stub method is extended by child classes.
        /// </summary>
        /// <returns></returns>
        public abstract XElement ToXML();

        /// <summary>
        /// Print a MARC Field object without tags, indicators, etc
        ///
        /// This stub method is extended by child classes.
        /// </summary>
        /// <returns></returns>
        public abstract string FormatField();

		/// <summary>
		/// Makes a deep clone of this instance.
		///
		/// This stub method is extended by child classes.
		/// </summary>
		/// <returns></returns>
		public abstract Field Clone();
    }
}
