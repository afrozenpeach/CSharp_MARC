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
using System.Xml.Linq;

namespace MARC
{
    /// <summary>
    /// A MARC record contains a leader and zero or more fields held within a
    /// List structure.  Fields are represented by MARC ControlField and
    /// DataField objects.
    /// </summary>
    public class Record
    {
        //Private member variables and properties
        #region Private member variables and properties

        private List<Field> fields;

        private string leader;

        private readonly List<string> warnings;

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public List<Field> Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        /// <summary>
        /// Gets the first <see cref="MARC.Field"/> with the specified tag.
        /// </summary>
        /// <value>The first matching field or null if none are found.</value>
        public Field this[string tag]
        {
            get
            {
                Field foundField = null;
                foreach (Field field in fields)
                {
                    if (field.Tag == tag)
                    {
                        foundField = field;
                        break;
                    }
                }

                return foundField;
            }
        }

        /// <summary>
        /// Gets or sets the leader.
        /// </summary>
        /// <value>The leader.</value>
        public string Leader
        {
			get { return leader; }
            set { leader = value; }
        }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings => warnings;

        #endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Record"/> class.
		/// </summary>
		public Record()
		{
			fields = new List<Field>();
			warnings = new List<string>();
			leader = string.Empty.PadRight(FileMARC.LEADER_LEN);
		}

        /// <summary>
        /// Returns a List of field objects that match a requested tag,
        /// or a cloned List that contains all the field objects if the
        /// requested tag is an empty string.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A List of fields that match the specified tag.</returns>
        public List<Field> GetFields(string tag)
        {
            List<Field> foundFields = new List<Field>();

            foreach (Field field in fields)
            {
                if (tag == string.Empty || field.Tag == tag)
                    foundFields.Add(field);
            }

            return foundFields;
        }

        /// <summary>
        /// Inserts the field into position before the first field found with a higher tag number.
        /// This assumes the record has already been sorted.
        /// </summary>
        /// <param name="newField">The new field.</param>
        public void InsertField(Field newField)
        {
            int rowNum = 0;
            foreach (Field field in fields)
            {
                if (String.CompareOrdinal(field.Tag, newField.Tag) > 0)
                {
                    fields.Insert(rowNum, newField);
                    return;
                }

                rowNum++;
            }

            //Insert at the end
            fields.Add(newField);
        }

		/// <summary>
		/// Returns <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>
		/// in raw USMARC format.
		///
		/// If you have modified an existing MARC record or created a new MARC record, use this method
		/// to save the record for use in other programs that accept the MARC format -- for example,
		/// your integrated library system.
		/// </summary>
		/// <returns></returns>
		public string ToRaw()
		{
			return ToRaw(new MARC8());
		}

        /// <summary>
        /// Returns <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>
        /// in raw USMARC format.
        ///
        /// If you have modified an existing MARC record or created a new MARC record, use this method
        /// to save the record for use in other programs that accept the MARC format -- for example,
        /// your integrated library system.
        /// </summary>
        /// <returns></returns>
        public string ToRaw(Encoding encoding)
        {
            //Build the directory
            string rawFields = string.Empty;
            string directory = string.Empty;
            int dataEnd = 0;
            int count = 0;

            foreach (Field field in fields)
            {
                //No empty fields allowed
                if (!field.IsEmpty())
                {
                    string rawField = field.ToRaw();
                    rawFields += rawField;

					int rawFieldLength = encoding.GetBytes(rawField).Length;
					directory += field.Tag.PadLeft(3, '0') + rawFieldLength.ToString().PadLeft(4, '0') + dataEnd.ToString().PadLeft(5, '0');
					dataEnd += rawFieldLength;
                    count++;
                }
            }

            int baseAddress = FileMARC.LEADER_LEN + (count * FileMARC.DIRECTORY_ENTRY_LEN) + 1;
            int recordLength = baseAddress + dataEnd + 1;

            //Set Leader Lengths
			leader = leader.PadRight(FileMARC.LEADER_LEN);
            leader = leader.Remove(0, 5).Insert(0, recordLength.ToString().PadLeft(5, '0'));
            leader = leader.Remove(12, 5).Insert(12, baseAddress.ToString().PadLeft(5, '0'));
            leader = leader.Remove(10, 2).Insert(10, "22");
            leader = leader.Remove(20, 4).Insert(20, "4500");

			return leader.Substring(0, FileMARC.LEADER_LEN) + directory + FileMARC.END_OF_FIELD.ToString() + rawFields + FileMARC.END_OF_RECORD.ToString();
        }

        /// <summary>
        /// Returns <see cref="T:XElement"/> that represents the current <see cref="T:System.Object"/>
        ///
        /// This sould be used when returning multiple records into a single larger XDocument. 
        /// </summary>
        /// <returns></returns>
        public XElement ToXML()
        {
            XElement record = new XElement(FileMARCXML.Namespace + "record");
            CalculateLeader();
            record.Add(new XElement(FileMARCXML.Namespace + "leader", leader));
            foreach (Field field in fields)
            {
                record.Add(field.ToXML());
            }

            return record;
        }

        /// <summary>
        /// Returns <see cref="T:XDocument"/> that represents the current <see cref="T:System.Object"/>
        ///
        /// This sould be used when needing a finalized XML document of this record and only this record.
        /// </summary>
        /// <returns></returns>
        public XDocument ToXMLDocument()
        {
            XDocument recordDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
            recordDocument.Add(ToXML());
            return recordDocument;
        }

		/// <summary>
		/// Calculates the leader.
		/// </summary>
		private void CalculateLeader()
		{
			int dataEnd = 0;
			int count = 0;

			foreach (Field field in fields)
			{
				//No empty fields allowed
				if (!field.IsEmpty())
				{
					string rawField = field.ToRaw();
					dataEnd += rawField.Length;
					count++;
				}
			}

			int baseAddress = FileMARC.LEADER_LEN + (count * FileMARC.DIRECTORY_ENTRY_LEN) + 1;
			int recordLength = baseAddress + dataEnd + 1;

			//Set Leader Lengths
			leader = leader.PadRight(FileMARC.LEADER_LEN);
			leader = leader.Remove(0, 5).Insert(0, recordLength.ToString().PadLeft(5, '0'));
			leader = leader.Remove(12, 5).Insert(12, baseAddress.ToString().PadLeft(5, '0'));
			leader = leader.Remove(10, 2).Insert(10, "22");
			leader = leader.Remove(20, 4).Insert(20, "4500");
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// This method produces an easy-to-read textual display of a MARC record.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
			CalculateLeader();
			string formatted = "LDR " + leader.Substring(0, FileMARC.LEADER_LEN) + Environment.NewLine;

            foreach (Field field in fields)
            {
                if (!field.IsEmpty())
					formatted += field + Environment.NewLine;
            }

            return formatted;
        }

		/// <summary>
		/// Adds the warnings.
		/// </summary>
		/// <param name="warning">The warning.</param>
		public void AddWarnings(string warning)
		{
			warnings.Add(warning);
		}

		/// <summary>
		/// Makes a deep clone of this instance.
		/// </summary>
		/// <returns></returns>
		public Record Clone()
		{
		    Record clone = new Record {leader = leader};

		    foreach (string needsCloned in warnings)
				clone.AddWarnings(needsCloned);

			foreach (Field needsCloned in fields)
				clone.fields.Add(needsCloned.Clone());

			return clone;
		}
    }
}
