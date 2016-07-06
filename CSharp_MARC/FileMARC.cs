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
 * @author    Matt Schraeder <mschraeder@csharpmarc.net> <mschraeder@btsb.com>
 * @copyright 2009-2016 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
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
    /// The main FileMARC class enables you to return Record
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
            rawSource = new List<string>();

			Add(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARC"/> class.
        /// </summary>
        public FileMARC()
        {
            rawSource = new List<string>();
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

            if (fileContents != null)
            {
				Add(fileContents);
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
				//Make sure the record isn't empty, or isn't the end of a file
				if (record != string.Empty && record != "\x1A")
				{
					//Split removes the END_OF_RECORD. Put it back in
					rawSource.Add(record + END_OF_RECORD.ToString());
				}
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
            int totalExtraBytesRead = 0;

            // Store record length
            if (match.Captures.Count == 0)
            {
                marc.AddWarnings("MARC record length is not numeric or incorrect number of characters.");
                string[] split = Regex.Split(raw, "[^0-9]");
                
                if (Int32.TryParse(split[0], out recordLength))
                {
                    string padding = "";
                    padding = padding.PadLeft(5 - split[0].Length, '0');
                    raw = padding + raw;
                }
            }
            else
                recordLength = Convert.ToInt32(match.Captures[0].Value);

            if (recordLength != raw.Length)
            {
                //Check if there are multi-byte characters in the string
                System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(raw);
                
                int extraBytes = raw.Length - stringInfo.LengthInTextElements;
                int extraBytes2 = Encoding.UTF8.GetByteCount(raw) - raw.Length;

                if (recordLength == extraBytes + raw.Length)
                {
                    recordLength -= extraBytes;
                }
                else if (recordLength == extraBytes2 + raw.Length)
                {
                    recordLength -= extraBytes2;
                }
                else
                {
                    marc.AddWarnings("MARC record length does not match actual length.");
                    recordLength = raw.Length;
                }
            }

            if (!raw.EndsWith(END_OF_RECORD.ToString()))
                throw new InvalidDataException("MARC record ends with an invalid terminator");

            //Store leader
            marc.Leader = raw.Substring(0, LEADER_LEN);

            //Bytes 12-16 of leader give offset to the body of the record
            int dataStart = Convert.ToInt32(raw.Substring(12, 5));

            //Verify data start matches the first end of field marker
            if (raw.IndexOf(END_OF_FIELD) + 1 != dataStart)
            {
                dataStart = raw.IndexOf(END_OF_FIELD) + 1;
                marc.AddWarnings("Leader specifies incorrect base address of data.");
            }

            //Immediately after the leader comes the directory (no separator)

            string directory = raw.Substring(LEADER_LEN, dataStart - LEADER_LEN - 1);

            //Character after the directory should be END_OF_FIELD
            if (raw.Substring(dataStart - 1, 1) != END_OF_FIELD.ToString())
                marc.AddWarnings("No directory found.");

            //All directory entries must be DIRECTORY_ENTRY_LEN long, so length % DIRECTORY_ENTRY_LEN should be 0
            if (directory.Length % DIRECTORY_ENTRY_LEN != 0)
                marc.AddWarnings("Invalid directory length.");

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
                    marc.AddWarnings("Invalid Directory Tag Length for tag " + tag + ".");
                }

                try
                {
                    fieldOffset = Convert.ToInt32(directory.Substring(i * DIRECTORY_ENTRY_LEN, DIRECTORY_ENTRY_LEN).Substring(7, 5)) + totalExtraBytesRead;
                }
                catch (FormatException)
                {
                    marc.AddWarnings("Invalid Directory Offset for tag " + tag + ".");
                }

                //Check Directory validity

				//If a tag isn't valid, default it to ZZZ. This should at least make the record valid enough to be readable and not throw exceptions
				if (!Field.ValidateTag(tag))
				{
					marc.AddWarnings("Invalid tag " + tag + " in directory.");
					tag = "ZZZ";
				}

                if (fieldOffset + fieldLength > recordLength)
                    marc.AddWarnings("Directory entry for tag " + tag + " runs past the end of the record.");

				int fieldStart = dataStart + fieldOffset - (totalExtraBytesRead * 2);
				if (fieldStart > recordLength)
				{
					marc.AddWarnings("Directory entry for tag " + tag + " starts past the end of the record. Skipping tag and all proceeding tags.");
					break;
				}
				else if (fieldStart + fieldLength > recordLength)
				{
					marc.AddWarnings("Directory entry for tag " + tag + " runs past the end of the record.");
					fieldLength = recordLength - fieldStart;
                }

                string tagData = raw.Substring(fieldStart, fieldLength);

                //Check if there are multi-byte characters in the string
                System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(tagData);
                int extraBytes = fieldLength - stringInfo.LengthInTextElements;
                int extraBytes2 = Encoding.UTF8.GetByteCount(tagData) - fieldLength;
                int endOfFieldIndex = tagData.IndexOf(END_OF_FIELD);

                if (tagData.Length - 1 != endOfFieldIndex)
                {
                    int differenceLength = tagData.Length - 1 - endOfFieldIndex;

                    if (differenceLength != extraBytes && differenceLength != extraBytes2)
                    {
                        fieldLength -= differenceLength;
                        totalExtraBytesRead += differenceLength;
                        tagData = raw.Substring(fieldStart, endOfFieldIndex + 1);
                    }
                    else
                    {
                        if (extraBytes > 0)
                        {
                            fieldLength -= extraBytes;
                            totalExtraBytesRead += extraBytes;
                            tagData = raw.Substring(fieldStart, fieldLength);
                        }
                        else if (extraBytes2 > 0)
                        {
                            fieldLength -= extraBytes2;
                            totalExtraBytesRead += extraBytes2;
                            tagData = raw.Substring(fieldStart, fieldLength);
                        }
                    }
                }

                if (fieldLength > 0)
                {
                    string endCharacter = tagData.Substring(tagData.Length - 1, 1);
                    if (endCharacter == END_OF_FIELD.ToString())
                    {
                        //Get rid of the end of tag character
                        tagData = tagData.Remove(tagData.Length - 1);
                        fieldLength--;
                    }
                    else
                    {
                        marc.AddWarnings("Field for tag " + tag + " does not end with an end of field character.");
                    }
                }
                else
                    marc.AddWarnings("Field for tag " + tag + " has a length of 0.");

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
                        marc.AddWarnings("Invalid indicator length. Forced indicators to blanks for tag " + tag + ".");
                        ind1 = ind2 = ' ';
                    }
                    else
                    {
                        ind1 = char.ToLower(indicators[0]);

						if (!DataField.ValidateIndicator(ind1))
						{
							ind1 = ' ';
							marc.AddWarnings("Invalid first indicator. Forced first indicator to blank for tag " + tag + ".");
						}

                        ind2 = char.ToLower(indicators[1]);

						if (!DataField.ValidateIndicator(ind2))
						{
							ind2 = ' ';
							marc.AddWarnings("Invalid second indicator. Forced second indicator to blank for tag " + tag + ".");
						}
                    }

                    //Split the subfield data into subfield name and data pairs
                    List<Subfield> subfieldData = new List<Subfield>();
                    foreach (string subfield in rawSubfields)
                    {
                        if (subfield.Length > 0)
                            subfieldData.Add(new Subfield(subfield[0], subfield.Substring(1)));
                        else
                            marc.AddWarnings("No subfield data found in tag " + tag + ".");
                    }

                    if (subfieldData.Count == 0)
                        marc.AddWarnings("No subfield data found in tag " + tag + ".");

                    marc.Fields.Add(new DataField(tag, subfieldData, ind1, ind2));
                }
            }
            return marc;
        }

        /// <summary>
        /// Cleans the raw MARC records to remove illegal stuff that sometimes occurs between records.
        /// </summary>
        /// <param name="source">The raw source data.</param>
        /// <returns></returns>
        private string CleanSource(string source)
        {
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
            position = -1;
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
