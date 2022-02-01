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
 
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Xml.Linq;
using System.Xml.Schema;
using MARC.Properties;
using System.IO;

namespace MARC
{
	/// <summary>
	/// The main FileMARCXML class enables you to return Record
	/// objects from a stream or string.
	/// </summary>
	public class FileMARCXML : IEnumerator, IEnumerable
	{
		//Constants
		#region Constants

		public static readonly XNamespace Namespace = "http://www.loc.gov/MARC21/slim";

		#endregion

		//Member Variables and Properties
		#region Member Variables and Properties

		//Source containing new records
		protected List<XElement> rawSource = null;
		protected int position = -1;

		/// <summary>
		/// Gets the raw source.
		/// </summary>
		/// <value>The raw source.</value>
		public List<XElement> RawSource => rawSource;

	    /// <summary>
		/// Gets the <see cref="MARC.Record"/> at the specified index.
		/// </summary>
		/// <value></value>
		public Record this[int index] => decode(index);

	    /// <summary>
		/// Gets the number of single records that have been imported.
		/// </summary>
		public int Count => rawSource.Count;

	    #endregion

        //Constructors
        #region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FileMARCXML"/> class.
		/// </summary>
		/// <param name="source">XDocument consisting of one or more raw MARCXML records.</param>
		public FileMARCXML(XDocument source)
        {
			rawSource = new List<XElement>();

			Add(source);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileMARCXML"/> class.
		/// </summary>
		/// <param name="source">XML String consisting of one or more raw MARCXML records.</param>
		public FileMARCXML(string source)
		{
			rawSource = new List<XElement>();

			Add(XDocument.Parse(source));
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARC"/> class.
        /// </summary>
        public FileMARCXML()
        {
			rawSource = new List<XElement>();
        }

		#endregion

		//Public member functions
		#region Public member functions

		/// <summary>
		/// Imports the MARCXML records from a file.
		/// </summary>
		/// <param name="file">The file.</param>
		public void ImportMARCXML(string file)
		{
			XDocument xmlSource = XDocument.Load(file);
			Add(xmlSource);
		}

		/// <summary>
		/// Adds the specified source.
		/// </summary>
		/// <param name="source">XDocument consisting of one or more raw MARCXML records.</param>
		public int Add(XDocument source)
		{
			int addCount = 0;

			//Usually there will be a collection tag wrapping the records
			foreach (XElement collection in source.Elements().Where(e => e.Name.LocalName == "collection"))
			{
				foreach (XElement record in collection.Elements().Where(e => e.Name.LocalName == "record"))
				{
					rawSource.Add(record);
					addCount++;
				}
			}

			//Sometimes there's just a record
			foreach (XElement record in source.Elements().Where(e => e.Name.LocalName == "record"))
			{
				rawSource.Add(record);
				addCount++;
			}

			return addCount;
		}

		/// <summary>
		/// Adds the specified source.
		/// </summary>
		/// <param name="source">XML String consisting of one or more raw MARCXML records.</param>
		public int Add(string source)
		{
			return Add(XDocument.Parse(source));
		}

		/// <summary>
		/// Validates the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static List<string> Validate(XDocument source)
		{
			XmlSchemaSet schemas = new XmlSchemaSet();
			using (StringReader reader = new StringReader(Resources.MARC21slim))
			{
				XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);
				schemas.Add(schema);
			}

			List<string> errors = new List<string>();
			source.Validate(schemas, (sender, args) => { errors.Add(args.Exception.Message); });

			return errors;
		}

		#endregion

		//Private utility functions
		#region Private utility functions

		/// <summary>
		/// Decodes the raw MARC record into a <see cref="MARC.Record"/> at the specified index.
		/// </summary>
		/// <param name="index">The index of the record to retrieve.</param>
		/// <returns></returns>
		private Record decode(int index)
		{
			XElement record = rawSource[index];
		    Record marcXML = new Record {Leader = record.Elements().First(e => e.Name.LocalName == "leader").Value};

		    //First we get the leader

		    //Now we get the control fields
			foreach (XElement controlField in record.Elements().Where(e => e.Name.LocalName == "controlfield"))
			{
				ControlField newField = new ControlField(controlField.Attribute("tag").Value, controlField.Value);
				marcXML.Fields.Add(newField);
			}

			//Now we get the data fields
			foreach (XElement dataField in record.Elements().Where(e => e.Name.LocalName == "datafield"))
			{
				DataField newField = new DataField(dataField.Attribute("tag").Value, new List<Subfield>(), dataField.Attribute("ind1").Value[0], dataField.Attribute("ind2").Value[0]);

				foreach (XElement subfield in dataField.Elements().Where(e => e.Name.LocalName == "subfield"))
				{
					Subfield newSubfield = new Subfield(subfield.Attribute("code").Value[0], subfield.Value);
					newField.Subfields.Add(newSubfield);
				}

				marcXML.Fields.Add(newField);
			}

			return marcXML;
		}

		/// <summary>
		/// Callback for XSD Validation
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
		private static void ValidationCallback(object sender, ValidationEventArgs e)
		{
			throw new XmlSchemaException("Unable to validate using MARC21slim.xsd. Invalid XSD.");
		}

		#endregion

		#region IEnumerator Members

		public object Current => this[position];

	    public bool MoveNext()
		{
			position++;
			return (position < rawSource.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		#endregion
	}
}
