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
using System.Collections;
using System.IO;

namespace MARC
{
	/// <summary>
	/// This is a wrapper for FileMARC that allows for reading large files without loading the entire file into memory.
	/// </summary>
	public class FileMARCReader : IEnumerable, IDisposable
	{
		//Member Variables and Properties
		#region Member Variables and Properties

		private string filename = null;
		private FileStream reader = null;

		#endregion

		//Constructors
		#region Constructors

		public FileMARCReader(string filename)
		{
			this.filename = filename;
			reader = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read);
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
			int bufferSize = 10 * 1024 * 1024; // Read 10 MB at a time
			if (bufferSize > reader.Length)
				bufferSize = Convert.ToInt32(reader.Length);

			byte[] ByteArray = new byte[bufferSize];

			while (reader.Position < reader.Length)
			{
				int DelPosition, RealReadSize;
				string encoded;

				do
				{
					RealReadSize = reader.Read(ByteArray, 0, bufferSize);

					if (RealReadSize != bufferSize)
						Array.Resize(ref ByteArray, RealReadSize);

					DelPosition = Array.LastIndexOf(ByteArray, Convert.ToByte(FileMARC.END_OF_RECORD)) + 1;

					if (DelPosition == 0 & RealReadSize == bufferSize)
					{
						bufferSize *= 2;
						ByteArray = new byte[bufferSize];
					}
				} while (DelPosition == 0 & RealReadSize == bufferSize);

				//Some files will have trailer characters, usually a hex code 1A. 
				//The record has to at least be longer than the leader length of a MARC record, so it's a good place to make sure we have enough to at least try and make a record
				//Otherwise we will relying error checking in the FileMARC class
				if (ByteArray.Length > FileMARC.LEADER_LEN)
				{
					reader.Position = reader.Position - (RealReadSize - DelPosition);
					char marc8utf8Flag = Convert.ToChar(ByteArray[9]);

                    if (marc8utf8Flag == ' ')
                    {
                        Encoding encoding = new MARC8();
                        encoded = encoding.GetString(ByteArray, 0, DelPosition);
                    }
                    else
                    {
                        encoded = Encoding.UTF8.GetString(ByteArray, 0, DelPosition);
                        encoded = encoded.Substring(1); //remove UTF8 Byte Order Mark
                    }

					FileMARC marc = new FileMARC(encoded);
					foreach (Record marcRecord in marc)
					{
						yield return marcRecord;
					}
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			reader.Dispose();
		}

		#endregion
	}
}
