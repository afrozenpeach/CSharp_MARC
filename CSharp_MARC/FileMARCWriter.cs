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
using System.Linq;
using System.Text;
using System.IO;

namespace MARC
{
    public class FileMARCWriter : IDisposable
    {
        //Member Variables and Properties
        #region Member Variables and Properties

		public enum RecordEncoding { MARC8, UTF8 };
        public const char END_OF_FILE = '\x1A';

        private string filename = null;
        private StreamWriter writer = null;
		private Encoding encoding;

        #endregion

		//Constructors
		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
        /// Will overwrite the given filename
        /// </summary>
        /// <param name="filename">The filename.</param>
		public FileMARCWriter(string filename) : this(filename, RecordEncoding.MARC8)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The FileMode.</param>
        public FileMARCWriter(string filename, RecordEncoding recordEncoding) : this(filename, recordEncoding, false)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="append">if set to <c>true</c> [append].</param>
		public FileMARCWriter(string filename, bool append) : this(filename, RecordEncoding.MARC8, append)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="recordEncoding">The record encoding.</param>
		/// <param name="append">if set to <c>true</c> [append].</param>
		public FileMARCWriter(string filename, RecordEncoding recordEncoding, bool append)
		{
			this.filename = filename;

			if (recordEncoding == RecordEncoding.MARC8)
				encoding = new MARC8();
			else
				encoding = Encoding.UTF8;

			writer = new StreamWriter(filename, append, encoding);
		}

		#endregion

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(Record record)
        {
            //Fix the leader if it doesn't match the correct encoding
            if (encoding.EncodingName == "MARC8" && record.Leader[9] != ' ')
            {
                record.Leader = record.Leader.Remove(9, 1);
                record.Leader = record.Leader.Insert(9, " ");
            }
            else if (encoding.EncodingName == "Unicode (UTF-8)" && record.Leader[9] != 'a')
            {
                record.Leader = record.Leader.Remove(9, 1);
                record.Leader = record.Leader.Insert(9, "a");
            }

            string raw = record.ToRaw(encoding);

			writer.Write(raw);
        }

        /// <summary>
        /// Writes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Write(List<Record> records)
        {
            foreach (Record record in records)
                Write(record);
        }

        /// <summary>
        /// Writes the end of file marker.
        /// </summary>
        public void WriteEnd()
		{
			writer.Write(END_OF_FILE);
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
