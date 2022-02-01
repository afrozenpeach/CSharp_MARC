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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace MARC
{
    public class FileMARCXMLWriter : IDisposable
    {
        //Member Variables and Properties
        #region Member Variables and Properties
            
        private readonly XmlWriter writer = null;

        #endregion

        //Constructors
        #region Constructors
            
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter" /> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public FileMARCXMLWriter(string filename)
		{
            writer = XmlWriter.Create(filename);

            writer.WriteStartDocument();
            writer.WriteStartElement("collection");
        }

		#endregion

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(Record record)
        {
            XElement xml = record.ToXML();
            xml.WriteTo(writer);
        }

        /// <summary>
        /// Writes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Write(List<Record> records)
        {
            foreach (Record record in records)
            {
                XElement xml = record.ToXML();
                xml.WriteTo(writer);
            }
        }

        /// <summary>
        /// Writes the end of file marker.
        /// </summary>
        public void WriteEnd()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)writer).Dispose();
        }
    }
}
