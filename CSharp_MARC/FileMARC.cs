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
 * @author    Matt Schraeder <mschraeder@btsb.com> <mschraeder@csharpmarc.net>
 * @copyright 2009-2011 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace MARC
{
    /// <summary>
    /// The main File_MARC class enables you to return File_MARC_Record
    /// objects from a stream or string.
    /// </summary>
    public class FileMARC : IEnumerator, IEnumerable
    {
        //Constants
        #region Constants

        //HEX value for Subfield indicator
        public const char SUBFIELD_INDICATOR = '\x1F';

        //HEX value for End of Field
        public const char END_OF_FIELD = '\x1E';

        //HEX value for End of Record
        public const char END_OF_RECORD = '\x1D';

        //Length of the Directory
        public const int DIRECTORY_ENTRY_LEN = 12;

        //Length of the Leader
        public const int LEADER_LEN = 24;

        //Maximum record length
        public const int MAX_RECORD_LENGTH = 99999;

        #endregion

        //Member Variables and Properties
        #region Member Variables and Properties

        //Source containing new records
        protected List<string> rawSource = null;
        protected int position = -1;
        private List<string> warnings = new List<string>();

        /// <summary>
        /// Gets the raw source.
        /// </summary>
        /// <value>The raw source.</value>
        public List<string> RawSource
        {
            get { return rawSource; }
        }

        /// <summary>
        /// Gets the <see cref="MARC.Record"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Record this[int index]
        {
            get { return decode(index);  }
        }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings
        {
            get { return warnings; }
        }

		/// <summary>
		/// Gets the number of single records that have been imported.
		/// </summary>
		public int Count
		{
			get { return rawSource.Count; }
		}

        #endregion

        //Constructors
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARC"/> class.
        /// </summary>
        /// <param name="source">String consisting of one or more raw MARC records.</param>
        public FileMARC(string source)
        {
            this.rawSource = new List<string>();

            source = CleanSource(source);

             foreach (string record in source.Split(END_OF_RECORD))
            {
                if (record != string.Empty)
                    this.rawSource.Add(record + END_OF_RECORD.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARC"/> class.
        /// </summary>
        public FileMARC()
        {
            this.rawSource = new List<string>();
        }

        #endregion

        //Public member functions
        #region Public member functions

        /// <summary>
        /// Imports the MARC records from a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void ImportMARC(string file)
        {
            string fileContents = null;

            using (StreamReader sr = new StreamReader(file))
            {
                fileContents = sr.ReadToEnd();
            }

            fileContents = CleanSource(fileContents);

            if (fileContents != null)
            {
                foreach (string record in fileContents.Split(END_OF_RECORD))
                {
                    //Make sure the record isn't empty, or isn't the end of a file
                    if (record != string.Empty && record != "\x1A")
                    {
                        //Split removes the END_OF_RECORD. Put it back in
                        this.rawSource.Add(record + END_OF_RECORD.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified source records.
        /// </summary>
        /// <param name="source">String consisting of one or more raw MARC records.</param>
        public void Add(string source)
        {
            source = CleanSource(source);

            foreach (string record in source.Split(END_OF_RECORD))
            {
                if (record != string.Empty)
                    this.rawSource.Add(record + END_OF_RECORD.ToString());
            }
        }

        #endregion

        //Private utility functions
        #region Private utility functions

        /// <summary>
        /// Decodes the raw MARC record into a <see cref="MARC.Record"/> at the specified index./// 
        /// </summary>
        /// <param name="index">The index of the record to retrieve.</param>
        /// <returns></returns>
        private Record decode(int index)
        {
            string raw = rawSource[index];
            Record marc = new Record();
            Match match = Regex.Match(raw, "^(\\d{5})");
            int recordLength = 0;

            // Store record length
            if (match.Captures.Count == 0)
                warnings.Add("MARC record length is not numeric.");
            else
                recordLength = Convert.ToInt32(match.Captures[0].Value);

            if (recordLength != raw.Length)
            {
                warnings.Add("MARC record length does not match actual length.");
                recordLength = raw.Length;
            }

            if (!raw.EndsWith(END_OF_RECORD.ToString()))
                throw new InvalidDataException("MARC record ends with an invalid terminator");

            //Store leader
            marc.Leader = raw.Substring(0, LEADER_LEN);

            //Bytes 12-16 of leader give offset to the body of the record
            int dataStart = Convert.ToInt32(raw.Substring(12, 5));

            //Immediately after the leader comes the directory (no separator)
            string directory = raw.Substring(LEADER_LEN, dataStart - LEADER_LEN - 1);

            //Character after the directory should be END_OF_FIELD
            if (raw.Substring(dataStart - 1, 1) != END_OF_FIELD.ToString())
                warnings.Add("No directory found.");

            //All directory entries must be DIRECTORY_ENTRY_LEN long, so length % DIRECTORY_ENTRY_LEN should be 0
            if (directory.Length % DIRECTORY_ENTRY_LEN != 0)
                warnings.Add("Invalid directory length.");

            //Go through all the fields
            int fieldCount = directory.Length / DIRECTORY_ENTRY_LEN;

            for (int i = 0; i < fieldCount; i++)
            {
                string tag = directory.Substring(i * DIRECTORY_ENTRY_LEN, DIRECTORY_ENTRY_LEN).Substring(0, 3);
                int fieldLength = 0;
                int fieldOffset = 0;
                try
                {
                    fieldLength = Convert.ToInt32(directory.Substring(i * DIRECTORY_ENTRY_LEN, DIRECTORY_ENTRY_LEN).Substring(3, 4));
                }
                catch (FormatException)
                {
                    warnings.Add("Invalid Directory Tag Length for tag " + tag + ".");
                }

                try
                {
                    fieldOffset = Convert.ToInt32(directory.Substring(i * DIRECTORY_ENTRY_LEN, DIRECTORY_ENTRY_LEN).Substring(7, 5));
                }
                catch (FormatException)
                {
                    warnings.Add("Invalid Directory Offset for tag " + tag + ".");
                }
                
                //Check Directory validity

				//If a tag isn't valid, default it to ZZZ. This should at least make the record valid enough to be readable and not throw exceptions
				if (Field.ValidateTag(tag))
					warnings.Add("Invalid tag " + tag + " in directory.");
				else
					tag = "ZZZ";

                if (fieldOffset + fieldLength > recordLength)
                    warnings.Add("Directory entry for tag " + tag + " runs past the end of the record.");

				int fieldStart = dataStart + fieldOffset;
				if (fieldStart > recordLength)
				{
					warnings.Add("Directory entry for tag " + tag + " starts past the end of the record. Skipping tag and all proceeding tags.");
					break;
				}
				else if (fieldStart + fieldLength > recordLength)
				{
					warnings.Add("Directory entry for tag " + tag + " runs past the end of the record.");
					fieldLength = recordLength - fieldStart;
				}
                string tagData = raw.Substring(fieldStart, fieldLength);

                if (tagData.Substring(tagData.Length - 1, 1) == END_OF_FIELD.ToString())
                {
                    //Get rid of the end of tag character
                    tagData = tagData.Remove(tagData.Length - 1);
					fieldLength--;
                }
                else
                    warnings.Add("Field for tag " + tag + " does not end with an end of field character.");

                match = Regex.Match(tag, "^\\d+$");
                if (match.Captures.Count > 0 && Convert.ToInt32(tag) < 10)
                    marc.Fields.Add(new ControlField(tag, tagData));
                else
                {
                    List<string> rawSubfields = new List<string>(tagData.Split(SUBFIELD_INDICATOR));
                    string indicators = rawSubfields[0];
                    rawSubfields.RemoveAt(0);

                    char ind1;
                    char ind2;

                    if (indicators.Length != 2)
                    {
                        warnings.Add("Invalid indicator length. Forced indicators to blanks for tag " + tag + ".");
                        ind1 = ind2 = ' ';
                    }
                    else
                    {
                        ind1 = indicators[0];

						if (!DataField.ValidateIndicator(ind1))
						{
							ind1 = ' ';
							warnings.Add("Invalid first indicator. Forced first indicator to blank for tag " + tag + ".");
						}

                        ind2 = indicators[1];

						if (!DataField.ValidateIndicator(ind2))
						{
							ind2 = ' ';
							warnings.Add("Invalid second indicator. Forced second indicator to blank for tag " + tag + ".");
						}
                    }

                    //Split the subfield data into subfield name and data pairs
                    List<Subfield> subfieldData = new List<Subfield>();
                    foreach (string subfield in rawSubfields)
                    {
                        if (subfield.Length > 0)
                            subfieldData.Add(new Subfield(subfield[0], subfield.Substring(1)));
                        else
                            warnings.Add("No subfield data found in tag " + tag + ".");
                    }

                    if (subfieldData.Count == 0)
                        warnings.Add("No subfield data found in tag " + tag + ".");

                    marc.Fields.Add(new DataField(tag, subfieldData, ind1, ind2));
                }              
            }
            return marc;
        }

        /// <summary>
        /// Resets the warnings.
        /// </summary>
        private void ResetWarnings()
        {
            warnings = new List<string>();
        }

        /// <summary>
        /// Cleans the raw MARC records to remove illegal stuff that sometimes occurs between records.
        /// </summary>
        /// <param name="source">The raw source data.</param>
        /// <returns></returns>
        private string CleanSource(string source)
        {
            Match match = Regex.Match(source, "^[\\x0a\\x0d\\x00]+");
            if (match.Captures.Count != 0)
                warnings.Add("Illegal characters found between records.");
            return Regex.Replace(source, "^[\\x0a\\x0d\\x00]+", "");
        }

        #endregion

        #region IEnumerator Members

        public object Current
        {
            get { return this[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < rawSource.Count);
        }

        public void Reset()
        {
            position = 0;
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        #endregion
    }
}
