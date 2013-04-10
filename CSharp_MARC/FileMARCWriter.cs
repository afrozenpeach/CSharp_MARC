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
 * @copyright 2009-2012 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
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

        private string filename = null;
        private FileStream writer = null;
        public const char END_OF_FILE = '\x1A';

        #endregion

		//Constructors
		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
        /// Will overwrite the given filename
        /// </summary>
        /// <param name="filename">The filename.</param>
		public FileMARCWriter(string filename) : this(filename, FileMode.Create)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMARCWriter"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The FileMode.</param>
        public FileMARCWriter(string filename, FileMode mode)
        {
            this.filename = filename;
            writer = new FileStream(filename, mode);
        }

		#endregion

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(Record record)
        {
            string raw = record.ToRaw();
            char[] rawCharArray = raw.ToCharArray();
            foreach (char c in rawCharArray)
            {
				if (record.Leader[9] == ' ')
					writer.WriteByte(GetMARC8Byte(c));
				else
				{
					byte[] array = Encoding.UTF8.GetBytes(new char[] { c });
					writer.Write(array, 0, array.Length);
				}
            }
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
            writer.WriteByte(Convert.ToByte(END_OF_FILE));
        }

        /// <summary>
        /// Gets the MARC8 byte.
		/// Right now this just takes care of most of the single byte special characters, such as the copyright symbol. 
        /// </summary>
        /// <param name="c">The c.</param>
        private byte GetMARC8Byte(char c)
        {
            byte b;

            switch (c)
            {
                case 'Ł':
                    b = 161;
                    break;
                case 'Ø':
                    b = 162;
                    break;
                case 'Đ':
                    b = 163;
                    break;
                case 'Þ':
                    b = 164;
                    break;
                case 'Æ':
                    b = 165;
                    break;
                case 'Œ':
					b = 166;
                    break;
                case 'ʹ':
                    b = 167;
                    break;
                case '·':
					b = 168;
                    break;
                case '♭':
					b = 169;
                    break;
                case '®':
					b = 170;
                    break;
                case '±':
					b = 171;
                    break;
                case 'Ơ':
					b = 172;
                    break;
                case 'Ư':
					b = 173;
                    break;
                case 'ʼ':
					b = 174;
                    break;
                case 'ʻ':
					b = 176;
                    break;
                case 'ł':
                    b = 177;
                    break;
                case 'ø':
					b = 178;
                    break;
                case 'đ':
                    b = 179;
                    break;
                case 'þ':
                    b = 180;
                    break;
                case 'æ':
                    b = 181;
                    break;
                case 'œ':
                    b = 182;
                    break;
                case 'ʺ':
                    b = 183;
                    break;
                case 'ı':
                    b = 184;
                    break;
                case '£':
                    b = 185;
                    break;
                case 'ð':
                    b = 186;
                    break;
                case 'ơ':
                    b = 189;
                    break;
                case 'ư':
                    b = 190;
                    break;
                case '°':
                    b = 192;
                    break;
                case 'ℓ':
                    b = 193;
                    break;
                case '℗':
                    b = 194;
                    break;
                case '©':
                    b = 195;
                    break;
                case '♯':
                    b = 196;
                    break;
                case '¿':
                    b = 197;
                    break;
                case '¡':
                    b = 198;
                    break;
                case 'ß':
                    b = 199;
                    break;
                case '€':
                    b = 200;
                    break;
                default:
					b = Convert.ToByte(c);
                    break;
            }

            return b;
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
