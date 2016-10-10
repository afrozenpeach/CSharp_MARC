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
 * @author    Matt Schraeder-Urbanowicz <matt@csharpmarc.net> <matt@btsb.com>
 * @copyright 2009-2016 Matt Schraeder-Urbanowicz and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace MARC
{
    public class FileMARCXMLWriter : IDisposable
    {
        //Member Variables and Properties
        #region Member Variables and Properties
            
        private readonly StreamWriter writer = null;

        #endregion

        //Constructors
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter" /> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public FileMARCXMLWriter(string filename) : this(filename, false)
        {
        }

		public FileMARCXMLWriter(string filename, bool append)
		{
			writer = new StreamWriter(filename, append, Encoding.UTF8);
		}

		#endregion

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(Record record)
        {
            XDocument xml = record.ToXMLDocument();

            xml.Save(writer);
        }

        /// <summary>
        /// Writes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Write(List<Record> records)
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", null));
            XElement collection = new XElement(FileMARCXML.Namespace + "collection");
            xml.Add(collection);

            foreach (Record record in records)
                collection.Add(record.ToXML());

            xml.Save(writer);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
