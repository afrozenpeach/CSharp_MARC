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
using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace MARC
{
	/// <summary>
	/// This is a wrapper for FileMARC that allows for reading large files without loading the entire file into memory.
	/// </summary>
	public class FileMARCXMLReader : IEnumerable, IDisposable
	{
		//Member Variables and Properties
		#region Member Variables and Properties

		private string filename = null;
        private readonly XmlReader reader = null;

		#endregion

		//Constructors
		#region Constructors

		public FileMARCXMLReader(string filename)
		{
			this.filename = filename;
            reader = XmlReader.Create(filename);
		}

		#endregion

		//Interface functions
		#region IEnumerator Members

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			while (!reader.EOF)
			{
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("record"))
                    {
                        XElement element = XElement.ReadFrom(reader) as XElement;

                        XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", null));
                        XElement collection = new XElement(FileMARCXML.Namespace + "collection");
                        xml.Add(collection);
                        collection.Add(element);

                        FileMARCXML marc = new FileMARCXML(xml);
                        foreach (Record marcRecord in marc)
                        {
                            yield return marcRecord;
                        }
                    }
                    else
                        reader.Read();
                }
                else
                    reader.Read();
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
            ((IDisposable)reader).Dispose();
        }

		#endregion
	}
}
